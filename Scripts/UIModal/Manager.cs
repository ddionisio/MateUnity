using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace M8.UIModal {
    [AddComponentMenu("M8/UI Modal/Manager")]
    public class Manager : MonoBehaviour {
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

            public Controller ui {
                get {
                    if(!mUI) {
                        if(isPrefab) {
                            Vector3 p = _ui.transform.localPosition;
                            mUI = (Controller)Object.Instantiate(_ui);
                            mUI.transform.parent = instantiateTo;
                            mUI.transform.localPosition = p;
                            mUI.transform.localScale = Vector3.one;
                        }
                        else
                            mUI = _ui;
                    }

                    return mUI;
                }
            }

#if UNITY_EDITOR
            public Controller e_ui { get { return _ui; } set { _ui = value; } }
#endif
        }

        public UIData[] uis;

        public string openOnStart = "";

        [SerializeField]
        bool persistent = false;

        private static Manager mInstance;

        private Dictionary<string, UIData> mModals;
        private Stack<UIData> mModalStack;
        private Queue<UIData> mModalToOpen;
        private int mModalCloseCount;

        private IEnumerator mTask;

        public event CallbackBool activeCallback;

        public static Manager instance {
            get {
                return mInstance;
            }
        }

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

            if(mTask == null)
                StartCoroutine(mTask = DoTask());
        }

        public void ModalCloseTop() {
            if(mModalCloseCount < mModalStack.Count) {
                mModalCloseCount++;

                if(mTask == null)
                    StartCoroutine(mTask = DoTask());
            }
        }

        public void ModalCloseAll() {
            mModalCloseCount = mModalStack.Count;

            if(mTask == null)
                StartCoroutine(mTask = DoTask());
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

            if(mTask == null)
                StartCoroutine(mTask = DoTask());
        }

        void OnDestroy() {
            if(mInstance == this) {
                mInstance = null;

                activeCallback = null;
            }
        }

        void Awake() {
            if(mInstance == null) {
                mInstance = this;

                mModals = new Dictionary<string, UIData>(uis.Length);
                mModalStack = new Stack<UIData>(uis.Length);
                mModalToOpen = new Queue<UIData>(uis.Length);

                //setup data and deactivate object
                for(int i = 0; i < uis.Length; i++) {
                    UIData uid = uis[i];
                    Controller ui = uid.ui;
                    if(ui != null) {
                        ui.gameObject.SetActive(false);
                    }

                    mModals.Add(uid.name, uid);
                }

                if(persistent)
                    Object.DontDestroyOnLoad(gameObject);
            }
        }

        void Start() {
            if(!string.IsNullOrEmpty(openOnStart)) {
                ModalOpen(openOnStart);
            }
        }

        IEnumerator DoTask() {
            WaitForEndOfFrame wait = new WaitForEndOfFrame();

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
                        while(ui.Closing())
                            yield return wait;

                        ui.gameObject.SetActive(false);
                    }

                    //last modal to close?
                    if(mModalCloseCount == 1 && mModalStack.Count > 0) {
                        UIData prevUID = mModalStack.Peek();
                        Controller prevUI = prevUID.ui;

                        if(uid.exclusive) {
                            //re-show modals behind
                            int openCount = 0;
                            foreach(UIData prevUId in mModalStack) {
                                prevUId.ui.gameObject.SetActive(true);
                                prevUId.ui.Open();
                                openCount++;
                                if(prevUId.exclusive)
                                    break;
                            }

                            //wait till everything is done
                            while(true) {
                                int openComplete = 0;
                                foreach(UIData prevUId in mModalStack) {
                                    if(!prevUId.ui.Opening())
                                        openComplete++;
                                    if(prevUId.exclusive)
                                        break;
                                }

                                if(openComplete == openCount)
                                    break;

                                yield return wait;
                            }
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
                        int closeCount = 0;
                        foreach(UIData prevUId in mModalStack) {
                            prevUId.ui.Close();
                            closeCount++;
                            if(prevUId.exclusive)
                                break;
                        }

                        while(true) {
                            int closeComplete = 0;
                            foreach(UIData prevUId in mModalStack) {
                                if(!prevUId.ui.Closing()) {
                                    prevUId.ui.gameObject.SetActive(false);
                                    closeComplete++;
                                }
                                if(prevUId.exclusive)
                                    break;
                            }

                            if(closeComplete == closeCount)
                                break;

                            yield return wait;
                        }
                    }
                }

                //open
                mModalStack.Push(uid);

                uiLastOpen = uid.ui;
                uiLastOpen.gameObject.SetActive(true);
                uiLastOpen.Open();

                while(uiLastOpen.Opening())
                    yield return wait;
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

            mTask = null;
        }
    }
}