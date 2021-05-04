using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace M8.UI.Texts {
    [AddComponentMenu("M8/UI/Texts/Typewriter")]
    public class TextTypewriter : MonoBehaviour {
        public Text label;
        public float delay;

        public bool autoPlay = true;
        public bool useRealTime;

        public string text {
            get {
                return label.text;
            }

            set {
                mString = value;

                if(autoPlay)
                    Play();
            }
        }

        public event System.Action proceedCallback;
        public event System.Action doneCallback;

        public bool isBusy { get { return mRout != null; } }

        private string mString;
        private System.Text.StringBuilder mStringBuff;
        private Coroutine mRout;

        public void Play() {
            if(mRout != null)
                StopCoroutine(mRout);

            if(gameObject.activeInHierarchy)
                mRout = StartCoroutine(DoType());
        }

        public void Skip() {
            if(mRout != null) {
                StopCoroutine(mRout);
                mRout = null;

                label.text = mString;

                if(doneCallback != null)
                    doneCallback();
            }
        }

        void OnEnable() {
            if(autoPlay)
                Play();
        }

        void OnDisable() {
            label.text = "";

            if(mRout != null) {
                StopCoroutine(mRout);
                mRout = null;
            }
        }

        void Awake() {
            label.text = "";
        }

        IEnumerator DoType() {
            var wait = useRealTime ? null : new WaitForSeconds(delay);

            label.text = "";

            if(mStringBuff == null || mStringBuff.Capacity < mString.Length)
                mStringBuff = new System.Text.StringBuilder(mString.Length);
            else
                mStringBuff.Remove(0, mStringBuff.Length);

            int count = mString.Length;
            for(int i = 0; i < count; i++) {
                if(useRealTime) {
                    var lastTime = Time.realtimeSinceStartup;
                    while(Time.realtimeSinceStartup - lastTime < delay)
                        yield return null;
                }
                else
                    yield return wait;

                if(mString[i] == '<') {
                    int endInd = -1;
                    bool foundEnd = false;
                    for(int j = i + 1; j < mString.Length; j++) {
                        if(mString[j] == '>') {
                            endInd = j;
                            if(foundEnd)
                                break;
                        }
                        else if(mString[j] == '/')
                            foundEnd = true;
                    }

                    if(endInd != -1 && foundEnd) {
                        mStringBuff.Append(mString, i, (endInd - i) + 1);
                        i = endInd;
                    }
                    else
                        mStringBuff.Append(mString[i]);
                }
                else
                    mStringBuff.Append(mString[i]);

                label.text = mStringBuff.ToString();

                if(proceedCallback != null)
                    proceedCallback();
            }

            mRout = null;

            if(doneCallback != null)
                doneCallback();
        }
    }
}