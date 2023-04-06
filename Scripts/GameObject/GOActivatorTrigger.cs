using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    [AddComponentMenu("M8/Game Object/Activator Trigger")]
    public class GOActivatorTrigger : GOActivator {
#if !M8_PHYSICS_DISABLED
        void OnTriggerEnter(Collider other) {
            Activate();
        }

        void OnTriggerExit(Collider other) {
            Deactivate();
        }
#endif
    }
}