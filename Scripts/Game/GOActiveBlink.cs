using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/Game Object/Active Blink")]
public class GOActiveBlink : MonoBehaviour {

    public GameObject target;
    public float delay;

    private bool mDefaultActive;
        
    void OnEnable() {
        mDefaultActive = target.activeSelf;
        InvokeRepeating("Blink", delay, delay);
    }

    void OnDisable() {
        CancelInvoke("Blink");
        target.SetActive(mDefaultActive);
    }

    void Blink() {
        target.SetActive(!target.activeSelf);
    }
}
