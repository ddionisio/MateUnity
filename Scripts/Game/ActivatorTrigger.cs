using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    [AddComponentMenu("M8/Game/Activator Trigger")]
    public class ActivatorTrigger : Activator {

        void OnTriggerEnter(Collider other) {
            Activate();
        }

        void OnTriggerExit(Collider other) {
            Deactivate();
        }
    }
}