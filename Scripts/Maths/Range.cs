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

        public float Clamp(float v) {
            return Mathf.Clamp(v, min, max);
        }

        /// <summary>
        /// Returns [0, 1] based on given value
        /// </summary>
        public float GetT(float v) {
            return Mathf.Clamp01((v - min) / length);
        }

        public bool InRange(float v) {
            return v >= min && v <= max;
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

        public int Clamp(int v) {
            return Mathf.Clamp(v, min, max);
        }

        /// <summary>
        /// Returns [0, 1] based on given value
        /// </summary>
        public float GetT(int v) {
            float fMin = min;
            float fMax = max;

            return Mathf.Clamp01((v - fMin) / (fMax - fMin));
        }
    }
}