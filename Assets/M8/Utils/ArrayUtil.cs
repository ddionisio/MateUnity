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
	}
}
