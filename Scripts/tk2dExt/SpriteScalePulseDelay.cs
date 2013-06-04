using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/tk2D/ScalePulseDelay")]
public class SpriteScalePulseDelay : MonoBehaviour {
    public tk2dBaseSprite sprite;

    public float startDelay;
    public float delay;
    public float pauseDelay;

    public Vector2 startScale;
    public Vector2 endScale;

    public bool squared;

    private WaitForFixedUpdate mDoUpdate;
    private WaitForSeconds mWaitSecondsStart;
    private WaitForSeconds mWaitSecondsUpdate;
        
    void OnEnable() {
        if(sprite != null) {
            StartCoroutine(DoPulseUpdate());
        }
    }

    void OnDisable() {
        if(sprite != null) {
            sprite.scale = new Vector3(startScale.x, startScale.y, 1.0f);
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
        sprite.scale = new Vector3(startScale.x, startScale.y, 1.0f);
        
        if(startDelay > 0.0f)
            yield return mWaitSecondsStart;
        else
            yield return mDoUpdate;

        float t = 0.0f;

        while(true) {
            t += Time.fixedDeltaTime;

            if(t >= delay) {
                sprite.scale = new Vector3(startScale.x, startScale.y, 1.0f);
                t = 0.0f;

                if(pauseDelay > 0.0f) {
                    yield return mWaitSecondsUpdate;
                    continue;
                }
            }
            else {
                float s = Mathf.Sin(Mathf.PI * (t / delay));

                Vector2 scale = Vector2.Lerp(startScale, endScale, squared ? s * s : s);

                sprite.scale = new Vector3(scale.x, scale.y, 1.0f);
            }

            yield return mDoUpdate;
        }
    }
}
