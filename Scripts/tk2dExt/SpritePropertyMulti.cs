using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/tk2D/SpritePropertyMulti")]
[ExecuteInEditMode]
public class SpritePropertyMulti : MonoBehaviour {
    public tk2dBaseSprite[] targets;
    public bool autoIncludeInactive = true;

    [SerializeField]
    Color startColor = Color.white;
    public bool applyStartColor;

    private Color mColor = Color.white;

    public Color color {
        get { return mColor; }
        set {
            mColor = value;

            tk2dBaseSprite[] items;
            if(targets == null || targets.Length == 0) {
                if(Application.isPlaying) {
                    items = targets = GetComponentsInChildren<tk2dBaseSprite>(autoIncludeInactive);
                }
                else {
                    items = GetComponentsInChildren<tk2dBaseSprite>(autoIncludeInactive);
                }
            }
            else
                items = targets;

            for(int i = 0; i < items.Length; i++) {
                if(items[i] && items[i].gameObject.activeSelf && items[i].enabled)
                    items[i].color = mColor;
            }
        }
    }

    void Awake() {
#if UNITY_EDITOR
        if(!Application.isPlaying)
            return;
#endif

        if(applyStartColor)
            color = startColor;
    }

#if UNITY_EDITOR
    void Update() {
        if(!Application.isPlaying) {
            if(applyStartColor) {
                color = startColor;
                applyStartColor = false;
            }
        }
    }
#endif
}