using UnityEngine;
using System.Collections;

namespace M8 {
    public interface IModalController {
        string id { get; }
        bool isExclusive { get; }
        MonoBehaviour behaviour { get; }

        void Init();
        void SetOwner(ModalManager mgr);
        void Push(GenericParams parms);
        void Pop();
        void Open();
        IEnumerator Opening();
        void Close();
        IEnumerator Closing();
        void SetShow(bool aShow);
        void SetActive(bool aActive);
    }

    /// <summary>
    /// This is when the modal is pushed to the stack, use this for initializing data
    /// </summary>
    public interface IModalPush {
        void Push(GenericParams parms);
    }

    /// <summary>
    /// This is when the modal is popped, use this to deinitialize data
    /// </summary>
    public interface IModalPop {
        void Pop();
    }

    /// <summary>
    /// Called when modal needs to show up, use this to initialize data, display, and preparation for Opening.
    /// </summary>
    public interface IModalOpen {
        void Open();
    }

    /// <summary>
    /// After Open, Opening is called, use this for animations
    /// </summary>
    public interface IModalOpening {
        IEnumerator Opening();
    }

    /// <summary>
    /// Called when modal is about to close, use this for animations
    /// </summary>
    public interface IModalClose {
        void Close();
    }

    /// <summary>
    /// Called when closing, use this for animations
    /// </summary>
    public interface IModalClosing {
        IEnumerator Closing();
    }

    /// <summary>
    /// Called whenever this controller becomes the top modal (active) or a new modal is pushed (inactive).  This is called after everything is opened/closed.
    /// Use this to set input focus if needed.
    /// </summary>
    public interface IModalActive {
        void SetActive(bool aActive);
    }
}