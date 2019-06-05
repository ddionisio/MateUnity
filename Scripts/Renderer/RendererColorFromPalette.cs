using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    [ExecuteInEditMode]
    [AddComponentMenu("M8/Renderer/Color From Palette")]
    public class RendererColorFromPalette : ColorFromPaletteBase {
        [Header("Material Data")]
        public string colorProperty = "_Color";
        public Material material;

        private int mColorId;
        private Material mMat;

        public override void ApplyColor() {
#if UNITY_EDITOR
            if(!Application.isPlaying) {
                //refresh
                if(mMat) {
                    DestroyImmediate(mMat);
                    mMat = null;
                }
            }
#endif
            if(mMat == null && material) {
                mColorId = Shader.PropertyToID(colorProperty);

                var renderer = GetComponent<Renderer>();
                if(renderer) {
                    mMat = new Material(material);
                    renderer.material = mMat;
                }
            }

            if(mMat)
                mMat.SetColor(mColorId, color);
        }

        void OnDestroy() {
            if(mMat)
                DestroyImmediate(mMat);
        }

        void Awake() {
#if UNITY_EDITOR
            if(!Application.isPlaying) {
                var renderer = GetComponent<Renderer>();
                if(renderer) {
                    renderer.sharedMaterial = material;
                }
            }
#endif
        }
    }
}