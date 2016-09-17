using UnityEngine;
using System.Collections;

namespace M8.UIModal.Interface {
    /// <summary>
    /// Called when modal needs to show up, use this to initialize data, display, and preparation for Opening.
    /// </summary>
    public interface IOpen {
        void Open();
    }

    /// <summary>
    /// After Open, Opening is called, use this for animations
    /// </summary>
    public interface IOpening {
        IEnumerator Opening();
    }
        
    /// <summary>
    /// Called when closing, use this for animations
    /// </summary>
    public interface IClosing {
        IEnumerator Closing();
    }

    /// <summary>
    /// Called when modal is now closed, use this to clean up if needed.
    /// </summary>
    public interface IClose {
        void Close();
    }

    /// <summary>
    /// Called whenever this controller becomes the top modal (active) or a new modal is pushed (inactive).  This is called after everything is opened/closed.
    /// Use this to set input focus if needed.
    /// </summary>
    public interface IActive {
        void SetActive(bool aActive);
    }
}