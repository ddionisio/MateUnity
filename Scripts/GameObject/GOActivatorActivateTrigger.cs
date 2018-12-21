using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    [AddComponentMenu("M8/Game Object/Activator Activate Trigger")]
    public class GOActivatorActivateTrigger : MonoBehaviour {
        void OnTriggerEnter(Collider collision) {
            var activator = collision.GetComponent<GOActivator>();
            if(activator)
                activator.Activate();
        }

        void OnTriggerExit(Collider collision) {
            var activator = collision.GetComponent<GOActivator>();
            if(activator)
                activator.Deactivate();
        }
    }
}