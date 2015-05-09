using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace M8 {
    [AddComponentMenu("M8/Renderer/Color Controller")]
    public class RendererColorController : MonoBehaviour {
        public string colorProperty = "_Color";

        public Color startColor = Color.white;
        public bool startColorApply;

        public bool recursive;

        private Renderer mRenderer;

        private int mColorId;
        private Color mColor;
        private Material[] mMats;

        public Color color {
            get { return mColor; }
            set {
                if(mColor != value) {
                    mColor = value;
#if UNITY_EDITOR
                    if(!Application.isPlaying)
                        return;
#endif
                    if(mMats == null) {
                        mColorId = Shader.PropertyToID(colorProperty);

                        List<Material> matList = new List<Material>();
                        if(recursive) {
                            Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
                            for(int i = 0; i < renderers.Length; i++) {
                                Material[] sms = renderers[i].sharedMaterials;
                                for(int j = 0; j < sms.Length; j++) {
                                    Material sm = sms[j];
                                    if(sm.HasProperty(mColorId)) {
                                        Material mat = matList.Find(m => m.name.CompareTo(sm.name) == 0);
                                        if(mat == null) {
                                            mat = new Material(sm);
                                            matList.Add(mat);
                                        }
                                        sms[j] = mat;
                                    }
                                }
                                renderers[i].sharedMaterials = sms;
                            }
                        }
                        else {
                            Material[] sms = mRenderer.sharedMaterials;
                            for(int j = 0; j < sms.Length; j++) {
                                Material sm = sms[j];
                                if(sm.HasProperty(mColorId)) {
                                    Material mat = matList.Find(m => m.name.CompareTo(sm.name) == 0);
                                    if(mat == null) {
                                        mat = new Material(sm);
                                        matList.Add(mat);
                                    }
                                    sms[j] = mat;
                                }
                            }
                            mRenderer.sharedMaterials = sms;
                        }

                        mMats = matList.ToArray();
                    }

                    for(int i = 0; i < mMats.Length; i++)
                        mMats[i].SetColor(mColorId, mColor);
                }
            }
        }

        void OnDestroy() {
            if(mMats != null) {
                for(int i = 0; i < mMats.Length; i++) {
                    if(mMats[i])
                        DestroyImmediate(mMats[i]);
                }
            }
        }

        void Awake() {
            mRenderer = GetComponent<Renderer>();
        }

        // Use this for initialization
        void Start() {
            if(startColorApply)
                color = startColor;
        }
    }
}