using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/tk2D/TextColorBlink")]
public class TextColorBlink : MonoBehaviour {
    public tk2dTextMesh sprite;
    public Color color;
    public float delay;

    private Color mOrigColor;
    private WaitForSeconds mWait;

    void OnEnable() {
        StartCoroutine(DoBlink());
    }

    void Awake() {
        if(sprite == null)
            sprite = GetComponent<tk2dTextMesh>();

        mOrigColor = sprite.color;

        mWait = new WaitForSeconds(delay);
    }

    IEnumerator DoBlink() {
        while(true) {
            sprite.color = color;
            sprite.Commit();

            yield return mWait;

            sprite.color = mOrigColor;
            sprite.Commit();

            yield return mWait;
        }

    }
}
