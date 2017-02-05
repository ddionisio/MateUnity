using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    [AddComponentMenu("M8/Sprite/FrameSequence")]
    public class SpriteFrameSequence : MonoBehaviour {
        public enum Mode {
            Loop,
            SeeSaw
        }
                
        public SpriteRenderer sprite;

        public Mode mode;

        public Sprite[] frames;

        public float startDelay; //pause at start in seconds
        public float nextDelay;

        public bool playOnStart;

        private bool mActive;
        private Coroutine mRout;

        public void Play() {
            mActive = true;
            if(mRout == null)
                mRout = StartCoroutine(DoPlay());
        }

        public void Stop() {
            mActive = false;

            if(mRout != null) {
                StopCoroutine(mRout);
                mRout = null;
            }
        }

        void OnEnable() {
            if(mActive)
                Play();
        }

        void OnDisable() {
            if(mRout != null) {
                StopCoroutine(mRout);
                mRout = null;
            }
        }

        void Awake() {
            if(sprite == null) {
                sprite = GetComponent<SpriteRenderer>();
            }
        }

        void Start() {
            if(playOnStart)
                Play();
        }

        IEnumerator DoPlay() {
            if(startDelay > 0f)
                yield return new WaitForSeconds(startDelay);

            var wait = new WaitForSeconds(nextDelay);

            int mCurInd = 0;

            while(true) {
                for(mCurInd = 0; mCurInd < frames.Length; mCurInd++) {
                    sprite.sprite = frames[mCurInd];
                    yield return wait;
                }

                switch(mode) {
                    case Mode.SeeSaw:
                        //sequence backwards
                        for(mCurInd = frames.Length - 2; mCurInd > 0; mCurInd--) {
                            sprite.sprite = frames[mCurInd];
                            yield return wait;
                        }
                        break;

                    default:
                        break;
                }
            }
        }
    }
}