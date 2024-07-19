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
		int _propID;

		[SerializeField]
		ValueType _valueType;
		[SerializeField]
		Vector4 _valueVector;
		[SerializeField]
		Texture _valueTexture;

		public Renderer target { get { return _target; } }

		public int propertyID { get { return _propID; } }

		public ValueType valueType { get { return _valueType; } }

		private MaterialPropertyBlock mMatPropBlock;

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
					_valueVector = mat.GetColor(_propID);
					break;
				case ValueType.Vector:
					_valueVector = mat.GetVector(_propID);
					break;
				case ValueType.Float:
				case ValueType.Range:
					_valueVector.x = mat.GetFloat(_propID);
					break;
				case ValueType.TexEnv:
					_valueTexture = mat.GetTexture(_propID);
					break;
				case ValueType.Int:
					_valueVector.x = mat.GetInteger(_propID);
					break;
			}

			Apply();
		}

		public void Apply() {
			if(!_target || _valueType == ValueType.None) return;

			if(mMatPropBlock == null)
				mMatPropBlock = new MaterialPropertyBlock();

			_target.GetPropertyBlock(mMatPropBlock);

			switch(_valueType) {
				case ValueType.Color:
					mMatPropBlock.SetColor(_propID, _valueVector);
					break;
				case ValueType.Vector:
					mMatPropBlock.SetVector(_propID, _valueVector);
					break;
				case ValueType.Float:
				case ValueType.Range:
					mMatPropBlock.SetFloat(_propID, _valueVector.x);
					break;
				case ValueType.TexEnv:
					if(_valueTexture)
						mMatPropBlock.SetTexture(_propID, _valueTexture);
					break;
				case ValueType.Int:
					mMatPropBlock.SetInteger(_propID, (int)_valueVector.x);
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