using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/NGUI/SelectClick")]
public class NGUISelectClick : MonoBehaviour {
    public GameObject select;

    void OnClick() {
        if(enabled && select != null) {
            UICamera.selectedObject = select;
        }
    }
}
