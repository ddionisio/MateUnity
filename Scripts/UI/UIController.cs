using UnityEngine;
using System.Collections;

[AddComponentMenu("")]
public abstract class UIController : MonoBehaviour {
    public delegate void ActiveCallback(bool active);
    public delegate void Callback();

    public event ActiveCallback onActiveCallback;

    protected virtual void OnActive(bool active) { }

    /// <summary>
    /// Called when modal needs to show up
    /// </summary>
    public virtual void Open() { }

    /// <summary>
    /// After Open, this is continually called until it returns false.  Optional override for when opening (e.g. animation)
    /// </summary>
    public virtual bool Opening() { return false; }

    /// <summary>
    /// Called when modal needs to hide
    /// </summary>
    public virtual void Close() { }

    /// <summary>
    /// After Close, this is continually called until it returns false.  Optional override for when closing (e.g. animation)
    /// </summary>
    public virtual bool Closing() { return false; }

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
}
