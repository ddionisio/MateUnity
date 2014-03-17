using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/UI/ModalMessage")]
public class UIModalMessage : UIController {
    public const string modalName = "message";
    
    public delegate void OnClick();
    
    public UILabel title;
    public UILabel text;
    
    public UIEventListener ok;
    
    private OnClick mCallback = null;

    public static void Open(string aTitle, string aText, OnClick aCallback) {
        UIModalManager uiMgr = UIModalManager.instance;
        
        if(uiMgr.ModalGetTop() == modalName)
            uiMgr.ModalCloseTop();
        
        UIModalManager.UIData dat = uiMgr.ModalGetData(modalName);
        
        if(dat != null) {
            UIModalMessage uiConfirm = dat.ui as UIModalMessage;
            if(aTitle != null) uiConfirm.title.text = aTitle;
            if(aText != null) uiConfirm.text.text = aText;
            uiConfirm.mCallback = aCallback;
            
            uiMgr.ModalOpen(modalName);
        }
    }
    
    protected override void OnActive(bool active) {
        if(active) {
            ok.onClick = OKClick;
        }
        else {
            ok.onClick = null;
        }
    }
    
    protected override void OnOpen() {
        NGUILayoutBase.RefreshNow(transform);
    }
    
    protected override void OnClose() {
        mCallback = null;
    }
    
    void OKClick(GameObject go) {
        OnClick toCall = mCallback;
        
        UIModalManager.instance.ModalCloseTop();
        
        if(toCall != null)
            toCall();
    }
}
