using UnityEngine;
using System.Collections;

namespace M8 {
    [AddComponentMenu("M8/Sprite/FrameBlink")]
    public class SpriteFrameBlink : MonoBehaviour {
        public SpriteRenderer sprite;
        public Sprite frame;

        public float blinkDelay = 0.5f;
        public int blinkCount = 1;
        public float blinkWaitDelay = 0.0f;
        public bool blinkStartWait;

        private Sprite mFrameDefault;

        private bool mStarted;

        void OnEnable() {
            if(mStarted) {
                StartCoroutine(DoBlink());
            }
        }

        void OnDisable() {
            if(mStarted) {
                if(sprite)
                    sprite.sprite = mFrameDefault;

                StopAllCoroutines();
            }
        }

        void Awake() {
            if(sprite == null)
                sprite = GetComponent<SpriteRenderer>();

            mFrameDefault = sprite.sprite;
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
                    sprite.sprite = frame;
                    yield return waitBlink;
                    sprite.sprite = mFrameDefault;
                    yield return waitBlink;
                }

                yield return waitBlinkRest;
            }
        }
    }
}