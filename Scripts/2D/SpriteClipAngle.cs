using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    [AddComponentMenu("M8/Sprite/Clip Angle")]
    [RequireComponent(typeof(SpriteRenderer))]
    [ExecuteInEditMode]
    public class SpriteClipAngle : MonoBehaviour {
        [SerializeField]
        [HideInInspector]
        Material _clipAngleMaterial;

        [SerializeField]
        float _angle;

        [SerializeField]
        float _angleMin;

        [SerializeField]
        float _angleMax;

        public float angle {
            get { return _angle; }
            set {
                if(_angle != value) {
                    _angle = value;

                    if(mIsInit)
                        mMatInst.SetFloat(mAngleId, _angle);
                    else
                        Awake();
                }
            }
        }

        public float angleMin {
            get { return _angleMin; }
            set {
                if(_angleMin != value) {
                    _angleMin = value;

                    if(mIsInit)
                        mMatInst.SetFloat(mAngleMinId, _angleMin);
                    else
                        Awake();
                }
            }
        }

        public float angleMax {
            get { return _angleMax; }
            set {
                if(_angleMax != value) {
                    _angleMax = value;

                    if(mIsInit)
                        mMatInst.SetFloat(mAngleMaxId, _angleMax);
                    else
                        Awake();
                }
            }
        }

        private static int mAngleId = Shader.PropertyToID("_Angle");
        private static int mAngleMinId = Shader.PropertyToID("_AngleMin");
        private static int mAngleMaxId = Shader.PropertyToID("_AngleMax");

        private SpriteRenderer mSprRender;
        private Material mMatInst;
        private bool mIsInit;

        public void Refresh() {
            if(mIsInit) {
                mMatInst.SetFloat(mAngleId, _angle);
                mMatInst.SetFloat(mAngleMinId, _angleMin);
                mMatInst.SetFloat(mAngleMaxId, _angleMax);
            }
            else
                Awake();
        }

        void OnEnable() {
            if(!mIsInit)
                Awake();
        }

        void Awake() {
            if(_clipAngleMaterial) {
                mSprRender = GetComponent<SpriteRenderer>();

                mMatInst = new Material(_clipAngleMaterial);

                mSprRender.sharedMaterial = mMatInst;

                mMatInst.SetFloat(mAngleId, _angle);
                mMatInst.SetFloat(mAngleMinId, _angleMin);
                mMatInst.SetFloat(mAngleMaxId, _angleMax);

                mIsInit = true;
            }
        }

        void OnDestroy() {
            if(mMatInst) {
                if(Application.isPlaying)
                    Destroy(mMatInst);
                else
                    DestroyImmediate(mMatInst);

                mMatInst = null;
            }
        }
    }
}