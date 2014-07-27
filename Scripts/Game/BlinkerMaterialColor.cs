using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("M8/Game/Blinker Material Color")]
public class BlinkerMaterialColor : Blinker {
    public string modProperty = "_ColorOverlay";
    public Color color = Color.white;

    public Renderer[] targets; //manually select targets, leave empty to grab recursively

    private Renderer[] mRenderers;
    private Material[] mBlinkMats;
    private Material[] mBlinkDefaultMats;

    private int mModID;

    void OnDestroy() {
        if(mBlinkMats != null) {
            for(int i = 0, max = mBlinkMats.Length; i < max; i++) {
                if(mBlinkMats[i])
                    DestroyImmediate(mBlinkMats[i]);
            }
        }
    }

    void Awake() {
        mModID = Shader.PropertyToID(modProperty);

        Renderer[] renders = targets == null || targets.Length == 0 ? GetComponentsInChildren<Renderer>(true) : targets;
        if(renders.Length > 0) {
            List<Renderer> validRenders = new List<Renderer>(renders.Length);
            foreach(Renderer r in renders) {
                if(r.sharedMaterial.HasProperty(mModID)) {
                    validRenders.Add(r);
                }
            }

            mRenderers = new Renderer[validRenders.Count];
            mBlinkMats = new Material[validRenders.Count];
            mBlinkDefaultMats = new Material[validRenders.Count];

            for(int i = 0, max = mBlinkMats.Length; i < max; i++) {
                mRenderers[i] = validRenders[i];

                mBlinkMats[i] = new Material(mBlinkDefaultMats[i] = validRenders[i].sharedMaterial);
                mBlinkMats[i].SetColor(mModID, color);
            }
        }
    }

    protected override void OnBlink(bool on) {
        if(on) {
            for(int i = 0, max = mBlinkMats.Length; i < max; i++) {
                mRenderers[i].sharedMaterial = mBlinkMats[i];
            }
        }
        else {
            for(int i = 0, max = mBlinkMats.Length; i < max; i++) {
                mRenderers[i].sharedMaterial = mBlinkDefaultMats[i];
            }
        }
    }
}
