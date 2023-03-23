using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    /// <summary>
    /// Set the particle's start color to a palette
    /// </summary>
    [ExecuteInEditMode]
    [AddComponentMenu("M8/Particle/ColorFromPalette")]
    public class ParticleColorFromPalette : ColorFromPaletteBase {
        [SerializeField]
        ParticleSystem _target; //set null to grab from self.

        public ParticleSystem target {
            get { return _target; }
            set {
                if(_target != value) {
                    _target = value;
                    ApplyColor();
                }
            }
        }

        public override void ApplyColor() {
            if(_target) {
                var dat = _target.main;
                dat.startColor = color;
            }
        }

        void Awake() {
            if(!_target)
                _target = GetComponent<ParticleSystem>();
        }
    }
}