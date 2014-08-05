using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("M8/UI/ModalManager")]
public class UIModalManager : MonoBehaviour {
    public delegate void CallbackBool(bool b);

    [System.Serializable]
    public class UIData {
        public string name;
        
        [SerializeField]
        UIController _ui;

        public bool exclusive = true; //hide modals behind
        public bool isPrefab;
        public Transform instantiateTo; //target to instantiate ui if it's a prefab
        public Vector3 instantiateOfs;

        private UIController mUI;

        public UIController ui {
            get {
                if(!mUI) {
                    if(isPrefab) {
                        mUI = (UIController)Object.Instantiate(_ui);
                        mUI.transform.parent = instantiateTo;
                        mUI.transform.localPosition = instantiateOfs;
                        mUI.transform.localScale = Vector3.one;
                    }
                    else
                        mUI = _ui;
                }

                return mUI;
            }
        }

#if UNITY_EDITOR
        public UIController e_ui { get { return _ui; } set { _ui = value; } }
#endif
    }

    public UIData[] uis;

    public string openOnStart = "";
        
    [SerializeField]
    bool persistent = false;
        
    private static UIModalManager mInstance;

    private Dictionary<string, UIData> mModals;
    private Stack<UIData> mModalStack;
    private Queue<UIData> mModalToOpen;

    public event CallbackBool activeCallback;
        
    public static UIModalManager instance {
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

    public T ModalGetController<T>(string modal) where T : UIController {
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
        ModalClearStack(false);

        //cancel opening other modals
        mModalToOpen.Clear();

        ModalPushToStack(modal, false);

    }

    public void ModalOpen(string modal) {
        ModalPushToStack(modal, true);
    }

    public void ModalCloseTop() {
        //TODO: check queue?

        if(mModalStack.Count > 0) {
            UIData uid = mModalStack.Pop();
            UIController ui = uid.ui;
            ui._active(false);
            ui._close();
            ui.gameObject.SetActive(false);

            if(mModalStack.Count == 0) {
                ModalInactive();
            }
            else {
                //re-show top
                UIData prevUID = mModalStack.Peek();
                UIController prevUI = prevUID.ui;
                if(!prevUI.gameObject.activeSelf) {
                    prevUI.gameObject.SetActive(true);
                }

                prevUI._active(true);

                //show modal behind this one if this is not exclusive
                if(!prevUID.exclusive) {
                    foreach(UIData prevUId in mModalStack) {
                        if(prevUId != prevUID) {
                            prevUId.ui.gameObject.SetActive(true);
                            if(prevUId.exclusive)
                                break;
                        }
                    }
                }
            }
        }
    }

    public void ModalCloseAll() {
        ModalClearStack(true);

        //cancel opening other modals
        mModalToOpen.Clear();
    }

    void ModalPushToStack(string modal, bool evokeActive) {
        if(evokeActive && mModalStack.Count == 0) {
            if(activeCallback != null)
                activeCallback(true);
        }

        UIData uid = null;
        if(!mModals.TryGetValue(modal, out uid)) {
            Debug.LogError("Modal not found: " + modal);
            return;
        }

        //wait for an update, this is to allow the ui game object to initialize properly
        mModalToOpen.Enqueue(uid);
    }

    void ModalClearStack(bool evokeInactive) {
        if(mModalStack.Count > 0) {
            foreach(UIData uid in mModalStack) {
                UIController ui = uid.ui;
                ui._active(false);
                ui._close();
                ui.gameObject.SetActive(false);
            }

            mModalStack.Clear();

            if(evokeInactive) {
                ModalInactive();
            }
        }
    }

    void ModalInactive() {
        if(activeCallback != null)
            activeCallback(false);
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
                UIController ui = uid.ui;
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

    void LateUpdate() {
        //open all queued modals
        while(mModalToOpen.Count > 0) {
            UIData uid = mModalToOpen.Dequeue();

            if(mModalStack.Count > 0) {
                //hide below
                UIData prevUID = mModalStack.Peek();
                UIController prevUI = prevUID.ui;
                prevUI._active(false);

                if(uid.exclusive) {
                    foreach(UIData prevUId in mModalStack) {
                        prevUId.ui.gameObject.SetActive(false);
                    }
                }
            }

            UIController ui = uid.ui;
            ui.gameObject.SetActive(true);
            ui._open();
            ui._active(true);

            mModalStack.Push(uid);
        }
    }
}
