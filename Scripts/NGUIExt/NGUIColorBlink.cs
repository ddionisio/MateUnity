using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/NGUI/ColorBlink")]
public class NGUIColorBlink : MonoBehaviour {
    public UIWidget target;
    public Color color;
    public float delay;
    public bool useRealtime = true;

    private const string blinkFunc = "DoBlink";
    private Color mOrigColor;
    private float mLastTime;
    private bool mIsMod = false;

    void OnEnable() {
        mLastTime = useRealtime ? Time.realtimeSinceStartup : Time.time;
    }

    void OnDisable() {
        if(mIsMod) {
            if(target != null)
                target.color = mOrigColor;
            mIsMod = false;
        }
    }

    void Awake() {
        if(target == null)
            target = GetComponent<UIWidget>();

        mOrigColor = target.color;
    }

    void Update() {
        float t = useRealtime ? Time.realtimeSinceStartup : Time.time;
        if(t - mLastTime >= delay) {
            DoBlink();
            mLastTime = t;
        }
    }

    void DoBlink() {
        if(mIsMod)
            target.color = mOrigColor;
        else
            target.color = color;

        mIsMod = !mIsMod;
    }
}
