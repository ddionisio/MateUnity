using UnityEngine;
using System.Collections;

namespace M8 {
    [AddComponentMenu("M8/Game/Blinker Material Color")]
    public class BlinkerMaterialColor : Blinker {
        public Material material;
        public string modProperty = "_ColorOverlay";
        public Color color = Color.white;

        public Renderer[] targets; //manually select targets, leave empty to grab recursively

        private Material[] mTargetDefaultMats;

        private Material mMatInstance;
        private int mModID;

        void OnDestroy() {
            if(mMatInstance)
                DestroyImmediate(mMatInstance);
            mMatInstance = null;
        }

        void Awake() {
            mMatInstance = new Material(material);
            mModID = Shader.PropertyToID(modProperty);

            mMatInstance.SetColor(mModID, color);

            if(targets == null || targets.Length == 0)
                targets = GetComponentsInChildren<Renderer>(true);

            mTargetDefaultMats = new Material[targets.Length];

            for(int i = 0, max = targets.Length; i < max; i++) {
                mTargetDefaultMats[i] = targets[i].sharedMaterial;
            }
        }

        protected override void OnBlink(bool yes) {
            if(yes) {
                for(int i = 0, max = targets.Length; i < max; i++) {
                    targets[i].sharedMaterial = mMatInstance;
                }
            }
            else {
                for(int i = 0, max = targets.Length; i < max; i++) {
                    targets[i].sharedMaterial = mTargetDefaultMats[i];
                }
            }
        }
    }
}