using UnityEngine;
using System.Collections;

//general helper functions

namespace M8 {
    public struct Util {
        /// <summary>
        /// Get the component from parent of given t, if inclusive is true, try to find the component on t first.
        /// </summary>
        public static T GetComponentUpwards<T>(Transform t, bool inclusive) where T : Component {
            T ret = null;

            Transform parent = inclusive ? t : t.parent;
            while(parent != null) {
                ret = parent.GetComponent<T>();
                if(ret != null)
                    break;

                parent = parent.parent;
            }

            return ret;
        }

        public static bool IsParentOf(Transform child, Transform parent) {
            bool ret = false;

            Transform p = child;
            while(p != null) {
                p = p.parent;
                if(p == parent) {
                    ret = true;
                    break;
                }
            }

            return ret;
        }
    }
}