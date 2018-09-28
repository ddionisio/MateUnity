using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace M8.UI {
    [AddComponentMenu("M8/UI/InputField/Activate On Enable")]
    public class InputFieldActivateOnEnable : MonoBehaviour {
        public InputField inputField;

        void OnEnable() {
            if(inputField)
                inputField.ActivateInputField();
        }

        void Awake() {
            if(!inputField)
                inputField = GetComponent<InputField>();
        }
    }
}