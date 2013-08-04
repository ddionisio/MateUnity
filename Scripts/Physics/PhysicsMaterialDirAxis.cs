using UnityEngine;
using System.Collections;

/// <summary>
/// Set the physics material direction based on given axis
/// </summary>
[AddComponentMenu("M8/Physics/PhysicsMaterialDirAxis")]
public class PhysicsMaterialDirAxis : MonoBehaviour {
    public enum Axis {
        Up,
        Forward,
        Right
    }

    public Axis axis;
    public bool invert;

    void Start() {
        ApplyDir();
    }

    void FixedUpdate() {
        if(!gameObject.isStatic)
            ApplyDir();
    }

    void ApplyDir() {
        Vector3 dir = Vector3.zero;

        switch(axis) {
            case Axis.Up:
                dir = Vector3.up;
                break;
            case Axis.Forward:
                dir = Vector3.forward;
                break;
            case Axis.Right:
                dir = Vector3.right;
                break;
        }

        if(invert)
            dir *= -1;

        collider.material.frictionDirection2 = dir;
    }
}
