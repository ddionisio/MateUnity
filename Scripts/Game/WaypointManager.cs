using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using System.Text.RegularExpressions;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace M8 {
    [AddComponentMenu("M8/Game/WaypointManager")]
    public class WaypointManager : MonoBehaviour {

        private static WaypointManager mInstance = null;
        private Dictionary<string, List<Transform>> mWaypoints;

#if UNITY_EDITOR
        public float arrowSize = 2.0f;
        public float arrowAngle = 30.0f;
        public float pointSize = 0.5f;
        public float pointSizeSelected = 1.0f;

        [HideInInspector]
        public Transform __inspectorSelected;
#endif

        public static WaypointManager instance {
            get {
                return mInstance;
            }
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
                        int i1 = int.Parse(t1.name);
                        int i2 = int.Parse(t2.name);
                        return i1.CompareTo(i2);
                    });
                }
                else {
                    points = new List<Transform>(1);
                    points.Add(child);
                }

                mWaypoints.Add(child.name, points);
            }
        }

#if UNITY_EDITOR
        void OnDrawGizmos() {
            Transform selection = Selection.activeTransform;

            foreach(Transform t in transform) {
                bool wpActive = selection != null 
                && (t == selection || selection.parent == t || (__inspectorSelected != null && (__inspectorSelected == t || __inspectorSelected.parent == t)));
                Gizmos.color = wpActive ? Color.green : Color.green * 0.5f;

                if(t.childCount > 0) {
                    //first child
                    Transform child = t.GetChild(0);

                    Vector3 p = child.position;

                    if(pointSize > 0.0f)
                        Gizmos.DrawSphere(p, pointSize);

                    if(child == selection || child == __inspectorSelected) {
                        if(pointSizeSelected > 0.0f) {
                            Gizmos.DrawWireSphere(p, pointSizeSelected);
                        }
                    }
                    //

                    //render others
                    for(int i = 1; i < t.childCount; i++) {
                        child = t.GetChild(i);
                        Vector3 np = child.position;
                        Vector3 dir = np - p;
                        float len = dir.magnitude;
                        if(len > 0) {
                            dir /= len;
                            M8.Gizmo.ArrowFourLine(p, dir, len, arrowSize, arrowAngle);
                        }

                        if(pointSize > 0.0f)
                            Gizmos.DrawSphere(np, pointSize);

                        if(child == selection || child == __inspectorSelected) {
                            if(pointSizeSelected > 0.0f) {
                                Gizmos.DrawWireSphere(np, pointSizeSelected);
                            }
                        }

                        p = np;
                    }
                }
                else {
                    Vector3 np = t.position;

                    if(pointSize > 0.0f)
                        Gizmos.DrawSphere(np, pointSize);

                    if(t == selection || t == __inspectorSelected) {
                        if(pointSizeSelected > 0.0f) {
                            Gizmos.DrawWireSphere(np, pointSizeSelected);
                        }
                    }
                }
            }
        }
#endif
    }
}