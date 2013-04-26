using UnityEngine;
using System.Collections;

public class EntityWarp : MonoBehaviour {
	public const int maxCheckCount = 20;
	
	public delegate void Callback(bool success);
	
	public SphereCollider colliderCheck;
	public float radiusCheck; //if colliderCheck is null
	
	public float outerRadius;
	
	public LayerMask layerCheck;
	
	public event Callback onFinishCallback;
	
	

	public void WarpTo(Vector2 pos) {
		if(outerRadius > 0.0f) {
			StartCoroutine(DoWarp(pos));
		}
		else {
			float r = colliderCheck != null ? colliderCheck.radius : radiusCheck;
			
			bool success = Physics.CheckSphere(new Vector3(pos.x, pos.y, 0.0f), r, layerCheck.value);
			
			if(success) {
				transform.position = new Vector3(pos.x, pos.y, transform.position.z);
			}
			
			if(onFinishCallback != null) {
				onFinishCallback(success);
			}
		}
	}
	
	IEnumerator DoWarp(Vector2 pos) {
		float r = colliderCheck != null ? colliderCheck.radius : radiusCheck;
		
		for(int i = 0; i < maxCheckCount; i++) {
			Vector2 dir = Random.insideUnitCircle;
			float dist = dir.magnitude;
			if(dist > 0.0f) {
				dir /= dist;
				dir *= outerRadius;
				
				Vector2 newPos = pos+dir;
				if(!Physics.CheckSphere(new Vector3(newPos.x, newPos.y, 0.0f), r, layerCheck.value)) {
					transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
					
					if(onFinishCallback != null) {
						onFinishCallback(true);
					}
					
					yield break;
				}
			}
			
			yield return new WaitForFixedUpdate();
		}
		
		if(onFinishCallback != null) {
			onFinishCallback(false);
		}
		
		yield break;
	}
	
	void OnDestroy() {
		onFinishCallback = null;
	}
	
	void OnDrawGizmosSelected() {
		Gizmos.color = new Color(128.0f/255.0f, 158.0f/255.0f, 182.0f/255.0f);
		
		if(outerRadius > 0.0f) {
			Gizmos.DrawWireSphere(transform.position, outerRadius);
		}
		
		if(colliderCheck == null && radiusCheck > 0.0f) {
			Gizmos.color = Gizmos.color*0.8f;
			Gizmos.DrawWireSphere(transform.position, radiusCheck);
		}
	}
}
