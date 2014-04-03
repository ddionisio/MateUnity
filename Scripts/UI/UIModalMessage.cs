using UnityEngine;
using System.Collections;

[AddComponentMenu("")]
public abstract class UIModalMessage : UIController {
	public const string modalName = "message";
	
	public delegate void OnClick();

	private OnClick mCallback = null;

	public static void Open(string aTitle, string aText, OnClick aCallback) {
		UIModalManager uiMgr = UIModalManager.instance;
		
		if(uiMgr.ModalGetTop() == modalName)
			uiMgr.ModalCloseTop();
		
		UIModalManager.UIData dat = uiMgr.ModalGetData(modalName);
		
		if(dat != null) {
			UIModalMessage uiConfirm = dat.ui as UIModalMessage;

			uiConfirm.OnSetInfo(aTitle, aText);
			uiConfirm.mCallback = aCallback;
			
			uiMgr.ModalOpen(modalName);
		}
	}

	protected abstract void OnSetInfo(string aTitle, string aText);

	/// <summary>
	/// Call this when closing the message
	/// </summary>
	protected void Click() {
		OnClick toCall = mCallback;
		
		UIModalManager.instance.ModalCloseTop();
		
		if(toCall != null)
			toCall();
	}
}
