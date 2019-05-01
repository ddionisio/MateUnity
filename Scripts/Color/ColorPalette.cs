using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    [CreateAssetMenu(fileName = "colorPalette", menuName = "M8/Color Palette")]
    public class ColorPalette : ScriptableObject {
        [SerializeField]
        Color[] _colors = null;

        public int count { get { return _colors != null ? _colors.Length : 0; } }

        public event System.Action<int> updateCallback; //called whenever a color has changed

        private Color[] mColorBuffer; //filled up when setting color

        public void Invoke(int index) {
            updateCallback?.Invoke(index);
        }

        public void RevertColors() {
            if(mColorBuffer != null) {
                for(int i = 0; i < mColorBuffer.Length; i++)
                    mColorBuffer[i] = _colors[i];
            }
        }

        public Color GetColor(int index) {
            if(mColorBuffer == null || mColorBuffer.Length != _colors.Length)
                GenerateColorBuffer();

            if(index < 0 || index >= mColorBuffer.Length)
                return Color.white;

            return mColorBuffer[index];
        }

        public void SetColor(int index, Color color) {
            if(mColorBuffer == null || mColorBuffer.Length != _colors.Length)
                GenerateColorBuffer();

            if(index < 0 || index >= mColorBuffer.Length)
                return;

            mColorBuffer[index] = color;

            Invoke(index);
        }

        public void RevertColor(int index) {
            if(mColorBuffer != null) {
                if(index < 0 || index >= mColorBuffer.Length)
                    return;

                mColorBuffer[index] = _colors[index];
            }
        }

        private void GenerateColorBuffer() {
            mColorBuffer = new Color[_colors.Length];
            System.Array.Copy(_colors, mColorBuffer, _colors.Length);
        }
    }
}