using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace M8.UIModal {
    [AddComponentMenu("M8/UI Modal/Manager")]
    public class Manager : SingletonBehaviour<Manager> {
        public delegate void CallbackBool(bool b);

        [System.Serializable]
        public class UIData {
            public string name;

            [SerializeField]
            Controller _ui;

            public bool exclusive { get { return _ui ? _ui.exclusive : false; } }

            public bool isPrefab;
            public Transform instantiateTo; //target to instantiate ui if it's a prefab

            private Controller mUI;

            private Interface.IPush[] mIPushes;
            private Interface.IPop[] mIPops;
            private Interface.IOpen[] mIOpens;
            private Interface.IOpening[] mIOpenings;
            private Interface.IClose[] mICloses;
            private Interface.IClosing[] mIClosings;
            private Interface.IActive[] mIActives;
            
            public Controller ui {
                get {
                    if(!mUI)
                        GrabUI();
                    return mUI;
                }
            }

            public void Push(Params parms) {
                for(int i = 0; i < mIPushes.Length; i++)
                    mIPushes[i].Push(parms);
            }

            public void Pop() {
                for(int i = 0; i < mIPops.Length; i++)
                    mIPops[i].Pop();
            }

            public void Open() {
                for(int i = 0; i < mIOpens.Length; i++)
                    mIOpens[i].Open();
            }

            public void Close() {
                for(int i = 0; i < mICloses.Length; i++)
                    mICloses[i].Close();
            }

            public IEnumerator Opening() {
                for(int i = 0; i < mIOpenings.Length; i++)
                    yield return mIOpenings[i].Opening();
            }

            public IEnumerator Closing() {
                for(int i = 0; i < mIClosings.Length; i++)
                    yield return mIClosings[i].Closing();
            }

            public void SetActive(bool aActive) {
                for(int i = 0; i < mIActives.Length; i++)
                    mIActives[i].SetActive(aActive);
            }

            public Controller e_ui  { get { return _ui; } set { _ui = value; } }

            private void GrabUI() {
                if(isPrefab) {
                    Vector3 p = _ui.transform.localPosition;
                    mUI = (Controller)Object.Instantiate(_ui);
                    mUI.transform.parent = instantiateTo;
                    mUI.transform.localPosition = p;
                    mUI.transform.localScale = Vector3.one;
                }
                else
                    mUI = _ui;

                if(mUI) {
                    var comps = mUI.GetComponents<MonoBehaviour>();

                    var IPushes = new List<Interface.IPush>();
                    var IPops = new List<Interface.IPop>();
                    var IOpens = new List<Interface.IOpen>();
                    var IOpenings = new List<Interface.IOpening>();
                    var ICloses = new List<Interface.IClose>();
                    var IClosings = new List<Interface.IClosing>();
                    var IActives = new List<Interface.IActive>();

                    for(int i = 0; i < comps.Length; i++) {
                        var comp = comps[i];

                        var push = comp as Interface.IPush;
                        if(comp != null) IPushes.Add(push);

                        var pop = comp as Interface.IPop;
                        if(comp != null) IPops.Add(pop);

                        var open = comp as Interface.IOpen;
                        if(comp != null) IOpens.Add(open);

                        var opening = comp as Interface.IOpening;
                        if(comp != null) IOpenings.Add(opening);

                        var close = comp as Interface.IClose;
                        if(comp != null) ICloses.Add(close);

                        var closing = comp as Interface.IClosing;
                        if(comp != null) IClosings.Add(closing);

                        var active = comp as Interface.IActive;
                        if(comp != null) IActives.Add(active);
                    }

                    mIPushes = IPushes.ToArray();
                    mIPops = IPops.ToArray();
                    mIOpens = IOpens.ToArray();
                    mIOpenings = IOpenings.ToArray();
                    mICloses = ICloses.ToArray();
                    mIClosings = IClosings.ToArray();
                    mIActives = IActives.ToArray();
                }
                else
                    Debug.LogWarning("UIController not set.");
            }
        }

        private enum UICommandType {
            Push,
            Pop,
            PopTo,
            PopToInclusive,
            PopAll
        }

        private struct UICommand {
            public UICommandType type;
            public Params parms;
            public UIData uid;
        }

        public UIData[] uis;

        public Transform instantiateTo; //place prefab modals here if they don't have a designated "instantiateTo"

        public string openOnStart = "";

        private Dictionary<string, UIData> mModals;
        private Stack<UIData> mModalStack;
        private Queue<UICommand> mCommands;
        
        private bool mTaskActive;

        public event CallbackBool activeCallback;

        public int activeCount {
            get {
                return mModalStack.Count;
            }
        }

        public bool isBusy { get { return mTaskActive; } }

        public UIData ModalGetData(string modal) {
            UIData dat = null;
            mModals.TryGetValue(modal, out dat);
            return dat;
        }

        public T ModalGetController<T>(string modal) where T : Controller {
            UIData dat = ModalGetData(modal);
            return dat != null ? dat.ui as T : null;
        }

        public bool ModalIsInStack(string modal) {
            bool ret = false;
            foreach(UIData uid in mModalStack) {
                if(uid.name == modal) {
                    ret = true;
                    break;
                }
            }
            return ret;
        }

        public string ModalGetTop() {
            string ret = "";
            if(mModalStack.Count > 0) {
                ret = mModalStack.Peek().name;
            }
            return ret;
        }

        //closes all modal and open this
        public void ModalReplace(string modal, Params parms) {
            ModalCloseAll();

            ModalOpen(modal, parms);
        }

        public void ModalReplace(string modal, params ParamArg[] parms) {
            ModalReplace(modal, new Params(parms));
        }
                
        public void ModalOpen(string modal, Params parms) {
            var uid = ModalGetData(modal);
            if(uid == null) {
                Debug.LogError("Modal not found: " + modal);
                return;
            }

            //wait for an update, this is to allow the ui game object to initialize properly
            mCommands.Enqueue(new UICommand() { type=UICommandType.Push, uid=uid, parms=parms });

            if(!mTaskActive)
                StartCoroutine(DoTask());
        }

        public void ModalOpen(string modal, params ParamArg[] parms) {
            ModalOpen(modal, new Params(parms));
        }

        public void ModalCloseTop() {
            mCommands.Enqueue(new UICommand() { type=UICommandType.Pop });

            if(!mTaskActive)
                StartCoroutine(DoTask());
        }

        public void ModalCloseAll() {
            mCommands.Enqueue(new UICommand() { type=UICommandType.PopAll });

            if(!mTaskActive)
                StartCoroutine(DoTask());
        }

        public void ModalCloseUpTo(string modal, bool inclusive) {
            var uid = ModalGetData(modal);
            if(uid == null)
                return;

            mCommands.Enqueue(new UICommand() { type=inclusive ? UICommandType.PopToInclusive : UICommandType.PopTo, uid=uid });

            if(!mTaskActive)
                StartCoroutine(DoTask());
        }

        void OnDisable() {
            mTaskActive = false;
        }

        protected override void OnInstanceInit() {
            mModals = new Dictionary<string, UIData>();
            mModalStack = new Stack<UIData>();
            mCommands = new Queue<UICommand>();

            //setup data and deactivate object
            if(uis != null) {
                for(int i = 0; i < uis.Length; i++) {
                    var uid = uis[i];

                    if(uid.isPrefab && !uid.instantiateTo) //set default instantiate to
                        uid.instantiateTo = instantiateTo;

                    var ui = uid.ui;
                    if(ui) {
                        ui.root.SetActive(false);
                    }

                    mModals.Add(uid.name, uid);
                }
            }
        }

        void Start() {
            if(!string.IsNullOrEmpty(openOnStart)) {
                ModalOpen(openOnStart, new Params());
            }
        }
        
        IEnumerator DoPop(UIData uid) {
            uid.SetActive(false);

            uid.Close();

            yield return uid.Closing();
                        
            uid.Pop();

            uid.ui.root.SetActive(false);
        }

        IEnumerator DoOpenCurrent() {
            if(mModalStack.Count > 0) {
                var modalsToOpen = new List<UIData>();

                foreach(var uid in mModalStack) {
                    modalsToOpen.Add(uid);
                    if(uid.exclusive)
                        break;
                }

                //open from back to front
                for(int i = modalsToOpen.Count - 1; i >= 0; i--) {
                    var uid = modalsToOpen[i];

                    uid.ui.root.SetActive(true);

                    uid.Open();

                    yield return uid.Opening();
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
                foreach(var uid in mModalStack) {
                    uid.Close();

                    yield return uid.Closing();

                    uid.ui.root.SetActive(false);

                    if(uid.exclusive) //things below are already closed
                        break;
                }
            }
        }

        IEnumerator DoTask() {
            mTaskActive = true;

            int lastCount = mModalStack.Count;

            while(mCommands.Count > 0) {
                var command = mCommands.Dequeue();
                
                switch(command.type) {
                    case UICommandType.Push:
                        var pushUID = command.uid;
                                                
                        if(pushUID.exclusive) //close previous
                            yield return DoCloseCurrent();

                        mModalStack.Push(pushUID);

                        pushUID.ui.root.SetActive(true);

                        pushUID.Push(command.parms);

                        pushUID.Open();

                        yield return pushUID.Opening();

                        pushUID.SetActive(true);
                        break;                                            
                    case UICommandType.PopTo:
                        int popToLastStack = mModalStack.Count;

                        while(mModalStack.Count > 0) {
                            if(mModalStack.Peek() == command.uid)
                                break;

                            var uid = mModalStack.Pop();

                            yield return DoPop(uid);
                        }

                        if(popToLastStack != mModalStack.Count) //make sure there are actual pops
                            yield return DoOpenCurrent();
                        break;
                    case UICommandType.PopToInclusive:
                        while(mModalStack.Count > 0) {
                            var uid = mModalStack.Pop();

                            yield return DoPop(uid);

                            if(uid == command.uid)
                                break;
                        }

                        yield return DoOpenCurrent();
                        break;
                    case UICommandType.PopAll:
                        while(mModalStack.Count > 0) {
                            var uid = mModalStack.Pop();
                            yield return DoPop(uid);
                        }
                        break;
                    case UICommandType.Pop:
                        if(mModalStack.Count > 0) {
                            var uid = mModalStack.Pop();
                            yield return DoPop(uid);

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

            mTaskActive = false;
        }
    }
}