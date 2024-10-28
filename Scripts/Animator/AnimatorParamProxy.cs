using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
	[AddComponentMenu("M8/Animator Unity/ParamProxy")]
	public class AnimatorParamProxy : MonoBehaviour {
		[SerializeField]
		UnityEngine.Animator _target;

		[SerializeField]
		AnimatorControllerParameterType _paramType;

		[SerializeField]
		int _paramID;

		public UnityEngine.Animator target { get { return _target; } }

		public AnimatorControllerParameterType paramType { get { return _paramType; } }

		public int paramID { get { return _paramID; } }

		public bool SetParameter(string paramName) {
			if(_target) {
				var newID = UnityEngine.Animator.StringToHash(paramName);

				//find parameter, apply if valid
				for(int i = 0; i < _target.parameterCount; i++) {
					var parm = _target.GetParameter(i);
					if(parm.nameHash == _paramID) {
						_paramID = newID;
						_paramType = parm.type;
						return true;
					}
				}
			}

			return false;
		}

		public void SetTrigger() {
			target.SetTrigger(_paramID);
		}

		public void ResetTrigger() {
			target.ResetTrigger(_paramID);
		}

		public void SetBool(bool val) {
			target.SetBool(_paramID, val);
		}

		public bool GetBool() {
			return target.GetBool(_paramID);
		}

		public void SetFloat(float val) {
			target.SetFloat(_paramID, val);
		}

		public float GetFloat() {
			return target.GetFloat(_paramID);
		}

		public void SetInteger(int val) {
			target.SetInteger(_paramID, val);
		}

		public int GetInteger() {
			return target.GetInteger(_paramID);
		}

		void Awake() {
			if(!_target)
				_target = GetComponent<UnityEngine.Animator>();
		}
	}
}