using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("M8/Renderer/Color Controller")]
public class RendererColorController : MonoBehaviour {
    public string colorProperty = "_Color";

    public Color startColor = Color.white;

    public bool recursive;

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
                            for(int j = 0; j < renderers[i].sharedMaterials.Length; j++) {
                                Material sm = renderers[i].sharedMaterials[j];
                                if(sm.HasProperty(mColorId)) {
                                    Material mat = matList.Find(m => m.name.CompareTo(sm.name) == 0);
                                    if(mat == null) {
                                        mat = new Material(sm);
                                        matList.Add(mat);
                                    }
                                    renderers[i].sharedMaterials[j] = mat;
                                }
                            }
                        }
                    }
                    else {
                        for(int j = 0; j < renderer.sharedMaterials.Length; j++) {
                            Material sm = renderer.sharedMaterials[j];
                            if(sm.HasProperty(mColorId)) {
                                Material mat = matList.Find(m => m.name.CompareTo(sm.name) == 0);
                                if(mat == null) {
                                    mat = new Material(sm);
                                    matList.Add(mat);
                                }
                                renderer.sharedMaterials[j] = mat;
                            }
                        }
                    }

                    mMats = matList.ToArray();
                }

                for(int i = 0; i < mMats.Length; i++)
                    mMats[i].SetColor(mColorId, mColor);
            }
        }
    }

    // Use this for initialization
    void Start() {
        color = startColor;
    }
}
