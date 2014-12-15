using UnityEngine;
using System.Collections;

namespace M8.UI.Transforms {
    [AddComponentMenu("M8/UI/Transforms/PointAt")]
    public class PointAt : MonoBehaviour {
        public enum Dir {
            Up,
            Down,
            Left,
            Right
        }

        public RectTransform pointer;
        public Dir dir = Dir.Up;
        public bool ignorePOIActive = false;

        [SerializeField]
        private Transform _POI; //point of interest

        private Camera mPOICam;

        private RectTransform mRootCanvasRectTrans;


        public Transform POI {
            get { return _POI; }
            set {
                if(_POI != value) {
                    _POI = value;
                    ApplyPOI();
                }
            }
        }

        void Awake() {
            //grab root canvas
            Canvas rootCanvas = Util.GetRootCanvas(pointer);
            if(rootCanvas)
                mRootCanvasRectTrans = rootCanvas.transform as RectTransform;

            ApplyPOI();
        }

        void Update() {
            if(_POI != null && (ignorePOIActive || _POI.gameObject.activeInHierarchy)) {
                Vector3 vpPos = mPOICam.WorldToViewportPoint(_POI.position);

                bool isEdge = false;

                if(vpPos.x > 1) {
                    vpPos.x = 1; isEdge = true;
                }
                else if(vpPos.x < 0) {
                    vpPos.x = 0; isEdge = true;
                }

                if(vpPos.y > 1) {
                    vpPos.y = 1; isEdge = true;
                }
                else if(vpPos.y < 0) {
                    vpPos.y = 0; isEdge = true;
                }

                if(isEdge) {
                    if(!pointer.gameObject.activeSelf) {
                        pointer.gameObject.SetActive(true);
                    }

                    Vector2 sizeDelta = mRootCanvasRectTrans.sizeDelta;
                    pointer.anchoredPosition = new Vector2(((vpPos.x*sizeDelta.x) - (sizeDelta.x*0.5f)), ((vpPos.y*sizeDelta.y) - (sizeDelta.y*0.5f)));

                    switch(dir) {
                        case Dir.Up:
                            pointer.up = new Vector3(vpPos.x - 0.5f, vpPos.y - 0.5f, 0.0f);
                            break;
                        case Dir.Down:
                            pointer.up = -new Vector3(vpPos.x - 0.5f, vpPos.y - 0.5f, 0.0f);
                            break;
                        case Dir.Left:
                            pointer.right = -new Vector3(vpPos.x - 0.5f, vpPos.y - 0.5f, 0.0f);
                            break;
                        case Dir.Right:
                            pointer.right = new Vector3(vpPos.x - 0.5f, vpPos.y - 0.5f, 0.0f);
                            break;
                    }
                }
                else {
                    if(pointer.gameObject.activeSelf) {
                        pointer.gameObject.SetActive(false);
                    }
                }
            }
            else {
                pointer.gameObject.SetActive(false);
            }
        }

        void ApplyPOI() {
            if(_POI != null) {
                mPOICam = M8.Util.FindCameraForLayer(_POI.gameObject.layer);

                gameObject.SetActive(true);
            }
            else {
                mPOICam = null;

                gameObject.SetActive(false);

                pointer.gameObject.SetActive(false);
            }
        }
    }
}