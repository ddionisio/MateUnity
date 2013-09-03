using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/tk2D/SpriteProperty")]
[ExecuteInEditMode]
public class SpriteProperty : MonoBehaviour {

    private tk2dBaseSprite mSprite;
    private int mSpriteId;

    public string spriteName {
        get {
            return mSprite.GetCurrentSpriteDef().name;
        }

        set {
            mSprite.SetSprite(value);
            mSpriteId = mSprite.spriteId;
        }
    }

    void Awake() {
        mSprite = GetComponent<tk2dBaseSprite>();
        mSpriteId = mSprite.spriteId;
    }
}
