using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    [AddComponentMenu("M8/Sprite/Color Random")]
    public class SpriteColorRandom : MonoBehaviour {
        public SpriteRenderer spriteRender;

        public Color[] colors;

        public void Apply() {
            if(spriteRender == null)
                spriteRender = GetComponent<SpriteRenderer>();

            if(spriteRender)
                spriteRender.color = colors[Random.Range(0, colors.Length)];
        }

        void OnEnable() {
            Apply();
        }
    }
}