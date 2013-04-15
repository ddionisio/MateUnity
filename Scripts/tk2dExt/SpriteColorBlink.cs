using UnityEngine;
using System.Collections;

public class SpriteColorBlink : MonoBehaviour {
    public tk2dBaseSprite sprite;
    public Color color;
    public float delay;

    private Color mOrigColor;
    private WaitForSeconds mWait;

    void OnEnable() {
        StartCoroutine(DoBlink());
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
