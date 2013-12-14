using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/NGUI/TweenPlayOnEnable")]
public class NGUITweenPlayOnEnable : MonoBehaviour {

    public UITweener tweener;

    void OnEnable() {
        tweener.enabled = true;
        tweener.SetStartToCurrentValue();
        tweener.SetEndToCurrentValue();
    }

    void Awake() {
        if(tweener == null)
            tweener = GetComponent<UITweener>();
    }
}
