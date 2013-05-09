using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/2D/TransUpLookAt")]
public class TransUpLookAt : MonoBehaviour {
    public Transform target;
    public Transform source; //the source to set the up vector

    public bool useTrigger; //acquire target via trigger collider, allow for look-at to stop looking upon exit

    private Transform mTrans;

    void OnTriggerEnter(Collider c) {
        if(target == null)
            target = c.transform;
    }

    void OnTriggerExit(Collider c) {
        if(target == c.transform)
            target = null;
    }

    void Awake() {
        if(source == null)
            source = transform;
    }
    		
	// Update is called once per frame
	void Update () {
        if(target != null) {
            Vector2 dpos = target.position - source.position;
            dpos.Normalize();
            source.up = dpos;
        }
	}
}
