using UnityEngine;
using UnityEngine.Events;

namespace M8 {
#if !M8_PHYSICS2D_DISABLED
    [AddComponentMenu("M8/Physics2D/Collider Exit Event")]
    public class ColliderExitUnityEvent2D : MonoBehaviour {
        [Tooltip("Which tags is allowed to invoke callback. Set this to empty to allow any collision.")]
        [TagSelector]
        public string[] tagFilters;

        public UnityEventCollision2D callback;

        void OnCollisionExit2D(Collision2D collision) {
            if(tagFilters.Length > 0) {
                var go = collision.gameObject;

                int filterInd = -1;
                for(int i = 0; i < tagFilters.Length; i++) {
                    var tagFilter = tagFilters[i];
                    if(!string.IsNullOrEmpty(tagFilter) && go.CompareTag(tagFilter)) {
                        filterInd = i;
                        break;
                    }
                }

                if(filterInd != -1)
                    callback.Invoke(collision);
            }
            else
                callback.Invoke(collision);
        }
    }
#endif
}