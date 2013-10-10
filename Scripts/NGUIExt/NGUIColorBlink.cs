using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/NGUI/ColorBlink")]
public class NGUIColorBlink : MonoBehaviour {
    public UIWidget target;
    public Color color;
    public float delay;

    private Color mOrigColor;
    private WaitForSeconds mWait;

    void OnEnable() {
        StartCoroutine(DoBlink());
    }

    void OnDisable() {
        StopAllCoroutines();
        target.color = mOrigColor;
    }

    void Awake() {
        if(target == null)
            target = GetComponent<UIWidget>();

        mOrigColor = target.color;

        mWait = new WaitForSeconds(delay);
    }

    IEnumerator DoBlink() {
        while(true) {
            target.color = color;

            yield return mWait;

            target.color = mOrigColor;

            yield return mWait;
        }

    }
}
