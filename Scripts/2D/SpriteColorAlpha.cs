using UnityEngine;
using System.Collections;

namespace M8 {
    /// <summary>
    /// Allow direct control of sprite's alpha, ignoring the rgb channels.
    /// </summary>
    [AddComponentMenu("M8/Sprite/ColorAlpha")]
    public class SpriteColorAlpha : MonoBehaviour {
        public SpriteRenderer sprite;

        public float alpha {
            get {
                if(!sprite)
                    sprite = GetComponent<SpriteRenderer>();

                return sprite ? sprite.color.a : 0f;
            }

            set {
                if(!sprite)
                    sprite = GetComponent<SpriteRenderer>();

                if(sprite) {
                    var clr = sprite.color;
                    clr.a = value;
                    sprite.color = clr;
                }
            }
        }
    }
}