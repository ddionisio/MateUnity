using UnityEngine;
using System.Collections;

namespace M8.UI {
    public struct Util {
        public static Canvas GetRootCanvas(Transform transform) {
            Canvas rootCanvas = null;
            Transform parent = transform;
            while(parent) {
                rootCanvas = parent.GetComponentInParent<Canvas>();
                if(rootCanvas && !rootCanvas.isRootCanvas)
                    parent = parent.parent;
                else
                    break;
            }

            return rootCanvas;
        }
    }
}