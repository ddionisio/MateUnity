using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace M8.UI {
    [AddComponentMenu("M8/UI/Selectable/Select On Enable")]
    public class SelectOnEnable : MonoBehaviour {
        public Selectable target;

        void OnEnable() {
            target.Select();
        }

        void Awake() {
            if(!target)
                target = GetComponent<Selectable>();
        }
    }
}