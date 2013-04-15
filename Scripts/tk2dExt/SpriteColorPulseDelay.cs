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

    private WaitForFixedUpdate mDoUpdate;
    private WaitForSeconds mWaitSecondsStart;
    private WaitForSeconds mWaitSecondsUpdate;
        
    void OnEnable() {
        if(sprite != null) {
            StartCoroutine(DoPulseUpdate());
        }
    }

    void Awake() {
        mDoUpdate = new WaitForFixedUpdate();
        mWaitSecondsStart = new WaitForSeconds(startDelay);
        mWaitSecondsUpdate = new WaitForSeconds(pauseDelay);
    }

    // Use this for initialization
    void Start() {
        if(sprite == null)
            sprite = GetComponent<tk2dBaseSprite>();

        StartCoroutine(DoPulseUpdate());
    }

    IEnumerator DoPulseUpdate() {
        sprite.color = startColor;
        
        if(startDelay > 0.0f)
            yield return mWaitSecondsStart;
        else
            yield return mDoUpdate;

        const float pi2 = Mathf.PI * 2.0f;
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
                float s = Mathf.Sin(pi2 * (t / delay));
                sprite.color = Color.Lerp(startColor, endColor, s);
            }

            yield return mDoUpdate;
        }
    }
}
