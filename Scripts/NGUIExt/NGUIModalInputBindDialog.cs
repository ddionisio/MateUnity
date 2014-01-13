using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/NGUI/ModalInputBindDialog")]
public class NGUIModalInputBindDialog : UIModalInputBindDialog {
    public GameObject template;

    public Transform itemHolder;

    public UIEventListener saveItem;
    public UIEventListener cancelItem;
    public UIEventListener defaultItem;

    public GameObject keyPressGO;
    public UILabel keyPressActionLabel;
    public UILabel keyPressBindLabel;

    public string defaultConfirmTitle;
    public string defaultConfirmDesc;

    public string cancelConfirmTitle;
    public string cancelConfirmDesc;

    private NGUIModalInputBindItem[] mItems; //corresponds to actions

    private GameObject mLastSelectedObject;

    protected override void OnActive(bool active) {
        if(active) {
            for(int i = 0; i < mItems.Length; i++) {
                mItems[i].primaryListener.onClick = OnKeyClick;
                mItems[i].secondaryListener.onClick = OnKeyClick;
            }

            saveItem.onClick = OnSaveClick;
            cancelItem.onClick = OnCancelClick;
            defaultItem.onClick = OnDefaultClick;

            UICamera.selectedObject = mItems[0].primary;
        }
        else {
            for(int i = 0; i < mItems.Length; i++) {
                mItems[i].primaryListener.onClick = null;
                mItems[i].secondaryListener.onClick = null;
            }
            
            saveItem.onClick = null;
            cancelItem.onClick = null;
            defaultItem.onClick = null;
        }
    }

    void RefreshKeyLabels() {
        InputManager input = Main.instance.input;
        
        for(int i = 0; i < mItems.Length; i++) {
            InputManager.Key key = input.GetBindKey(playerIndex, actions[i].index, actions[i].keys[0]);
            mItems[i].primaryLabel.text = key.isValid ? key.GetKeyString() : "None";
            
            key = input.GetBindKey(playerIndex, actions[i].index, actions[i].keys[1]);
            mItems[i].secondaryLabel.text = key.isValid ? key.GetKeyString() : "None";
        }
        
        NGUILayoutBase.RefreshNow(transform);
    }
    
    protected override void OnOpen() {
        RefreshKeyLabels();
        keyPressGO.SetActive(false);
    }
    
    protected override void OnClose() {
        mLastSelectedObject = null;
        keyPressGO.SetActive(false);
    }

    protected override void Awake() {
        base.Awake();

        mItems = new NGUIModalInputBindItem[actions.Length];

        UIButtonKeys defaultBtnKeys = defaultItem.GetComponent<UIButtonKeys>();
        UIButtonKeys cancelBtnKeys = cancelItem.GetComponent<UIButtonKeys>();

        for(int i = 0; i < mItems.Length; i++) {
            GameObject go = (GameObject)GameObject.Instantiate(template);
            go.name = i.ToString("D2");
            go.transform.parent = itemHolder;
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            go.transform.localRotation = Quaternion.identity;

            mItems[i] = go.GetComponent<NGUIModalInputBindItem>();

            if(i == 0) {
                mItems[i].primaryBtnKeys.selectOnUp = cancelBtnKeys;
                mItems[i].secondaryBtnKeys.selectOnUp = cancelBtnKeys;
            }
            else {
                mItems[i].primaryBtnKeys.selectOnUp = mItems[i - 1].primaryBtnKeys;
                mItems[i].secondaryBtnKeys.selectOnUp = mItems[i - 1].secondaryBtnKeys;

                mItems[i - 1].primaryBtnKeys.selectOnDown = mItems[i].primaryBtnKeys;
                mItems[i - 1].secondaryBtnKeys.selectOnDown = mItems[i].secondaryBtnKeys;
            }

            mItems[i].nameLabel.text = actions[i].localized ? GameLocalize.GetText(actions[i].name) : actions[i].name;
        }

        mItems[mItems.Length - 1].primaryBtnKeys.selectOnDown = defaultBtnKeys;
        mItems[mItems.Length - 1].secondaryBtnKeys.selectOnDown = defaultBtnKeys;

        defaultBtnKeys.selectOnUp = mItems[mItems.Length - 1].primaryBtnKeys;
        cancelBtnKeys.selectOnDown = mItems[0].primaryBtnKeys;

        template.SetActive(false);

        keyPressGO.SetActive(false);
    }

    protected override void BindFinish(int actionIndex, int actionKeyIndex) {
        RefreshKeyLabels();

        keyPressGO.SetActive(false);

        UICamera.selectedObject = mLastSelectedObject;
        mLastSelectedObject = null;
    }

    void OnKeyClick(GameObject go) {
        int actionInd = -1;
        int keyInd = -1;

        for(int i = 0; i < mItems.Length; i++) {
            if(mItems[i].primary == go) {
                actionInd = i;
                keyInd = 0;
                break;
            }
            else if(mItems[i].secondary == go) {
                actionInd = i;
                keyInd = 1;
                break;
            }
        }

        if(actionInd != -1 && keyInd != -1) {
            keyPressGO.SetActive(true);
            keyPressActionLabel.text = mItems[actionInd].nameLabel.text;

            if(keyInd == 0)
                keyPressBindLabel.text = mItems[actionInd].primaryLabel.text;
            else if(keyInd == 1)
                keyPressBindLabel.text = mItems[actionInd].secondaryLabel.text;

            Bind(actionInd, keyInd);

            mLastSelectedObject = UICamera.selectedObject;
            UICamera.selectedObject = null;
        }
    }

    void OnSaveClick(GameObject go) {
        Save();
        UIModalManager.instance.ModalCloseTop();
    }

    void OnCancelClick(GameObject go) {
        UIModalManager uiMgr = UIModalManager.instance;

        if(isDirty) {
            if(uiMgr.ModalGetData(UIModalConfirm.modalName) != null) {
                UIModalConfirm.Open(cancelConfirmTitle, cancelConfirmDesc, delegate(bool yes) {
                    if(yes) {
                        Revert(false);
                        UIModalManager.instance.ModalCloseTop();
                    }
               });
            }
            else {
                Revert(false);
                UIModalManager.instance.ModalCloseTop();
            }
        }
        else
            UIModalManager.instance.ModalCloseTop();
    }

    void OnDefaultClick(GameObject go) {
        UIModalManager uiMgr = UIModalManager.instance;
        
        if(uiMgr.ModalGetData(UIModalConfirm.modalName) != null) {
            UIModalConfirm.Open(defaultConfirmTitle, defaultConfirmDesc, delegate(bool yes) {
                if(yes) {
                    Revert(true);
                    RefreshKeyLabels();
                }
            });
        }
        else {
            Revert(true);
            RefreshKeyLabels();
        }
    }
}
