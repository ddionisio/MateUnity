using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
#if !M8_PHYSICS2D_DISABLED
    [AddComponentMenu("M8/Game Object/Activator Trigger2D")]
    public class GOActivatorTrigger2D : GOActivator {
        void OnTriggerEnter2D(Collider2D collision) {
            Activate();
        }

        void OnTriggerExit2D(Collider2D collision) {
            Deactivate();
        }
    }
#endif
}