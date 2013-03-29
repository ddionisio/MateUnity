using UnityEngine;
using System.Collections;

public abstract class UIController : MonoBehaviour {
    public delegate void ActiveCallback(bool active);
    public delegate void Callback();

    public event ActiveCallback onActiveCallback;
    public event Callback onOpenCallback;
    public event Callback onCloseCallback;

    protected abstract void OnActive(bool active);

    protected abstract void OnOpen();

    protected abstract void OnClose();
	

    //don't call these, only uimodalmanager

	public void _active(bool active) {
        if(active) {
            StartCoroutine(DelayActivate());
        }
        else {
            StopCoroutine("DelayActivate");

            OnActive(false);

            if(onActiveCallback != null)
                onActiveCallback(false);
        }
	}
	
	public void _open() {
        OnOpen();

        if(onOpenCallback != null)
            onOpenCallback();
	}
	
	public void _close() {
        OnClose();

        if(onCloseCallback != null)
            onCloseCallback();
	}

    IEnumerator DelayActivate() {
        yield return new WaitForFixedUpdate();

        OnActive(true);

        if(onActiveCallback != null)
            onActiveCallback(true);

        yield break;
    }
}
