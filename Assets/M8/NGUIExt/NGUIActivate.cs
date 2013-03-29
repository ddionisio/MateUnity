using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/NGUI/Activate")]
public class NGUIActivate : MonoBehaviour {
    public GameObject target;
    public GameObject targetHover;

    private bool mStarted = false;

    void OnEnable() {
        if(mStarted) {
            if(target != null)
                target.SetActive(UICamera.selectedObject == gameObject);

            if(targetHover != null) {
                targetHover.SetActive(UICamera.IsHighlighted(gameObject));
            }
        }
    }

    void OnSelect(bool yes) {
        if(target != null)
            target.SetActive(yes);
    }

    void OnHover(bool yes) {
        if(targetHover != null)
            targetHover.SetActive(yes);
    }

    void Awake() {
        if(target != null)
            target.SetActive(false);
    }

    void Start() {
        mStarted = true;
    }
}
