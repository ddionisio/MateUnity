using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace M8.UI.Graphics {
    /// <summary>
    /// Use this to apply color based on ColorPalette
    /// </summary>
    [ExecuteInEditMode]
    [AddComponentMenu("M8/UI/Graphics/ColorFromPalette")]
    public class ColorFromPalette : ColorFromPaletteBase {
        [SerializeField]
        Graphic _target;

        public Graphic target {
            get { return _target; }
            set {
                if(_target != value) {
                    _target = value;
                    ApplyColor();
                }
            }
        }

        public override void ApplyColor() {
            if(_target)
                _target.color = color;
        }

        void Awake() {
            if(!_target)
                _target = GetComponent<Graphic>();
        }
    }
}