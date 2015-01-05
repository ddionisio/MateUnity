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
            private TransitionBase mTransition;

            public Controller ui {
                get {
                    if(!mUI)
                        GrabUI();
                    return mUI;
                }
            }

            public TransitionBase transition {
                get {
                    if(!mUI) 
                        GrabUI(); //this will also grab transition
                    return mTransition;
                }
            }

#if UNITY_EDITOR
            public Controller e_ui { get { return _ui; } set { _ui = value; } }
#endif

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

                mTransition = ui.GetComponent<TransitionBase>();
            }
        }

        public UIData[] uis;

        public string openOnStart = "";

        private Dictionary<string, UIData> mModals;
        private Stack<UIData> mModalStack;
        private Queue<UIData> mModalToOpen;
        
        private int mModalCloseCount;

        private TransitionBase[] mTransitions;
        private int mTransitionCount;

        private bool mTaskActive;

        public event CallbackBool activeCallback;

        public int activeCount {
            get {
                return mModalStack.Count;
            }
        }

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
        public void ModalReplace(string modal) {
            ModalCloseAll();

            ModalOpen(modal);

        }

        public void ModalOpen(string modal) {
            UIData uid = null;
            if(!mModals.TryGetValue(modal, out uid)) {
                Debug.LogError("Modal not found: " + modal);
                return;
            }

            //wait for an update, this is to allow the ui game object to initialize properly
            mModalToOpen.Enqueue(uid);

            if(!mTaskActive)
                StartCoroutine(DoTask());
        }

        public void ModalCloseTop() {
            if(mModalCloseCount < mModalStack.Count) {
                mModalCloseCount++;

                if(!mTaskActive)
                    StartCoroutine(DoTask());
            }
        }

        public void ModalCloseAll() {
            mModalCloseCount = mModalStack.Count;

            if(!mTaskActive)
                StartCoroutine(DoTask());
        }

        public void ModalCloseUpTo(string modal, bool inclusive) {
            foreach(UIData dat in mModalStack) {
                if(dat.name == modal) {
                    if(inclusive)
                        mModalCloseCount++;
                    break;
                }
                else
                    mModalCloseCount++;
            }

            if(!mTaskActive)
                StartCoroutine(DoTask());
        }

        void OnDisable() {
            mTaskActive = false;
            mTransitionCount = 0;
        }

        protected override void Awake() {
            base.Awake();

            mModals = new Dictionary<string, UIData>(uis.Length);
            mModalStack = new Stack<UIData>(uis.Length);
            mModalToOpen = new Queue<UIData>(uis.Length);
            mTransitions = new TransitionBase[uis.Length];

            //setup data and deactivate object
            for(int i = 0; i < uis.Length; i++) {
                UIData uid = uis[i];
                Controller ui = uid.ui;
                if(ui != null) {
                    ui.gameObject.SetActive(false);
                }

                mModals.Add(uid.name, uid);
            }
        }

        void Start() {
            if(!string.IsNullOrEmpty(openOnStart)) {
                ModalOpen(openOnStart);
            }
        }

        IEnumerator DoTask() {
            mTaskActive = true;

            int lastCount = mModalStack.Count;

            //close modals
            while(mModalCloseCount > 0) {
                if(mModalStack.Count > 0) {
                    UIData uid = mModalStack.Pop();
                    Controller ui = uid.ui;
                    ui._active(false);

                    if(ui.gameObject.activeSelf) {
                        ui.Close();
                        //wait for closing? (animation, etc)
                        if(uid.transition)
                            yield return StartCoroutine(uid.transition.Close());

                        ui.gameObject.SetActive(false);
                    }

                    //last modal to close?
                    if(mModalCloseCount == 1 && mModalStack.Count > 0) {
                        UIData prevUID = mModalStack.Peek();
                        Controller prevUI = prevUID.ui;

                        if(uid.exclusive) {
                            //open previous UIs
                            yield return StartCoroutine(DoOpenPrevs());
                        }

                        prevUI._active(true);
                    }

                    mModalCloseCount--;
                }
                else //ran out of things to close?
                    mModalCloseCount = 0;
            }

            //open new modals
            Controller uiLastOpen = null;

            while(mModalToOpen.Count > 0) {
                UIData uid = mModalToOpen.Dequeue();

                if(mModalStack.Count > 0) {
                    UIData prevUID = mModalStack.Peek();
                    Controller prevUI = prevUID.ui;

                    prevUI._active(false);

                    //hide below
                    if(uid.exclusive) {
                        yield return StartCoroutine(DoClosePrevs());
                    }
                }

                //open
                mModalStack.Push(uid);

                uiLastOpen = uid.ui;
                uiLastOpen.gameObject.SetActive(true);
                uiLastOpen.Open();

                if(uid.transition)
                    yield return StartCoroutine(uid.transition.Open());
            }

            if(uiLastOpen)
                uiLastOpen._active(true);

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

        IEnumerator DoOpenPrevs() {
            mTransitionCount = 0;

            //re-show modals behind
            foreach(UIData prevUId in mModalStack) {
                prevUId.ui.gameObject.SetActive(true);
                prevUId.ui.Open();

                TransitionBase trans = prevUId.transition;
                if(trans) {
                    mTransitions[mTransitionCount] = trans;
                    mTransitionCount++;
                    StartCoroutine(trans.Open());
                }

                if(prevUId.exclusive)
                    break;
            }

            //wait till everything is done
            for(int i = 0; i < mTransitionCount; i++) {
                while(mTransitions[i].state == TransitionBase.State.Open)
                    yield return null;
            }

            mTransitionCount = 0;
        }

        IEnumerator DoClosePrevs() {
            mTransitionCount = 0;

            //re-show modals behind
            foreach(UIData prevUId in mModalStack) {
                prevUId.ui.Close();

                TransitionBase trans = prevUId.transition;
                if(trans) {
                    mTransitions[mTransitionCount] = trans;
                    mTransitionCount++;
                    StartCoroutine(trans.Close());
                }
                else
                    prevUId.ui.gameObject.SetActive(false);

                if(prevUId.exclusive)
                    break;
            }

            //wait till everything is done
            for(int i = 0; i < mTransitionCount; i++) {
                while(mTransitions[i].state == TransitionBase.State.Close)
                    yield return null;

                mTransitions[i].gameObject.SetActive(false);
            }

            mTransitionCount = 0;
        }
    }
}