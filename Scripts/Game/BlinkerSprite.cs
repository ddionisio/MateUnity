using UnityEngine;
using System.Collections;

namespace M8 {
    [AddComponentMenu("M8/Game/Blinker Sprite")]
    public class BlinkerSprite : Blinker {
        public SpriteRenderer[] targets; //manually select targets, leave empty to grab recursively

        void Awake() {
            if(targets == null || targets.Length == 0) {
                targets = GetComponentsInChildren<SpriteRenderer>(true);
            }
        }

        protected override void OnBlink(bool yes) {
            if(yes) {
                for(int i = 0; i < targets.Length; i++) {
                    if(targets[i]) {
                        Color c = targets[i].color;
                        c.a = 0.0f;
                        targets[i].color = c;
                    }
                }
            }
            else {
                for(int i = 0; i < targets.Length; i++) {
                    if(targets[i]) {
                        Color c = targets[i].color;
                        c.a = 1.0f;
                        targets[i].color = c;
                    }
                }
            }
        }
    }
}