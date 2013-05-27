using UnityEngine;
using System.Collections;

/// <summary>
/// Rotate Z axis back and forth
/// </summary>
[AddComponentMenu("M8/2D/TransAnimRotWave")]
public class TransAnimRotWave : MonoBehaviour {

    //TODO: types, lerp modes

    public Transform target;

    public float originRot;

    public float rot;

    public float speed;

    void OnEnable() {

    }

    void Awake() {
        if(target == null)
            target = transform;

        transform.eulerAngles = new Vector3(0, 0, originRot);
    }

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        Vector3 a = transform.eulerAngles;
        a.z = originRot + Mathf.Sin(Time.time * speed * Mathf.Deg2Rad) * rot;
        transform.eulerAngles = a;
    }
}
