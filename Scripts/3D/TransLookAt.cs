using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/3D/TransLookAt")]
public class TransLookAt : MonoBehaviour {
    public string targetTag = "MainCamera"; //if target is null
    public Transform target;

    public bool visibleCheck = true; //if true, only compute if source's renderer is visible

    public Transform source; //if null, use this transform

    void Awake() {
        if(target == null) {
            GameObject go = GameObject.FindGameObjectWithTag(targetTag);
            if(go != null)
                target = go.transform;
        }

        if(source == null)
            source = transform;
    }

    void Update() {
        if(!visibleCheck || source.renderer.isVisible) {
            source.rotation = Quaternion.LookRotation(-target.forward, target.up);
        }
    }
}
