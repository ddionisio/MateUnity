using UnityEngine;
using System.Collections;

namespace M8 {
    [AddComponentMenu("M8/Transform/LookAt")]
    [ExecuteInEditMode]
    public class TransLookAt : MonoBehaviour {
        public string targetTag = "MainCamera"; //if target is null
        public Transform target;

        public bool visibleCheck = true; //if true, only compute if source's renderer is visible
        public bool backwards;

        public Transform source; //if null, use this transform

        private Renderer mRenderer;

        void Awake() {
            if(target == null) {
                GameObject go = GameObject.FindGameObjectWithTag(targetTag);
                if(go != null)
                    target = go.transform;
            }

            if(source == null)
                source = transform;

            if(visibleCheck) {
                mRenderer = source.GetComponent<Renderer>();
                if(mRenderer == null)
                    Debug.LogError("No Renderer found.");
            }
        }

        void Update() {
            bool isVisible = !visibleCheck || mRenderer == null || mRenderer.isVisible;
            Transform tgt = target;
            Transform src = source;

#if UNITY_EDITOR
            if(!Application.isPlaying) {
                if(tgt == null) {
                    GameObject go = GameObject.FindGameObjectWithTag(targetTag);
                    if(go != null)
                        tgt = go.transform;
                }

                if(src == null)
                    src = transform;

                isVisible = isVisible && tgt != null && src != null;
            }
#endif

            if(isVisible) {
                src.rotation = Quaternion.LookRotation(backwards ? tgt.forward : -tgt.forward, tgt.up);
            }
        }
    }
}