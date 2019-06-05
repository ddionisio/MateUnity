using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    [AddComponentMenu("M8/Renderer/Color From Palette")]
    public class RendererColorFromPalette : ColorFromPaletteBase {
        [Header("Material Data")]
        public string colorProperty = "_Color";

        private int mColorId;
        private Material[] mMats;

        public override void ApplyColor() {
            if(mMats == null) {
                mColorId = Shader.PropertyToID(colorProperty);

                var renderer = GetComponent<Renderer>();

                var matList = new List<Material>();

                var sms = renderer.sharedMaterials;
                for(int j = 0; j < sms.Length; j++) {
                    var sm = sms[j];
                    if(sm.HasProperty(mColorId)) {
                        var mat = matList.Find(m => m.name.CompareTo(sm.name) == 0);
                        if(mat == null) {
                            mat = new Material(sm);
                            matList.Add(mat);
                        }
                        sms[j] = mat;
                    }
                }

                renderer.sharedMaterials = sms;

                mMats = matList.ToArray();
            }

            for(int i = 0; i < mMats.Length; i++)
                mMats[i].SetColor(mColorId, color);
        }

        void OnDestroy() {
            if(mMats != null) {
                for(int i = 0; i < mMats.Length; i++) {
                    if(mMats[i])
                        DestroyImmediate(mMats[i]);
                }

                mMats = null;
            }
        }
    }
}