using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    public abstract class ColorFromPaletteBase : MonoBehaviour {
        [HideInInspector]
        [SerializeField]
        ColorPalette _palette;
        [HideInInspector]
        [SerializeField]
        int _index;
        [HideInInspector]
        [SerializeField]
        float _brightness = 1f;
        [HideInInspector]
        [SerializeField]
        float _alpha = 1f;

        public ColorPalette palette {
            get { return _palette; }
            set {
                if(_palette != value) {
                    if(_palette)
                        _palette.updateCallback -= OnColorChanged;

                    _palette = value;

                    if(_palette) {
                        ApplyColor();

                        _palette.updateCallback += OnColorChanged;
                    }
                }
            }
        }

        public int index {
            get { return _index; }
            set {
                if(_index != value) {
                    _index = value;
                    ApplyColor();
                }
            }
        }

        public float brightness {
            get { return _brightness; }
            set {
                if(_brightness != value) {
                    _brightness = value;
                    ApplyColor();
                }
            }
        }

        public float alpha {
            get { return _alpha; }
            set {
                if(_alpha != value) {
                    _alpha = value;
                    ApplyColor();
                }
            }
        }

        public Color color {
            get {
                if(_palette) {
                    var clr = _palette.GetColor(_index);
                    return new Color(clr.r * _brightness, clr.g * _brightness, clr.b * _brightness, clr.a * _alpha);
                }
                else
                    return Color.white;
            }
        }

        public abstract void ApplyColor();

        protected virtual void OnDisable() {
            if(_palette)
                _palette.updateCallback -= OnColorChanged;
        }

        protected virtual void OnEnable() {
            if(_palette) {
                ApplyColor();
                _palette.updateCallback += OnColorChanged;
            }
        }

        void OnColorChanged(int aIndex) {
            if(_index == aIndex)
                ApplyColor();
        }
    }
}