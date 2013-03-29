using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaypointData : Object {
    public int curInd;
    public List<Transform> waypoints;
}

[AddComponentMenu("M8/Game/WaypointManager")]
public class WaypointManager : MonoBehaviour {

    private static WaypointManager mInstance = null;
    private Dictionary<string, List<Transform>> mWaypoints;

    public static WaypointManager instance {
        get {
            return mInstance;
        }
    }

    public WaypointData CreateWaypointData(string name) {
        List<Transform> wps = GetWaypoints(name);
        if(wps != null) {
            WaypointData newWaypoint = new WaypointData();
            newWaypoint.curInd = 0;
            newWaypoint.waypoints = wps;
            return newWaypoint;
        }

        return null;
    }

    public List<Transform> GetWaypoints(string name) {
        List<Transform> ret = null;
        mWaypoints.TryGetValue(name, out ret);
        return ret;
    }

    //get the first one if > 1
    public Transform GetWaypoint(string name) {
        Transform ret = null;
        List<Transform> wps;
        if(mWaypoints.TryGetValue(name, out wps) && wps.Count > 0) {
            ret = wps[0];
        }
        return ret;
    }

    void OnDestroy() {
        mInstance = null;
        mWaypoints.Clear();
    }

    void Awake() {
        mInstance = this;

        mWaypoints = new Dictionary<string, List<Transform>>(transform.childCount);

        //generate waypoints based on their names
        foreach(Transform child in transform) {
            List<Transform> points;

            if(child.childCount > 0) {
                points = new List<Transform>(child.childCount);
                foreach(Transform t in child) {
                    points.Add(t);
                }
                points.Sort(delegate(Transform t1, Transform t2) {
                    return t1.name.CompareTo(t2.name);
                });
            }
            else {
                points = new List<Transform>(1);
                points.Add(child);
            }

            mWaypoints.Add(child.name, points);
        }
    }

    void OnDrawGizmos() {
        foreach(Transform t in transform) {
            Gizmos.color = Color.green;
            
            if(t.childCount > 0) {
                Vector3 p = t.GetChild(0).position;

                Gizmos.DrawIcon(p, "waypoint", true);

                for(int i = 1; i < t.childCount; i++) {
                    Vector3 np = t.GetChild(i).position;
                    Vector3 dir = np - p;
                    float len = dir.magnitude;
                    if(len > 0) {
                        dir /= len;
                        M8.Gizmo.ArrowFourLine(p, dir, len, 2.0f, 30.0f);
                        //Gizmos.DrawSphere(np, 1.0f);
                    }

                    p = np;
                }
            }
            else {
                Gizmos.DrawIcon(t.position, "waypoint", true);
            }
        }
    }
}
