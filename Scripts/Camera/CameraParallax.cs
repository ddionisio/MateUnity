using UnityEngine;
using System.Collections;

namespace M8 {
    [AddComponentMenu("M8/Camera/Parallax")]
    [ExecuteInEditMode]
    public class CameraParallax : MonoBehaviour {

        public Transform view;

        public Vector2 bound = new Vector2(5, 2);
        public Vector2 moveScale = new Vector2(0.1f, 0.1f);

        private Vector2 mLastViewPos;

        void OnEnable() {
            if(view != null) {
                mLastViewPos = view.transform.position;
            }
        }

        // Update is called once per frame
        void Update() {
            if(view != null) {
                Vector2 viewPos = view.position;
                if(viewPos != mLastViewPos) {
                    Vector3 ppos = transform.localPosition;
                    Vector2 lookDelta = viewPos - mLastViewPos;
                    ppos.x = Mathf.Clamp(ppos.x + lookDelta.x * moveScale.x, -bound.x, bound.x);
                    ppos.y = Mathf.Clamp(ppos.y + lookDelta.y * moveScale.y, -bound.y, bound.y);
                    transform.localPosition = ppos;

                    mLastViewPos = viewPos;
                }
            }
        }
    }
}