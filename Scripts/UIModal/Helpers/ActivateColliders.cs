using UnityEngine;

namespace M8.UIModal.Helpers {
    /// <summary>
    /// Add this along UIController to enable/disable colliders during activate
    /// </summary>
    [AddComponentMenu("M8/UI Modal/Controller/ActivateColliders")]
    [RequireComponent(typeof(Controller))]
    public class ActivateColliders : MonoBehaviour {
        private Controller mCtrl;
        private Collider[] mColls;

        void Awake() {
            mColls = GetComponentsInChildren<Collider>(true);
            GetComponent<Controller>().onActiveCallback += OnActivate;
        }

        void OnActivate(bool yes) {
            for(int i = 0, max = mColls.Length; i < max; i++)
                mColls[i].enabled = yes;
        }
    }
}