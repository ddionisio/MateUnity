using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    [ExecuteInEditMode]
    [AddComponentMenu("M8/Sprite/ColorFromPalette")]
    public class SpriteColorFromPalette : ColorFromPaletteBase {
        [SerializeField]
        SpriteRenderer _target; //set null to grab from self.

        public SpriteRenderer target {
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
                _target = GetComponent<SpriteRenderer>();
        }
    }
}