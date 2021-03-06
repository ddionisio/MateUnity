﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    [AddComponentMenu("M8/Sprite/Random")]
    public class SpriteRandom : MonoBehaviour {
        public SpriteRenderer spriteRender;

        public Sprite[] sprites;

        public void Apply() {
            if(spriteRender == null)
                spriteRender = GetComponent<SpriteRenderer>();

            if(spriteRender)
                spriteRender.sprite = sprites[Random.Range(0, sprites.Length)];
        }

        void OnEnable() {
            Apply();
        }
    }
}