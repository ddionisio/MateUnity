using UnityEngine;
using System.Collections;

namespace M8 {
    [AddComponentMenu("M8/Renderer/ColorAndTextureRandom")]
    [RequireComponent(typeof(Renderer))]
    public class RendererColorAndTextureRandom : MonoBehaviour {
        public string colorVar = "_Color";
        public string textureVar = "_MainTex";

        public Texture[] frames;

        private int mColorId;
        private int mTextureId;

        public float start; //pause at start in seconds
        public float pulse = 1.0f; //time it takes for a pulse to complete
        public float pause; //pause between pulse

        //random delay offsets
        public float startRandomOfs;
        public float pulseRandomOfs;
        public float pauseRandomOfs;

        public Color startColor = Color.clear;
        public Color[] endColors = { Color.white };

        public bool squared;

        private bool mStarted = false;
        private Material mMat;
        private int mCurInd;

        void OnEnable() {
            if(mStarted) {
                StartCoroutine(DoPulseUpdate());
            }
        }

        void OnDisable() {
            if(mStarted)
                StopAllCoroutines();
        }

        void Awake() {
            mMat = GetComponent<Renderer>().material;
            mColorId = Shader.PropertyToID(colorVar);
            mTextureId = Shader.PropertyToID(textureVar);
        }

        void Start() {
            mStarted = true;
            StartCoroutine(DoPulseUpdate());
        }

        IEnumerator DoPulseUpdate() {
            WaitForFixedUpdate wait = new WaitForFixedUpdate();

            mMat.SetColor(mColorId, startColor);

            mCurInd = 0;

            float sDelay = start + Random.value * startRandomOfs;
            if(sDelay > 0.0f)
                yield return new WaitForSeconds(sDelay);
            else
                yield return wait;

            float t = 0.0f;
            float curDelay = pulse + Random.value * pulseRandomOfs;
            Color curEndColor = endColors.Length > 0 ? endColors[Random.Range(0, endColors.Length)] : startColor;

            while(true) {
                t += Time.fixedDeltaTime;

                if(t >= curDelay) {
                    if(frames.Length > 0) {
                        mMat.SetTexture(mTextureId, frames[mCurInd]);
                        mCurInd++; if(mCurInd == frames.Length) { mCurInd = 0; M8.ArrayUtil.Shuffle(frames); }
                    }

                    mMat.SetColor(mColorId, startColor);

                    t = 0.0f;
                    curDelay = pulse + Random.value * pulseRandomOfs;

                    if(endColors.Length > 0)
                        curEndColor = endColors[Random.Range(0, endColors.Length)];

                    float pdelay = pause + Random.value * pauseRandomOfs;
                    if(pdelay > 0.0f)
                        yield return new WaitForSeconds(pdelay);
                }
                else {
                    float s = Mathf.Sin(Mathf.PI * (t / pulse));

                    if(endColors.Length > 0)
                        mMat.SetColor(mColorId, Color.Lerp(startColor, curEndColor, squared ? s * s : s));
                }

                yield return wait;
            }
        }
    }
}