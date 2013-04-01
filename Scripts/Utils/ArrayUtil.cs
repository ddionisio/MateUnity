using UnityEngine;
using System;
using System.Collections;

namespace M8 {
    public struct ArrayUtil {
        public static void Shuffle(Array array) {
            for(int i = 0, max = array.Length; i < max; i++) {
                int r = UnityEngine.Random.Range(i, max);
                object obj = array.GetValue(i);
                object robj = array.GetValue(r);
                array.SetValue(robj, i);
                array.SetValue(obj, r);
            }
        }

        public static T[] RemoveAt<T>(T[] src, int index) {
            T[] dest = new T[src.Length - 1];
            if(index > 0) {
                Array.Copy(src, 0, dest, 0, index);
            }

            if(index < src.Length - 1) {
                Array.Copy(src, index + 1, dest, index, src.Length - index - 1);
            }
            
            return dest;
        }
	}
}
