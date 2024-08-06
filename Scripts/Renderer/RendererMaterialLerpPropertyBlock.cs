using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    [AddComponentMenu("M8/Renderer/Material Lerp Property Block")]
    [ExecuteInEditMode]
    public class RendererMaterialLerpPropertyBlock : MonoBehaviour {
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
		Vector4 _valueVectorStart;
		[SerializeField]
		Vector4 _valueVectorEnd;

		[SerializeField]
		float _time; //[0, 1]

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

		public Vector4 valueVectorCurrent {
			get { return Vector4.Lerp(_valueVectorStart, _valueVectorEnd, _time); }
		}

		public float time {
			get { return _time; }
			set {
				var t = Mathf.Clamp01(value);
				if(_time != t) {
					_time = t;
					Apply();
				}
			}
		}

		public ValueType valueType { get { return _valueType; } }

		private MaterialPropertyBlock mMatPropBlock;
		private bool mIsPropIDInit;
		private int mPropID;

		public void ResetValue() {
			if(!_target) return;

			var mat = _target.sharedMaterial;

			if(!mat) return;

			switch(_valueType) {
				case ValueType.Color:
					_valueVectorEnd = _valueVectorStart = mat.GetColor(propertyID);
					break;
				case ValueType.Vector:
					_valueVectorEnd = _valueVectorStart = mat.GetVector(propertyID);
					break;
				case ValueType.Float:
				case ValueType.Range:
					_valueVectorEnd.x = _valueVectorStart.x = mat.GetFloat(propertyID);
					break;
				case ValueType.Int:
					_valueVectorEnd.x = _valueVectorStart.x = mat.GetInteger(propertyID);
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

			var val = valueVectorCurrent;

			switch(_valueType) {
				case ValueType.Color:
					mMatPropBlock.SetColor(propertyID, val);
					break;
				case ValueType.Vector:
					mMatPropBlock.SetVector(propertyID, val);
					break;
				case ValueType.Float:
				case ValueType.Range:
					mMatPropBlock.SetFloat(propertyID, val.x);
					break;
				case ValueType.Int:
					mMatPropBlock.SetInteger(propertyID, Mathf.RoundToInt(val.x));
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