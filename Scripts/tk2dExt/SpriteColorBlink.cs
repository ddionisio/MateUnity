using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/tk2D/ColorBlink")]
public class SpriteColorBlink : MonoBehaviour {
    public tk2dBaseSprite sprite;
    public Color color;
    public float delay;

    private Color mOrigColor;
    private WaitForSeconds mWait;

    void OnEnable() {
        StartCoroutine(DoBlink());
    }

    void OnDisable() {
        StopAllCoroutines();

        if(sprite != null)
            sprite.color = mOrigColor;
    }

    void Awake() {
        if(sprite == null)
            sprite = GetComponent<tk2dBaseSprite>();

        mOrigColor = sprite.color;

        mWait = new WaitForSeconds(delay);
    }

    IEnumerator DoBlink() {
        while(true) {
            sprite.color = color;

            yield return mWait;

            sprite.color = mOrigColor;

            yield return mWait;
        }

    }
}
