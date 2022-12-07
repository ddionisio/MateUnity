using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    [AddComponentMenu("M8/Sprite/FrameRandom")]
    public class SpriteFrameRandom : MonoBehaviour {
        public SpriteRenderer sprite;

        public Sprite[] frames;

        public float startDelay; //pause at start in seconds
        public float nextDelayMin = 0.4f; //time it takes to go to next frame
        public float nextDelayMax = 1.0f; //time it takes to go to next frame

        public bool playOnStart;
        public bool useShuffle;

        private bool mActive;
        private Coroutine mRout;

        public void Play() {
            mActive = true;

            if(useShuffle)
                ArrayUtil.Shuffle(frames);

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
            //fail-safe
            if(frames.Length == 0)
                yield break;

            if(startDelay > 0f)
                yield return new WaitForSeconds(startDelay);

            if(nextDelayMin == nextDelayMax) {
                if(useShuffle) {
                    while(true) {
                        for(int i = 0; i < frames.Length; i++) {
                            sprite.sprite = frames[i];
                            yield return new WaitForSeconds(nextDelayMin);
                        }

                        ArrayUtil.Shuffle(frames);
                    }
                }
                else {
                    while(true) {
                        sprite.sprite = frames[Random.Range(0, frames.Length)];
                        yield return new WaitForSeconds(nextDelayMin);
                    }
                }
            }
            else {
                if(useShuffle) {
                    while(true) {
                        for(int i = 0; i < frames.Length; i++) {
                            sprite.sprite = frames[i];
                            yield return new WaitForSeconds(Random.Range(nextDelayMin, nextDelayMax));
                        }

                        ArrayUtil.Shuffle(frames);
                    }
                }
                else {
                    while(true) {
                        sprite.sprite = frames[Random.Range(0, frames.Length)];
                        yield return new WaitForSeconds(Random.Range(nextDelayMin, nextDelayMax));
                    }
                }
            }
        }
    }
}