using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//general helper functions

namespace M8 {
    public struct Util {
        static int ComponentNameCompare<T>(T obj1, T obj2) where T : Component {
            return obj1.name.CompareTo(obj2.name);
        }

        public static T[] GetComponentsInChildrenAlphaSort<T>(Component c, bool includeInactive) where T : Component {
            T[] items = c.GetComponentsInChildren<T>(includeInactive);
            System.Array.Sort<T>(items, ComponentNameCompare);

            return items;
        }

        public static T[] GetComponentsInChildrenAlphaSort<T>(GameObject go, bool includeInactive) where T : Component {
            T[] items = go.GetComponentsInChildren<T>(includeInactive);
            System.Array.Sort<T>(items, ComponentNameCompare);

            return items;
        }

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

        public static void SetPhysicsLayerRecursive(Transform t, int layer) {
            t.gameObject.layer = layer;

            foreach(Transform child in t) {
                SetPhysicsLayerRecursive(child, layer);
            }
        }

        /// <summary>
        /// Get the layer the given transform resides in, 0 = root, ie. no parent
        /// </summary>
        public static int GetNodeLayer(Transform t) {
            int ret = 0;

            Transform p = t.parent;
            while(p != null) {
                ret++;
                p = p.parent;
            }

            return ret;
        }

        public static void ShuffleList<T>(List<T> list) {
            for(int i = 0, max = list.Count; i < max; i++) {
                int r = UnityEngine.Random.Range(i, max);
                T obj = list[i];
                T robj = list[r];
                list[i] = robj;
                list[r] = obj;
            }
        }

        public static bool CheckLayerAndTag(GameObject go, int layerMask, string tag) {
            return (layerMask & (1 << go.layer)) != 0 && go.tag == tag;
        }

        public static ushort MakeWord(byte hb, byte lb) {
            return (ushort)(((hb & 0xff) << 8) | (lb & 0xff));
        }

        public static uint MakeLong(ushort hs, ushort ls) {
            return (uint)(((hs & 0xffff) << 16) | (ls & 0xffff));
        }

        public static uint MakeLong(byte b1, byte b2, byte b3, byte b4) {
            return MakeLong(MakeWord(b1, b2), MakeWord(b3, b4));
        }

        public static ushort GetHiWord(uint i) {
            return (ushort)(i >> 16);
        }

        public static ushort GetLoWord(uint s) {
            return (ushort)(s & 0xffff);
        }

        public static byte GetHiByte(ushort s) {
            return (byte)(s >> 8);
        }

        public static byte GetLoByte(ushort s) {
            return (byte)(s & 0xff);
        }
    }
}