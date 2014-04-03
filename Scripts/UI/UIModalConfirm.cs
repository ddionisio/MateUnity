using UnityEngine;
using System.Collections;

[AddComponentMenu("")]
public abstract class UIModalConfirm : UIController {
	public const string modalName = "confirm";
	
	public delegate void OnConfirm(bool yes);

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
			uiConfirm.OnSetInfo(aTitle, aText);
			uiConfirm.mCallback = aCallback;
			
			uiMgr.ModalOpen(modalName);
		}
	}

	protected abstract void OnSetInfo(string aTitle, string aText);
	
	/// <summary>
	/// Call this when closing the message
	/// </summary>
	protected void Click(bool yes) {
		OnConfirm toCall = mCallback;
		
		UIModalManager.instance.ModalCloseTop();
		
		if(toCall != null)
			toCall(yes);
	}
}
