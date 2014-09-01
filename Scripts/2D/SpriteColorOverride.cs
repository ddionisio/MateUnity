using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/Sprite/ColorOverride")]
public class SpriteColorOverride : MonoBehaviour {
    [SerializeField]
    SpriteRenderer[] _targets;

    [SerializeField]
    bool _recursive = false;

    [SerializeField]
    bool _includeInactive = false;

    [SerializeField]
    Color _color = Color.white;

    private SpriteRenderer[] mSpriteRenders;

    public Color color {
        get { return _color; }
        set {
            if(_color != value) {
                _color = value;
                for(int i = 0; i < mSpriteRenders.Length; i++) {
                    if(mSpriteRenders[i]) mSpriteRenders[i].color = _color;
                }
            }
        }
    }

    void Awake() {
        if(_targets == null || _targets.Length == 0) {
            if(_recursive) {
                mSpriteRenders = GetComponentsInChildren<SpriteRenderer>(_includeInactive);
            }
            else {
                mSpriteRenders = new SpriteRenderer[1];
                mSpriteRenders[0] = GetComponent<SpriteRenderer>();
            }
        }
        else
            mSpriteRenders = _targets;

        color = _color;
    }
}
