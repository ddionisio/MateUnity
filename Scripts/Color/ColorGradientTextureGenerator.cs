using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    [CreateAssetMenu(fileName = "gradient", menuName = "M8/Color Gradient")]
    public class ColorGradientTextureGenerator : ScriptableObject {
        [SerializeField]
        Gradient _gradient = new Gradient();
        [SerializeField]
        int _textureWidth = 256;

        public Texture2D Generate() {
            var tex = new Texture2D(_textureWidth, 1);

            tex.wrapMode = TextureWrapMode.Clamp;

            for(int i = 0; i < _textureWidth; i++)
                tex.SetPixel(i, 0, _gradient.Evaluate((float)i / _textureWidth));

            tex.Apply();

            return tex;
        }
    }
}