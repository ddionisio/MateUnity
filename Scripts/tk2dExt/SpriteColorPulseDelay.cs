using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/tk2D/ColorPulseDelay")]
public class SpriteColorPulseDelay : MonoBehaviour {
    public tk2dBaseSprite sprite;

    public float startDelay;
    public float delay;
    public float pauseDelay;

    public Color startColor;
    public Color endColor = Color.white;

    public bool squared;

    private WaitForFixedUpdate mDoUpdate;
    private WaitForSeconds mWaitSecondsStart;
    private WaitForSeconds mWaitSecondsUpdate;

    private bool mStarted = false;
        
    void OnEnable() {
        if(mStarted) {
            StartCoroutine(DoPulseUpdate());
        }
    }

    void OnDisable() {
        if(mStarted) {
            StopAllCoroutines();
            sprite.color = startColor;
        }
    }

    void Awake() {
        if(sprite == null)
            sprite = GetComponent<tk2dBaseSprite>();

        mDoUpdate = new WaitForFixedUpdate();
        mWaitSecondsStart = new WaitForSeconds(startDelay);
        mWaitSecondsUpdate = new WaitForSeconds(pauseDelay);
    }

    // Use this for initialization
    void Start() {
        mStarted = true;
        StartCoroutine(DoPulseUpdate());
    }

    IEnumerator DoPulseUpdate() {
        sprite.color = startColor;
        
        if(startDelay > 0.0f)
            yield return mWaitSecondsStart;
        else
            yield return mDoUpdate;

        float t = 0.0f;

        while(true) {
            t += Time.fixedDeltaTime;

            if(t >= delay) {
                sprite.color = startColor;
                t = 0.0f;

                if(pauseDelay > 0.0f) {
                    yield return mWaitSecondsUpdate;
                    continue;
                }
            }
            else {
                float s = Mathf.Sin(Mathf.PI * (t / delay));
                sprite.color = Color.Lerp(startColor, endColor, squared ? s * s : s);
            }

            yield return mDoUpdate;
        }
    }
}
