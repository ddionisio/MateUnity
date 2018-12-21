using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    [AddComponentMenu("M8/Game Object/Activator Activate Trigger2D")]
    public class GOActivatorActivateTrigger2D : MonoBehaviour {
        void OnTriggerEnter2D(Collider2D collision) {
            var activator = collision.GetComponent<GOActivator>();
            if(activator)
                activator.Activate();
        }

        void OnTriggerExit2D(Collider2D collision) {
            var activator = collision.GetComponent<GOActivator>();
            if(activator)
                activator.Deactivate();
        }
    }
}