using UnityEngine;
using System.Collections.Generic;

public abstract class SensorCheckSphere<T> : MonoBehaviour where T : Component {
	public delegate void OnUnitChange();
	
	public event OnUnitChange unitAddRemoveCallback;
	
	public LayerMask mask;
	
	public float radius;
	
	public float delay = 0.1f;
		
	private HashSet<T> mUnits = new HashSet<T>();
	private HashSet<T> mGatherUnits = new HashSet<T>();
	
	public HashSet<T> items {
		get { return mUnits; }
	}
	
	protected abstract bool UnitVerify(T unit);
	protected abstract void UnitAdded(T unit);
	protected abstract void UnitRemoved(T unit);
			
	/// <summary>
	/// Grabs one unit in the set
	/// </summary>
	public T GetSingleUnit() {
		T ret = null;
		
		if(mUnits.Count > 0) {
			foreach(T unit in mUnits) {
				if(unit != null) {
					ret = unit;
					break;
				}
			}
		}
		
		return ret;
	}
	
	void OnEnable() {
		InvokeRepeating("Check", delay, delay);
	}
	
	void OnDisable() {
		CancelInvoke();
	}
	
	void Check() {
		int prevCount = mUnits.Count;
		
		CleanUp();
		
		//get units in area
		mGatherUnits.Clear();
		
		Collider[] cols = Physics.OverlapSphere(transform.position, radius, mask.value);
		foreach(Collider col in cols) {
			T unit = col.GetComponent<T>();
			if(unit != null && UnitVerify(unit)) {
				mGatherUnits.Add(unit);
				
				//check if not in current units
				if(!mUnits.Contains(unit)) {
					//enter
					UnitAdded(unit);
				}
				else {
					mUnits.Remove(unit);
				}
			}
		}
		
		//swap
		HashSet<T> prevSet = mUnits;
		mUnits = mGatherUnits;
		mGatherUnits = prevSet;
		
		//what remains should be removed
		foreach(T other in prevSet) {
			UnitRemoved(other);
		}
		
		if(unitAddRemoveCallback != null && prevCount != mUnits.Count)
			unitAddRemoveCallback();
	}
	
	void CleanUp() {
		mUnits.RemoveWhere(delegate(T unit) {return unit == null || !unit.gameObject.activeInHierarchy;});
	}
	
	void OnDrawGizmosSelected() {
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(transform.position, radius);
	}
}
