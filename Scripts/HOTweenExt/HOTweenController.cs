using UnityEngine;
using System.Collections;
using Holoville.HOTween;

/// <summary>
/// Put this in the core object, the one where Main component is
/// </summary>
[AddComponentMenu("M8/HOTween/Controller")]
public class HOTweenController : MonoBehaviour {
    public bool showCount = true; //set to false when profiling
    public bool overwriteEnable = true; //he said it's awesome, so normally this is set to true.
    public bool overwriteShowLog = false; //if true, overwrite will spam the log when it is overwriting a tween.

    private static HOTweenController mInstance = null;

    public static HOTweenController instance { get { return mInstance; } }

    void OnDestroy() {
        if(mInstance == this)
            mInstance = null;
    }

    void Awake() {
        if(mInstance == null) {
            mInstance = this;

            HOTween.Init(true, showCount, overwriteEnable);

            if(overwriteEnable)
                HOTween.EnableOverwriteManager(overwriteShowLog);
        }
    }
}
