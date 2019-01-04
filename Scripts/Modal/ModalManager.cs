using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    [AddComponentMenu("M8/Modal/Manager")]
    public class ModalManager : MonoBehaviour {
        [Tooltip("Set this to true to allow this manager to be accessed via ModalManager.main")]
        public bool isMain;

        [Tooltip("Instantiate to root, and add to manager.")]
        public ModalController[] prefabs;

        [Tooltip("Modals reside here, any Controllers found here are added to manager. Prefabs are instantiated here. If null, use this transform.")]
        public Transform root;

        public static ModalManager main { get; private set; }
                
        public string openOnStart = "";
                
        public int activeCount { get { return mModalStack.Count; } }

        public bool isBusy { get; private set; }

        public event System.Action<bool> activeCallback;

        private enum UICommandType {
            Push,
            Pop,
            PopTo,
            PopToInclusive,
            PopAll
        }

        private struct Command {
            public UICommandType type;
            public GenericParams parms;
            public IModalController ctrl;
        }

        private Dictionary<string, IModalController> mModals;
        private Stack<IModalController> mModalStack;
        private CacheList<IModalController> mModalCache;
        private Queue<Command> mCommands;

        public MonoBehaviour GetBehaviour(string modal) {
            IModalController ctrl;
            if(mModals.TryGetValue(modal, out ctrl))
                return ctrl.behaviour;
            return null;
        }

        public T GetBehaviour<T>(string modal) where T:MonoBehaviour {
            return GetBehaviour(modal) as T;
        }

        public bool IsInStack(string modal) {
            bool ret = false;
            foreach(var ctrl in mModalStack) {
                if(ctrl.id == modal) {
                    ret = true;
                    break;
                }
            }
            return ret;
        }

        public string GetTop() {
            if(mModalStack.Count > 0)
                return mModalStack.Peek().id;

            return "";
        }

        public MonoBehaviour GetTopBehaviour() {
            if(mModalStack.Count > 0)
                return mModalStack.Peek().behaviour;

            return null;
        }

        public T GetTopBehaviour<T>() where T:MonoBehaviour {
            return GetTopBehaviour() as T;
        }

        /// <summary>
        /// Closes all modal and open this.
        /// </summary>
        public void Replace(string modal, GenericParams parms) {
            CloseAll();
            Open(modal, parms);
        }

        public void Open(string modal) {
            Open(modal, null);
        }

        public void Open(string modal, GenericParams parms) {
            IModalController ctrl;
            if(!mModals.TryGetValue(modal, out ctrl)) {
                Debug.LogError("Modal not found: " + modal);
                return;
            }

            //wait for an update, this is to allow the ui game object to initialize properly
            mCommands.Enqueue(new Command() { type = UICommandType.Push, ctrl = ctrl, parms = parms });

            if(!isBusy)
                StartCoroutine(DoTask());
        }

        public void CloseTop() {
            mCommands.Enqueue(new Command() { type = UICommandType.Pop });

            if(!isBusy)
                StartCoroutine(DoTask());
        }

        public void CloseAll() {
            mCommands.Enqueue(new Command() { type = UICommandType.PopAll });

            if(!isBusy)
                StartCoroutine(DoTask());
        }

        public void CloseUpTo(string modal, bool inclusive) {
            IModalController ctrl;
            if(!mModals.TryGetValue(modal, out ctrl)) {
                return;
            }

            mCommands.Enqueue(new Command() { type = inclusive ? UICommandType.PopToInclusive : UICommandType.PopTo, ctrl = ctrl });

            if(!isBusy)
                StartCoroutine(DoTask());
        }

        void OnDisable() {
            isBusy = false;
        }

        void OnDestroy() {
            if(main == this)
                main = null;
        }

        void Awake() {
            if(isMain)
                main = this;

            if(!root)
                root = transform;

            mModals = new Dictionary<string, IModalController>();
            
            //grab modals in root, and register
            for(int i = 0; i < root.childCount; i++) {
                //TODO: ModalController dependency, maybe just look for IModalController
                var ctrl = root.GetChild(i).GetComponent<ModalController>() as IModalController;
                if(ctrl != null) {
                    //prevent duplicate
                    if(!mModals.ContainsKey(ctrl.id)) {
                        ctrl.Init();
                        mModals.Add(ctrl.id, ctrl);
                        ctrl.SetOwner(this);
                    }
                }
            }

            //instantiate prefabs, put them in root, and register
            for(int i = 0; i < prefabs.Length; i++) {
                if(!prefabs[i])
                    continue;

                //TODO: ModalController dependency, maybe just look for IModalController
                var prefabCtrl = prefabs[i] as IModalController;

                //prevent duplicate
                if(prefabCtrl == null || mModals.ContainsKey(prefabCtrl.id))
                    continue;

                ModalController ctrl = Instantiate(prefabs[i], root, false);

                var ctrlTrans = ctrl.transform;
                ctrlTrans.localPosition = Vector3.zero;
                ctrlTrans.localRotation = Quaternion.identity;
                ctrlTrans.localScale = Vector3.one;

                //init and add
                var iCtrl = ctrl as IModalController;
                iCtrl.Init();
                mModals.Add(iCtrl.id, iCtrl);
                iCtrl.SetOwner(this);
            }

            int maxModalCount = mModals.Count;

            mModalStack = new Stack<IModalController>(maxModalCount);
            mCommands = new Queue<Command>(maxModalCount);
            mModalCache = new CacheList<IModalController>(maxModalCount);
        }

        void Start() {
            if(!string.IsNullOrEmpty(openOnStart)) {
                Open(openOnStart, new GenericParams());
            }
        }

        IEnumerator DoPop(IModalController ctrl) {
            ctrl.SetActive(false);

            ctrl.Close();

            yield return ctrl.Closing();

            ctrl.Pop();

            ctrl.SetShow(false);
        }

        IEnumerator DoOpenCurrent() {
            if(mModalStack.Count > 0) {
                mModalCache.Clear();
                foreach(var ctrl in mModalStack) {
                    mModalCache.Add(ctrl);
                    if(ctrl.isExclusive)
                        break;
                }

                //open from back to front
                for(int i = mModalCache.Count - 1; i >= 0; i--) {
                    var ctrl = mModalCache[i];

                    ctrl.SetShow(true);

                    ctrl.Open();

                    yield return ctrl.Opening();
                }

                //set top active
                mModalStack.Peek().SetActive(true);
            }
        }

        IEnumerator DoCloseCurrent() {
            if(mModalStack.Count > 0) {
                //set top inactive
                mModalStack.Peek().SetActive(false);

                //close from front to back
                foreach(var ctrl in mModalStack) {
                    ctrl.Close();

                    yield return ctrl.Closing();

                    ctrl.SetShow(false);

                    if(ctrl.isExclusive) //things below are already closed
                        break;
                }
            }
        }

        IEnumerator DoTask() {
            isBusy = true;

            int lastCount = mModalStack.Count;

            while(mCommands.Count > 0) {
                var command = mCommands.Dequeue();

                bool openCurrent = false;

                switch(command.type) {
                    case UICommandType.Push:
                        var pushCtrl = command.ctrl;

                        if(pushCtrl.isExclusive) //close previous
                            yield return DoCloseCurrent();

                        mModalStack.Push(pushCtrl);

                        pushCtrl.SetShow(true);

                        pushCtrl.Push(command.parms);

                        pushCtrl.Open();

                        yield return pushCtrl.Opening();

                        pushCtrl.SetActive(true);
                        break;
                    case UICommandType.PopTo:
                        int popToLastStack = mModalStack.Count;
                        openCurrent = false;

                        while(mModalStack.Count > 0) {
                            if(mModalStack.Peek() == command.ctrl)
                                break;

                            var ctrl = mModalStack.Pop();

                            if(ctrl.isExclusive)
                                openCurrent = true;

                            yield return DoPop(ctrl);
                        }

                        if(openCurrent) {
                            if(popToLastStack != mModalStack.Count) //make sure there are actual pops
                                yield return DoOpenCurrent();
                        }
                        break;
                    case UICommandType.PopToInclusive:
                        openCurrent = false;

                        while(mModalStack.Count > 0) {
                            var ctrl = mModalStack.Pop();

                            if(ctrl.isExclusive)
                                openCurrent = true;

                            yield return DoPop(ctrl);

                            if(ctrl == command.ctrl)
                                break;
                        }

                        if(openCurrent)
                            yield return DoOpenCurrent();
                        break;
                    case UICommandType.PopAll:
                        while(mModalStack.Count > 0) {
                            var ctrl = mModalStack.Pop();
                            yield return DoPop(ctrl);
                        }
                        break;
                    case UICommandType.Pop:
                        if(mModalStack.Count > 0) {
                            var ctrl = mModalStack.Pop();
                            openCurrent = ctrl.isExclusive;
                            yield return DoPop(ctrl);

                            if(openCurrent)
                                yield return DoOpenCurrent();
                        }
                        break;
                }
            }

            if(activeCallback != null) {
                if(lastCount > 0) {
                    //no more modals
                    if(mModalStack.Count == 0)
                        activeCallback(false);
                }
                else if(mModalStack.Count > 0) {
                    //modal active
                    activeCallback(true);
                }
            }

            isBusy = false;
        }
    }
}