using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    [AddComponentMenu("M8/Sprite/Random Group")]
    public class SpriteRandomGroup : MonoBehaviour {
        public SpriteRenderer[] spriteRenders;

        public Sprite[] sprites;

        public void Apply() {
            var spr = sprites[Random.Range(0, sprites.Length)];

            for(int i = 0; i < spriteRenders.Length; i++)
                spriteRenders[i].sprite = spr;
        }

        void OnEnable() {
            Apply();
        }

        void Awake() {
            if(spriteRenders == null || spriteRenders.Length == 0)
                spriteRenders = GetComponentsInChildren<SpriteRenderer>();
        }
    }
}