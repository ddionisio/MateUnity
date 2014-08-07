using UnityEngine;

/// <summary>
/// Add this along UIController to enable/disable colliders during activate
/// </summary>
[AddComponentMenu("M8/UI/ModalActivateColliders")]
[RequireComponent(typeof(UIController))]
public class UIModalActivateColliders : MonoBehaviour {
    private UIController mCtrl;
    private Collider[] mColls;

    void Awake() {
        mColls = GetComponentsInChildren<Collider>(true);
        GetComponent<UIController>().onActiveCallback += OnActivate;
    }

    void OnActivate(bool yes) {
        for(int i = 0, max = mColls.Length; i < max; i++)
            mColls[i].enabled = yes;
    }
}
