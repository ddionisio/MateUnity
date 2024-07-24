using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
	[AddComponentMenu("M8/Renderer/Material Set Property Block Proxy")]
	[ExecuteInEditMode]
	public class RendererMaterialSetPropertyBlockProxy : MonoBehaviour {
		public enum ValueType {
			None = -1,

			Color,
			Vector,
			Float,
			Range,
			TexEnv,
			Int
		}

		[SerializeField]
		Renderer _target;

		[SerializeField]
		string _name;

		[SerializeField]
		ValueType _valueType;
		[SerializeField]
		Vector4 _valueVector;
		[SerializeField]
		Texture _valueTexture;

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
					Apply();
				}
			}
		}

		public ValueType valueType { get { return _valueType; } }

		private MaterialPropertyBlock mMatPropBlock;
		private bool mIsPropIDInit;
		private int mPropID;

		public void SetColor(Color val) {
			_valueVector = val;
			Apply();
		}

		public void SetVector(Vector4 val) {
			_valueVector = val;
			Apply();
		}

		public void SetFloat(float val) {
			_valueVector.x = val;
			Apply();
		}

		public void SetTexture(Texture tex) {
			_valueTexture = tex;
			Apply();
		}

		public void SetInt(int val) {
			_valueVector.x = val;
			Apply();
		}

		public void ResetValue() {
			if(!_target) return;

			var mat = _target.sharedMaterial;

			if(!mat) return;

			switch(_valueType) {
				case ValueType.Color:
					_valueVector = mat.GetColor(propertyID);
					break;
				case ValueType.Vector:
					_valueVector = mat.GetVector(propertyID);
					break;
				case ValueType.Float:
				case ValueType.Range:
					_valueVector.x = mat.GetFloat(propertyID);
					break;
				case ValueType.TexEnv:
					_valueTexture = mat.GetTexture(propertyID);
					break;
				case ValueType.Int:
					_valueVector.x = mat.GetInteger(propertyID);
					break;
			}

			Apply();
		}

		public void Apply() {
			if(!_target || _valueType == ValueType.None) return;

			if(mMatPropBlock == null)
				mMatPropBlock = new MaterialPropertyBlock();

			_target.GetPropertyBlock(mMatPropBlock);

			if(!Application.isPlaying)
				mIsPropIDInit = false;

			switch(_valueType) {
				case ValueType.Color:
					mMatPropBlock.SetColor(propertyID, _valueVector);
					break;
				case ValueType.Vector:
					mMatPropBlock.SetVector(propertyID, _valueVector);
					break;
				case ValueType.Float:
				case ValueType.Range:
					mMatPropBlock.SetFloat(propertyID, _valueVector.x);
					break;
				case ValueType.TexEnv:
					if(_valueTexture)
						mMatPropBlock.SetTexture(propertyID, _valueTexture);
					break;
				case ValueType.Int:
					mMatPropBlock.SetInteger(propertyID, (int)_valueVector.x);
					break;
			}

			_target.SetPropertyBlock(mMatPropBlock);
		}

		void OnDidApplyAnimationProperties() {
			Apply();
		}

		void OnEnable() {
			Apply();
		}
	}
}