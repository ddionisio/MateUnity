using UnityEngine;
using System.Collections;

namespace M8 {
    [AddComponentMenu("M8/Transform/LookAt")]
    [ExecuteInEditMode]
    public class TransLookAt : MonoBehaviour {
        [M8.TagSelector]
        public string targetTag; //if target is null
        public bool targetUseMainCamera = true; //if target is null
        public Transform target;

        public bool visibleCheck = true; //if true, only compute if source's renderer is visible
        public bool backwards;

        public Transform source; //if null, use this transform

        private Renderer mRenderer;

        private Transform GetTarget() {
            if(target)
                return target;

            if(targetUseMainCamera) {
                var cam = Camera.main;
                if(cam)
                    return cam.transform;
            }

            if(!string.IsNullOrEmpty(targetTag)) {
                var go = GameObject.FindGameObjectWithTag(targetTag);
                if(go)
                    return go.transform;
            }

            return null;
        }

        void Awake() {
            if(source == null)
                source = transform;

            if(Application.isPlaying) {
                if(target == null)
                    target = GetTarget();

                if(visibleCheck) {
                    mRenderer = source.GetComponent<Renderer>();
                    if(mRenderer == null)
                        Debug.LogError("No Renderer found.");
                }
            }
        }

        void Update() {
            bool isVisible;
            Transform tgt, src = source;

            if(Application.isPlaying) {
                tgt = target;

                isVisible = tgt != null && source != null && (!visibleCheck || mRenderer == null || mRenderer.isVisible);
            }
            else {
                tgt = GetTarget();

                isVisible = tgt != null && source != null;
            }

            if(isVisible)
                src.rotation = Quaternion.LookRotation(backwards ? tgt.forward : -tgt.forward, tgt.up);
        }
    }
}