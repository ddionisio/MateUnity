using UnityEngine;
using System.Collections;

namespace M8.UIModal.Interface {
    /// <summary>
    /// This is when the modal is pushed to the stack, use this for initializing data
    /// </summary>
    public interface IPush {
        void Push(GenericParams parms);
    }

    /// <summary>
    /// This is when the modal is popped, use this to deinitialize data
    /// </summary>
    public interface IPop {
        void Pop();
    }

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
    /// Called when modal is about to close, use this for animations
    /// </summary>
    public interface IClose {
        void Close();
    }
        
    /// <summary>
    /// Called when closing, use this for animations
    /// </summary>
    public interface IClosing {
        IEnumerator Closing();
    }
        
    /// <summary>
    /// Called whenever this controller becomes the top modal (active) or a new modal is pushed (inactive).  This is called after everything is opened/closed.
    /// Use this to set input focus if needed.
    /// </summary>
    public interface IActive {
        void SetActive(bool aActive);
    }
}