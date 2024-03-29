﻿using UnityEngine;
using System.Collections;

namespace M8 {
    /// <summary>
    /// Make sure this is on an object with a rigidbody!
    /// </summary>
    [ExecuteInEditMode]
    [AddComponentMenu("M8/Transform/AttachTo")]
    public class TransAttachTo : MonoBehaviour {
        public Transform target;
        public Vector3 offset;

        public Vector3 rotOfs;

        public bool ignoreRot;

#if !M8_PHYSICS_DISABLED
        private Collider mColl;
#endif

        private Quaternion mRotQ;

        void Awake() {
#if !M8_PHYSICS_DISABLED
            mColl = GetComponent<Collider>();
#endif

            mRotQ = Quaternion.Euler(rotOfs);
        }

        // Update is called once per frame
        void Update() {
            if(target != null) {
#if !M8_PHYSICS_DISABLED
                if(mColl != null) {
                    Vector3 ofs = transform.worldToLocalMatrix.MultiplyPoint(mColl.bounds.center);

                    transform.position = target.localToWorldMatrix.MultiplyPoint(offset - ofs);
                }
                else 
#endif
                {
                    transform.position = target.position + target.rotation * offset;
                }

                if(!ignoreRot) {
#if UNITY_EDITOR
                    if(!Application.isPlaying)
                        mRotQ = Quaternion.Euler(rotOfs);
#endif

                    transform.rotation = mRotQ * target.rotation;
                }
            }
        }
    }
}