using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    [AddComponentMenu("M8/Transform/ScaleOfsRand")]
    public class TransScaleOfsRand : MonoBehaviour {
        public Transform target;

        public Vector3 ofsMin;
        public Vector3 ofsMax;

        public RangeFloat ofsUniform;

        private Vector3? mDefaultScale;

        public void Apply() {
            if(!target)
                target = transform;

            if(!mDefaultScale.HasValue)
                mDefaultScale = target.localScale;

            var _ofsUniform = ofsUniform.random;

            target.localScale = mDefaultScale.Value + new Vector3(
                _ofsUniform + Random.Range(ofsMin.x, ofsMax.x),
                _ofsUniform + Random.Range(ofsMin.y, ofsMax.y),
                _ofsUniform + Random.Range(ofsMin.z, ofsMax.z));
        }

        void OnEnable() {
            Apply();
        }
    }
}