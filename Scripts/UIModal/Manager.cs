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

            private Interface.IOpen[] mIOpens;
            private Interface.IOpening[] mIOpenings;
            private Interface.IClosing[] mIClosings;
            private Interface.IClose[] mICloses;
            private Interface.IActive[] mIActives;

            public Controller ui {
                get {
                    if(!mUI)
                        GrabUI();
                    return mUI;
                }
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
                    var comps = mUI.GetComponents(typeof(Interface.IOpen));
                    mIOpens = new Interface.IOpen[comps.Length];
                    for(int i = 0; i < comps.Length; i++) mIOpens[i] = comps[i] as Interface.IOpen;

                    comps = mUI.GetComponents(typeof(Interface.IOpening));
                    mIOpenings = new Interface.IOpening[comps.Length];
                    for(int i = 0; i < comps.Length; i++) mIOpenings[i] = comps[i] as Interface.IOpening;
                    
                    comps = mUI.GetComponents(typeof(Interface.IClosing));
                    mIClosings = new Interface.IClosing[comps.Length];
                    for(int i = 0; i < comps.Length; i++) mIClosings[i] = comps[i] as Interface.IClosing;

                    comps = mUI.GetComponents(typeof(Interface.IClose));
                    mICloses = new Interface.IClose[comps.Length];
                    for(int i = 0; i < comps.Length; i++) mICloses[i] = comps[i] as Interface.IClose;

                    comps = mUI.GetComponents(typeof(Interface.IActive));
                    mIActives = new Interface.IActive[comps.Length];
                    for(int i = 0; i < comps.Length; i++) mIActives[i] = comps[i] as Interface.IActive;
                }
                else
                    Debug.LogWarning("UIController not set.");
            }
        }

        public UIData[] uis;

        public string openOnStart = "";

        private Dictionary<string, UIData> mModals;
        private Stack<UIData> mModalStack;
        private Queue<UIData> mModalToOpen;
        
        private int mModalCloseCount;
        
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
        }

        protected override void OnInstanceInit() {
            mModals = new Dictionary<string, UIData>(uis.Length);
            mModalStack = new Stack<UIData>(uis.Length);
            mModalToOpen = new Queue<UIData>(uis.Length);

            //setup data and deactivate object
            for(int i = 0; i < uis.Length; i++) {
                var uid = uis[i];
                var ui = uid.ui;
                if(ui) {
                    ui.root.SetActive(false);
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
            if(mModalCloseCount > 0) {
                while(mModalCloseCount > 0 && mModalStack.Count > 0) {
                    var uid = mModalStack.Pop();
                    var ui = uid.ui;

                    yield return uid.Closing();

                    uid.Close();

                    uid.SetActive(false);

                    ui.root.SetActive(false);

                    mModalCloseCount--;
                }

                mModalCloseCount = 0;

                //open new top
                if(mModalStack.Count > 0) {
                    var prevUID = mModalStack.Peek();
                    var prevUI = prevUID.ui;

                    prevUI.root.SetActive(true);

                    prevUID.Open();

                    yield return prevUID.Opening();

                    prevUID.SetActive(true);
                }
            }

            //open new modals
            UIData uidLastOpen = null;

            while(mModalToOpen.Count > 0) {
                uidLastOpen = mModalToOpen.Dequeue();
                var uiLastOpen = uidLastOpen.ui;

                if(mModalStack.Count > 0) {
                    var prevUID = mModalStack.Peek();
                    var prevUI = prevUID.ui;

                    if(uiLastOpen.exclusive) {
                        //hide below
                        yield return prevUID.Closing();

                        prevUID.Close();

                        prevUID.SetActive(false);

                        prevUI.root.SetActive(false);
                    }
                    else //just set inactive
                        prevUID.SetActive(false);
                }

                //open
                mModalStack.Push(uidLastOpen);

                uiLastOpen.root.SetActive(true);

                uidLastOpen.Open();

                yield return uidLastOpen.Opening();
            }

            if(uidLastOpen != null)
                uidLastOpen.SetActive(true);

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