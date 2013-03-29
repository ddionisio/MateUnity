using UnityEngine;
using System.Collections.Generic;

public abstract class Sensor<T> : MonoBehaviour where T : Component {
	protected abstract bool UnitVerify(T unit);
	protected abstract void UnitAdded(T unit);
	protected abstract void UnitRemoved(T unit);
	
	private HashSet<T> mUnits = new HashSet<T>();
	
	public HashSet<T> items {
		get {
			return mUnits;
		}
	}
	
	void OnEnable() {
		collider.enabled = true;
	}
	
	void OnDisable() {
		collider.enabled = false;
	}
			
	void OnTriggerEnter(Collider other) {
		CleanUp();
		T unit = other.GetComponent<T>();
		if(unit != null && UnitVerify(unit)) {
			if(mUnits.Add(unit)) {
				UnitAdded(unit);
			}
		}
	}
	
	void OnTriggerExit(Collider other) {
		CleanUp();
		T unit = other.GetComponent<T>();
		if(unit != null && mUnits.Remove(unit)) {
			UnitRemoved(unit);
		}
	}
	
	void CleanUp() {
		mUnits.RemoveWhere(delegate(T unit) {return unit == null || !unit.gameObject.activeInHierarchy;});
	}
}
