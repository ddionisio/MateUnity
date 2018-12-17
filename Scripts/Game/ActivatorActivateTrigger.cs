using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    [AddComponentMenu("M8/Game/Activator Activate Trigger")]
    public class ActivatorActivateTrigger : MonoBehaviour {
        void OnTriggerEnter(Collider collision) {
            var activator = collision.GetComponent<Activator>();
            if(activator)
                activator.Activate();
        }

        void OnTriggerExit(Collider collision) {
            var activator = collision.GetComponent<Activator>();
            if(activator)
                activator.Deactivate();
        }
    }
}