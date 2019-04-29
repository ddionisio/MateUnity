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

        public void Invoke(int index) {
            updateCallback?.Invoke(index);
        }

        public Color GetColor(int index) {
            if(index < 0 || index >= _colors.Length)
                return Color.white;

            return _colors[index];
        }

        public void SetColor(int index, Color color) {
            _colors[index] = color;

            Invoke(index);
        }
    }
}