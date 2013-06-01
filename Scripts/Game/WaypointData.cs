using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("M8/Game/WaypointData")]
public class WaypointData : MonoBehaviour {
    public string waypoint;

    public int startIndex = 0;

    [System.NonSerialized]
    public int curInd;

    [System.NonSerialized]
    public List<Transform> waypoints;

    void Awake() {
        curInd = startIndex;
    }

    void Start() {
        if(!string.IsNullOrEmpty(waypoint)) {
            waypoints = WaypointManager.instance.GetWaypoints(waypoint);
        }
    }
}
