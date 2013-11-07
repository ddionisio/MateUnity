using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/NGUI/ColorBlink")]
public class NGUIColorBlink : MonoBehaviour {
    public UIWidget target;
    public Color color;
    public float delay;

    private Color mOrigColor;
    private WaitForSeconds mWait;
    private bool mStarted;
    private bool mBlinkActive;

    void OnEnable() {
        if(mStarted && !mBlinkActive)
            StartCoroutine(DoBlink());
    }

    void OnDisable() {
        if(mStarted) {
            mBlinkActive = false;
            StopAllCoroutines();

            if(target != null)
                target.color = mOrigColor;
        }
    }

    void Awake() {
        if(target == null)
            target = GetComponent<UIWidget>();

        mOrigColor = target.color;

        mWait = new WaitForSeconds(delay);
    }

    void Start() {
        mStarted = true;
        StartCoroutine(DoBlink());
    }

    IEnumerator DoBlink() {
        mBlinkActive = true;

        while(mBlinkActive) {
            target.color = color;

            yield return mWait;

            target.color = mOrigColor;

            yield return mWait;
        }
    }
}
