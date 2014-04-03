using UnityEngine;
using System.Collections;

/*

    protected override void OnActive(bool active) {
        if(active) {
        }
        else {
        }
    }

    protected override void OnOpen() {
    }

    protected override void OnClose() {
    }
 */
public abstract class UIController : MonoBehaviour {
    public delegate void ActiveCallback(bool active);
    public delegate void Callback();

    public event ActiveCallback onActiveCallback;
    public event Callback onOpenCallback;
    public event Callback onCloseCallback;

    protected abstract void OnActive(bool active);

    protected abstract void OnOpen();

    protected abstract void OnClose();

    private bool mActive;


    //don't call these, only uimodalmanager

    public void _active(bool active) {
        if(mActive != active) {
            mActive = active;

            if(active) {
                OnActive(true);

                if(onActiveCallback != null)
                    onActiveCallback(true);
            }
            else {
                OnActive(false);

                if(onActiveCallback != null)
                    onActiveCallback(false);
            }
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
}
