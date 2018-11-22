using UnityEngine;
using System.Collections;

namespace M8 {
    /// <summary>
    /// Automatically set the tile X/Y of target material based on the target object's scale X/Y
    /// </summary>
    [AddComponentMenu("M8/Renderer/AutoTileScale")]
    [RequireComponent(typeof(Renderer))]
    public class RendererAutoTileScale : MonoBehaviour {
        public float pixelPerUnit = 32.0f;
                
        [SerializeField]
        bool _flipX = false;

        [SerializeField]
        bool _flipY = false;

        [SerializeField]
        bool _applyX = true;

        [SerializeField]
        bool _applyY = true;

        public bool flipX { get { return _flipX; } set { _flipX = value; } }
        public bool flipY { get { return _flipY; } set { _flipY = value; } }
        public bool applyX { get { return _applyX; } set { _applyX = value; } }
        public bool applyY { get { return _applyY; } set { _applyY = value; } }

        private Transform mTrans = null;
        private Material mMat = null;
        private Vector2 mCurScale = Vector2.zero;

        void OnEnable() {
            mCurScale = Vector2.zero;
        }


        void Awake() {
            mMat = GetComponent<Renderer>().material;
            mTrans = transform;
        }

        // Update is called once per frame
        void Update() {
            if(mMat == null || mMat.mainTexture == null) return;

            bool apply = false;
            Vector2 s = mTrans.localScale;
            Vector2 newTextureScale = mMat.mainTextureScale;
            if(_applyX && mCurScale.x != s.x) {
                newTextureScale.x = (_flipX ? -s.x : s.x) * (pixelPerUnit / mMat.mainTexture.width);
                mCurScale.x = s.x;
                apply = true;
            }
            if(_applyY && mCurScale.y != s.y) {
                newTextureScale.y = (_flipY ? -s.y : s.y) * (pixelPerUnit / mMat.mainTexture.height);
                mCurScale.y = s.y;
                apply = true;
            }

            if(apply)
                mMat.mainTextureScale = newTextureScale;
        }
    }
}