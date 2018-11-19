using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace M8.UI.Graphics {
    [AddComponentMenu("M8/UI/Graphics/ColorMatch")]
    [ExecuteInEditMode]
    public class ColorMatch : MonoBehaviour {
        public Graphic source;
        public Graphic target;
        public Color colorMod = Color.white;

        private Color mCurColor;

        void OnEnable() {
            if(source != null || target != null) {
                mCurColor = source.color;
                target.color = mCurColor * colorMod;
            }
        }

        void Update() {
            if(source == null || target == null)
                return;

            if(mCurColor != source.color) {
                mCurColor = source.color;
                target.color = mCurColor * colorMod;
            }
        }
    }
}