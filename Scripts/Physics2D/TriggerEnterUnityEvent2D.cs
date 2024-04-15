using UnityEngine;
using UnityEngine.Events;

namespace M8 {
#if !M8_PHYSICS2D_DISABLED
    [AddComponentMenu("M8/Physics2D/Trigger Enter Event")]
    public class TriggerEnterUnityEvent2D : MonoBehaviour {
        [Tooltip("Which tags is allowed to invoke callback. Set this to empty to allow any collision.")]
        [TagSelector]
        public string[] tagFilters;

        public UnityEventCollider2D callback;

        void OnTriggerEnter2D(Collider2D collision) {
            if(tagFilters.Length > 0) {
                int filterInd = -1;
                for(int i = 0; i < tagFilters.Length; i++) {
                    var tagFilter = tagFilters[i];
                    if(!string.IsNullOrEmpty(tagFilter) && collision.CompareTag(tagFilter)) {
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