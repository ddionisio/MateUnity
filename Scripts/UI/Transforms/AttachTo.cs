using UnityEngine;
using System.Collections;

namespace M8.UI.Transforms {
    [AddComponentMenu("M8/UI/Transforms/AttachTo")]
    [RequireComponent(typeof(RectTransform))]
    public class AttachTo : MonoBehaviour {
        [SerializeField]
        private Transform _target;

        private Camera mTargetCam;

        private RectTransform mRootCanvasRectTrans;

        private RectTransform mRectTrans;

        public Transform target {
            get {
                return _target;
            }

            set {
                if(_target != value) {
                    _target = value;
                    ApplyTargetData();
                }
            }
        }

        void Awake() {
            ApplyTargetData();

            //grab root canvas
            Canvas rootCanvas = Util.GetRootCanvas(transform);
            if(rootCanvas)
                mRootCanvasRectTrans = rootCanvas.transform as RectTransform;

            mRectTrans = transform as RectTransform;
        }

        void LateUpdate() {
            if(_target && mRootCanvasRectTrans) {
                Vector2 sizeDelta = mRootCanvasRectTrans.sizeDelta;
                Vector2 vpPos = mTargetCam.WorldToViewportPoint(_target.position);
                mRectTrans.anchoredPosition = new Vector2(((vpPos.x*sizeDelta.x) - (sizeDelta.x*0.5f)), ((vpPos.y*sizeDelta.y) - (sizeDelta.y*0.5f)));
            }
        }

        private void ApplyTargetData() {
            if(_target != null)
                mTargetCam = M8.Util.FindCameraForLayer(_target.gameObject.layer);
        }
    }
}