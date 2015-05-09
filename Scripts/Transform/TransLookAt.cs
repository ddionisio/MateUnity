using UnityEngine;
using System.Collections;

namespace M8 {
    [AddComponentMenu("M8/Transform/LookAt")]
    public class TransLookAt : MonoBehaviour {
        public string targetTag = "MainCamera"; //if target is null
        public Transform target;

        public bool visibleCheck = true; //if true, only compute if source's renderer is visible

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
            if(!visibleCheck || mRenderer.isVisible) {
                source.rotation = Quaternion.LookRotation(-target.forward, target.up);
            }
        }
    }
}