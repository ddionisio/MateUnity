using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/NGUI/ColorBlink")]
public class NGUIColorBlink : MonoBehaviour {
    public UIWidget target;
    public Color color;
    public float delay;

    private const string blinkFunc = "DoBlink";
    private Color mOrigColor;
    private bool mStarted;

    void OnEnable() {
        if(mStarted && !IsInvoking(blinkFunc))
            InvokeRepeating(blinkFunc, 0, delay);
    }

    void OnDisable() {
        if(mStarted) {
            CancelInvoke();

            if(target != null)
                target.color = mOrigColor;
        }
    }

    void Awake() {
        if(target == null)
            target = GetComponent<UIWidget>();

        mOrigColor = target.color;
    }

    void Start() {
        mStarted = true;
        InvokeRepeating(blinkFunc, 0, delay);
    }

    void DoBlink() {
        if(target.color == mOrigColor)
            target.color = color;
        else
            target.color = mOrigColor;
    }
}
