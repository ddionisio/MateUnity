using UnityEngine;
using System.Collections;
using Holoville.HOTween;

namespace M8 {
    /// <summary>
    /// Put this in the core object, the one where Main component is
    /// </summary>
    [PrefabCore]
    [AddComponentMenu("M8/HOTween/Controller")]
    public class HOTweenController : SingletonBehaviour<HOTweenController> {
        public bool showCount = true; //set to false when profiling
        public bool overwriteEnable = true; //he said it's awesome, so normally this is set to true.
        public bool overwriteShowLog = false; //if true, overwrite will spam the log when it is overwriting a tween.

        protected override void OnInstanceInit() {
            HOTween.Init(true, showCount, overwriteEnable);

            if(overwriteEnable)
                HOTween.EnableOverwriteManager(overwriteShowLog);
        }
    }
}