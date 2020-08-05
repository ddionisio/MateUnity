using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    [CreateAssetMenu(fileName = "gradient", menuName = "M8/Color Gradient")]
    public class ColorGradientTextureGenerator : ScriptableObject {
        public enum Mode {
            Gradient,
            Curve,
            Steps,
        }

        public Texture texture { get { return _textureTarget; } }

        [SerializeField]
        Mode _mode = Mode.Gradient;

        //For curve and step
        [SerializeField]
        bool _mono = false; //if true, texture is saved with only one channel

        [SerializeField]
        Color _colorBegin = Color.black;
        [SerializeField]
        Color _colorEnd = Color.white;

        //Gradient
        [SerializeField]
        Gradient _gradient = new Gradient();

        //Curve
        [SerializeField]
        AnimationCurve _curve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

        //Steps
        [SerializeField]
        int _steps = 4;
                
        [SerializeField]
        int _textureWidth = 256;
        [SerializeField]
        FilterMode _textureFilterMode = FilterMode.Bilinear;
        [SerializeField]
        TextureWrapMode _textureWrapMode = TextureWrapMode.Clamp;

        [SerializeField]
        Texture2D _textureTarget = null;

        public Texture2D Generate() {
            int textureWidth = _mode == Mode.Steps ? _steps + 1 : _textureWidth; //texture width for steps is based on count
            TextureFormat textureFormat = _mode != Mode.Gradient && _mono ? TextureFormat.R8 : TextureFormat.RGBA32; //gradient is strictly 4 channels

            var tex = new Texture2D(textureWidth, 1, textureFormat, false);

            tex.filterMode = _textureFilterMode;
            tex.wrapMode = _textureWrapMode;

            switch(_mode) {
                case Mode.Gradient:
                    for(int i = 0; i < _textureWidth; i++)
                        tex.SetPixel(i, 0, _gradient.Evaluate((float)i / _textureWidth));
                    break;

                case Mode.Curve:
                    for(int i = 0; i < _textureWidth; i++) {
                        float t = Mathf.Clamp01(_curve.Evaluate((float)i / textureWidth));

                        Color clr;

                        if(_mono)
                            clr = new Color(t, t, t);
                        else
                            clr = Color.Lerp(_colorBegin, _colorEnd, t);

                        tex.SetPixel(i, 0, clr);
                    }
                    break;

                case Mode.Steps:
                    for(int i = 0; i < textureWidth; i++) {
                        float t = (float)i / textureWidth;

                        Color clr;

                        if(_mono)
                            clr = new Color(t, t, t);
                        else
                            clr = Color.Lerp(_colorBegin, _colorEnd, t);

                        tex.SetPixel(i, 0, clr);
                    }
                    break;
            }

            tex.Apply();

            return tex;
        }
    }
}