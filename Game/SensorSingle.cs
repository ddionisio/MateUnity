using UnityEngine;
using System.Collections.Generic;

public abstract class SensorSingle<T> : MonoBehaviour where T : Component {
	public delegate void Callback(T unit);
	
	public event Callback enterCallback;
	public event Callback stayCallback;
	public event Callback exitCallback;
	
	protected abstract bool UnitVerify(T unit);
	
	void OnEnable() {
		if(collider != null)
			collider.enabled = true;
	}
	
	void OnDisable() {
		if(collider != null)
			collider.enabled = false;
	}
	
	void OnTriggerEnter(Collider other) {
		if(enterCallback != null) {
			T unit = other.GetComponent<T>();
			if(unit != null && UnitVerify(unit)) {
				enterCallback(unit);
			}
		}
	}
	
	void OnTriggerStay(Collider other) {
		if(stayCallback != null) {
			T unit = other.GetComponent<T>();
			if(unit != null && UnitVerify(unit)) {
				stayCallback(unit);
			}
		}
	}
	
	void OnTriggerExit(Collider other) {
		if(exitCallback != null) {
			T unit = other.GetComponent<T>();
			if(unit != null) {
				exitCallback(unit);
			}
		}
	}
	
	void OnDestroy() {
		enterCallback = null;
		stayCallback = null;
		exitCallback = null;
	}
}
