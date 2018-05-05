using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    [System.Serializable]
    public struct RangeFloat {
        public float min;
        public float max;

        public float length { get { return max - min; } }

        public float random { get { return Random.Range(min, max); } }
                
        public float Lerp(float t) {
            return Mathf.Lerp(min, max, t);
        }
    }

    [System.Serializable]
    public struct RangeInt {
        public int min;
        public int max;

        public int length { get { return max - min; } }

        public int random { get { return Random.Range(min, max + 1); } }

        public int Lerp(float t) {
            return Mathf.RoundToInt(Mathf.Lerp(min, max, t));
        }
    }
}