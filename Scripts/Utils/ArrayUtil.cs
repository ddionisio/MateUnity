using UnityEngine;
using System;
using System.Collections;

namespace M8 {
    public struct ArrayUtil {
        public static void Fill<T>(T[] src, T data) {
            for(int i = 0, max = src.Length; i < max; i++) {
                src[i] = data;
            }
        }

        public static T[][] NewDoubleArray<T>(int numRow, int numCol) {
            T[][] ret = new T[numRow][];
            for(int i = 0; i < numRow; i++) {
                ret[i] = new T[numCol];
            }

            return ret;
        }

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

        public static void RemoveAt<T>(ref T[] src, int index) {
            T[] dest = new T[src.Length - 1];
            if(index > 0) {
                Array.Copy(src, 0, dest, 0, index);
            }

            if(index < src.Length - 1) {
                Array.Copy(src, index + 1, dest, index, src.Length - index - 1);
            }

            src = dest;
        }

        public static void InsertAfter<T>(ref T[] src, int index, T val) {
            if(index == src.Length - 1) {
                System.Array.Resize(ref src, src.Length + 1);
                src[src.Length - 1] = val;
            }
            else {
                T[] dest = new T[src.Length + 1];

                Array.Copy(src, dest, index+1);

                dest[index + 1] = val;

                Array.Copy(src, index + 1, dest, index + 2, src.Length - (index + 1));

                src = dest;
            }
        }
	}
}
