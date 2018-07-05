using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    [AddComponentMenu("M8/Entity/Activator Trigger2D")]
    public class EntityActivatorTrigger2D : EntityActivator {

        void OnTriggerEnter2D(Collider2D collision) {
            Activate();
        }

        void OnTriggerExit2D(Collider2D collision) {
            Deactivate();
        }
    }
}