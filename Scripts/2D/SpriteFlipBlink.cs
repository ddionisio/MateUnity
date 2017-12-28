using UnityEngine;
using System.Collections;

namespace M8 {
    [AddComponentMenu("M8/Sprite/FlipBlink")]
    public class SpriteFlipBlink : MonoBehaviour {
        public SpriteRenderer sprite;
        public bool useHorizontal;
        public bool useVertical;

        public float blinkDelay = 0.5f;
        public int blinkCount = 1;
        public float blinkWaitDelay = 0.0f;
        public bool blinkStartWait;
        
        private bool mStarted;

        void OnEnable() {
            if(mStarted) {
                StartCoroutine(DoBlink());
            }
        }

        void OnDisable() {
            if(mStarted) {
                StopAllCoroutines();
            }
        }

        void Awake() {
            if(sprite == null)
                sprite = GetComponent<SpriteRenderer>();
        }

        void Start() {
            mStarted = true;
            OnEnable();
        }

        IEnumerator DoBlink() {
            WaitForSeconds waitBlink = new WaitForSeconds(blinkDelay);
            WaitForSeconds waitBlinkRest = new WaitForSeconds(blinkWaitDelay);

            if(blinkStartWait)
                yield return waitBlinkRest;

            while(true) {
                for(int i = 0; i < blinkCount; i++) {
                    if(useHorizontal) sprite.flipX = !sprite.flipX;
                    if(useVertical) sprite.flipY = !sprite.flipY;

                    yield return waitBlink;

                    if(useHorizontal) sprite.flipX = !sprite.flipX;
                    if(useVertical) sprite.flipY = !sprite.flipY;

                    yield return waitBlink;
                }

                yield return waitBlinkRest;
            }
        }
    }
}