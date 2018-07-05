using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    [AddComponentMenu("M8/Entity/Activator Trigger")]
    public class EntityActivatorTrigger : EntityActivator {

        void OnTriggerEnter(Collider other) {
            Activate();
        }

        void OnTriggerExit(Collider other) {
            Deactivate();
        }
    }
}