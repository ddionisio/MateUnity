using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/UI/ModalConfirm")]
public class UIModalConfirm : UIController {
    public const string modalName = "confirm";

    public delegate void OnConfirm(bool yes);

    public UILabel title;
    public UILabel text;

    public UIEventListener yes;
    public UIEventListener no;

    private OnConfirm mCallback = null;

    public static void Open(OnConfirm aCallback) {
        Open(null, null, aCallback);
    }

    public static void Open(string aTitle, string aText, OnConfirm aCallback) {
        UIModalManager uiMgr = UIModalManager.instance;

        if(uiMgr.ModalGetTop() == modalName)
            uiMgr.ModalCloseTop();

        UIModalManager.UIData dat = uiMgr.ModalGetData(modalName);

        if(dat != null) {
            UIModalConfirm uiConfirm = dat.ui as UIModalConfirm;
            if(aTitle != null) uiConfirm.title.text = aTitle;
            if(aText != null) uiConfirm.text.text = aText;
            uiConfirm.mCallback = aCallback;

            uiMgr.ModalOpen(modalName);
        }
    }

    protected override void OnActive(bool active) {
        if(active) {
            yes.onClick = YesClick;
            no.onClick = NoClick;
        }
        else {
            yes.onClick = null;
            no.onClick = null;
        }
    }

    protected override void OnOpen() {
        M8.NGUIExtUtil.LayoutRefresh(transform);
    }

    protected override void OnClose() {
        mCallback = null;
    }

    void YesClick(GameObject go) {
        OnConfirm toCall = mCallback;

        UIModalManager.instance.ModalCloseTop();

        if(toCall != null)
            toCall(true);
    }

    void NoClick(GameObject go) {
        OnConfirm toCall = mCallback;

        UIModalManager.instance.ModalCloseTop();

        if(toCall != null)
            toCall(false);
    }
}
