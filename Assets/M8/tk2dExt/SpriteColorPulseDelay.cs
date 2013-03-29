using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/tk2D/ColorPulseDelay")]
public class SpriteColorPulseDelay : MonoBehaviour {
    public float startDelay;
    public float delay;
    public float pauseDelay;

    public Color startColor;
    public Color endColor = Color.white;

    private tk2dBaseSprite mSprite;

    void OnEnable() {
        if(mSprite != null) {
            StartCoroutine(DoPulseUpdate());
        }
    }

    void Awake() {
    }

    // Use this for initialization
    void Start() {
        mSprite = GetComponent<tk2dBaseSprite>();
        StartCoroutine(DoPulseUpdate());
    }

    IEnumerator DoPulseUpdate() {
        mSprite.color = startColor;
        
        if(startDelay > 0.0f)
            yield return new WaitForSeconds(startDelay);
        else
            yield return new WaitForFixedUpdate();

        const float pi2 = Mathf.PI * 2.0f;
        float t = 0.0f;

        while(true) {
            t += Time.fixedDeltaTime;

            if(t >= delay) {
                mSprite.color = startColor;
                t = 0.0f;

                if(pauseDelay > 0.0f) {
                    yield return new WaitForSeconds(pauseDelay);
                    continue;
                }
            }
            else {
                float s = Mathf.Sin(pi2 * (t / delay));
                mSprite.color = Color.Lerp(startColor, endColor, s);
            }

            yield return new WaitForFixedUpdate();
        }
    }
}
