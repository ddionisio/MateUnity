using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("M8/Game/WaypointData")]
public class WaypointData : MonoBehaviour {
    [System.NonSerialized]
    public int curInd;

    [System.NonSerialized]
    public List<Transform> waypoints;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
