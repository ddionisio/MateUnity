using UnityEngine;
using System.Collections;

namespace M8 {
    [AddComponentMenu("M8/Transform/RotateTowards")]
    public class TransRotateTowards : MonoBehaviour {
        public Transform target;

        public float speed;

        public bool local;

        // Update is called once per frame
        void Update() {
            if(target) {
                if(local)
                    transform.localRotation = Quaternion.RotateTowards(transform.localRotation, target.localRotation, speed*Time.deltaTime);
                else
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, target.rotation, speed*Time.deltaTime);
            }
        }
    }
}