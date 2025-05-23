using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    [AddComponentMenu("M8/Renderer/Material Set Property Block Color From Palette")]
    [ExecuteInEditMode]
    public class RendererMaterialSetPropertyBlockColorFromPalette : ColorFromPaletteBase {
		[SerializeField]
		Renderer _target;

		[SerializeField]
		string _name;

		public Renderer target { get { return _target; } }

		public int propertyID {
			get {
				if(!mIsPropIDInit)
					mPropID = Shader.PropertyToID(_name);
				return mPropID;
			}
		}

		public string propertyName {
			get { return _name; }
			set {
				if(_name != value) {
					_name = value;
					mIsPropIDInit = false;
					ApplyColor();
				}
			}
		}

		private MaterialPropertyBlock mMatPropBlock;
		private bool mIsPropIDInit;
		private int mPropID;

		public override void ApplyColor() {
			if(!_target) return;

			if(mMatPropBlock == null)
				mMatPropBlock = new MaterialPropertyBlock();

			_target.GetPropertyBlock(mMatPropBlock);

			if(!Application.isPlaying)
				mIsPropIDInit = false;

			mMatPropBlock.SetColor(propertyID, color);

			_target.SetPropertyBlock(mMatPropBlock);
		}
	}
}