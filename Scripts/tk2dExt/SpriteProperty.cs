using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/tk2D/SpriteProperty")]
[ExecuteInEditMode]
public class SpriteProperty : MonoBehaviour {

    private tk2dBaseSprite mSprite;
    //private int mSpriteId;

    public string spriteName {
        get {
            return mSprite.GetCurrentSpriteDef().name;
        }

        set {
            mSprite.SetSprite(value);
            //mSpriteId = mSprite.spriteId;
        }
    }

    /// <summary>
    /// Positive scale, preserves actual scale sign
    /// </summary>
    public Vector2 absoluteScale {
        get {
            Vector3 s = mSprite.scale;
            return new Vector2(Mathf.Abs(s.x), Mathf.Abs(s.y));
        }

        set {
            Vector3 s = mSprite.scale;
            s.x = Mathf.Sign(s.x) * value.x;
            s.y = Mathf.Sign(s.y) * value.y;
            mSprite.scale = s;
        }
    }

    void Awake() {
        mSprite = GetComponent<tk2dBaseSprite>();
        //mSpriteId = mSprite.spriteId;
    }
}
