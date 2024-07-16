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

        private static int mUVOfsId = Shader.PropertyToID("_UVOfs");
        private static int mAngleId = Shader.PropertyToID("_Angle");
        private static int mAngleMinId = Shader.PropertyToID("_AngleMin");
        private static int mAngleMaxId = Shader.PropertyToID("_AngleMax");

        private SpriteRenderer mSprRender;
        private Material mMatInst;

        private Vector2 mUVOfs;

        private bool mIsInit;

        public void Refresh() {
            if(mIsInit) {
                mMatInst.SetFloat(mAngleId, _angle);
                mMatInst.SetFloat(mAngleMinId, _angleMin);
                mMatInst.SetFloat(mAngleMaxId, _angleMax);

                if(!Application.isPlaying) //ensure uv offset is correct when not running for display purpose
					GenerateUVOfs();

				mMatInst.SetVector(mUVOfsId, mUVOfs);
			}
            else
                Awake();
        }

        void OnEnable() {
            if(!mIsInit)
                Awake();
            else
                Refresh();
		}

        void Awake() {
            if(_clipAngleMaterial) {
                mSprRender = GetComponent<SpriteRenderer>();

                mMatInst = new Material(_clipAngleMaterial);

                mSprRender.sharedMaterial = mMatInst;

                mMatInst.SetFloat(mAngleId, _angle);
                mMatInst.SetFloat(mAngleMinId, _angleMin);
                mMatInst.SetFloat(mAngleMaxId, _angleMax);

                GenerateUVOfs();

				mMatInst.SetVector(mUVOfsId, mUVOfs);

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

        void OnDidApplyAnimationProperties() {
            Refresh();
		}

		private void GenerateUVOfs() {
			if(mSprRender.sprite) {
				Vector2 uvMin = Vector2.one, uvMax = Vector2.zero;

				var uvs = mSprRender.sprite.uv;
				for(int i = 0; i < uvs.Length; i++) {
					if(uvs[i].x < uvMin.x)
						uvMin.x = uvs[i].x;
					if(uvs[i].y < uvMin.y)
						uvMin.y = uvs[i].y;

					if(uvs[i].x > uvMax.x)
						uvMax.x = uvs[i].x;
					if(uvs[i].y > uvMax.y)
						uvMax.y = uvs[i].y;
				}

				mUVOfs = new Vector2(0.5f, 0.5f) - (uvMax + uvMin) * 0.5f;
			}
			else
				mUVOfs = Vector2.zero;
		}
    }
}