using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace M8 {
	[AddComponentMenu("M8/Renderer/Material Pulse Property Block")]
	public class RendererMaterialPulsePropertyBlock : MonoBehaviour {
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
		public float _pulsePerSecond;
		[SerializeField]
		public bool _isRealTime;

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
			get { return Vector4.Lerp(_valueVectorStart, _valueVectorEnd, mTime); }
		}

		public ValueType valueType { get { return _valueType; } }

		private MaterialPropertyBlock mMatPropBlock;
		private bool mIsPropIDInit;
		private int mPropID;

		private float mCurPulseTime = 0;
		private float mLastTime;

		private float mTime;

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

		void OnEnable() {			
			mLastTime = _isRealTime ? Time.realtimeSinceStartup : Time.time;

			mCurPulseTime = 0f;
			mTime = 0f;
		}

		void Update() {
			var time = _isRealTime ? Time.realtimeSinceStartup : Time.time;
			var delta = time - mLastTime;
			mLastTime = time;

			mCurPulseTime += delta;

			mTime = Mathf.Sin(Mathf.PI * mCurPulseTime * _pulsePerSecond);
			mTime *= mTime;

			Apply();
		}
	}
}