using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/UI/ModalClickStackPop")]
public class UIModalClickStackPop : MonoBehaviour {
    void OnClick() {
        UIModalManager.instance.ModalCloseTop();
    }
}
