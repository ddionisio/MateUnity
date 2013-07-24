using UnityEngine;
using System.Collections;

public abstract class NGUILayoutBase : MonoBehaviour {
    /// <summary>
    /// lower number is the last to be updated
    /// </summary>
    public int priority;

    public bool rounding = true;
    public bool alwaysUpdate = false;
    public bool repositionOnEnable = false;
    public bool repositionNow = false;

    private bool mStarted = false;
        
    public static int Comparer(NGUILayoutBase l1, NGUILayoutBase l2) {
        return (M8.Util.GetNodeLayer(l1.transform)*1000 + l1.priority).CompareTo(M8.Util.GetNodeLayer(l2.transform)*1000 + l2.priority);
    }

    /// <summary>
    /// Reposition any layouts within the given t's hierarchy.
    /// </summary>
    public static void RefreshNow(Transform t) {
        t.BroadcastMessage("ProcessText", null, SendMessageOptions.DontRequireReceiver);

        NGUILayoutBase[] layouts = t.GetComponentsInChildren<NGUILayoutBase>(false);

        //sort
        System.Array.Sort(layouts, NGUILayoutBase.Comparer);

        for(int i = layouts.Length - 1; i >= 0; i--) {
            if(!layouts[i].alwaysUpdate)
                layouts[i].Reposition();
        }
    }

    public abstract void Reposition();

    protected virtual void OnEnable() {
        if(mStarted && repositionOnEnable)
            Reposition();
    }

    protected virtual void Start() {
        mStarted = true;
        if(repositionOnEnable)
            Reposition();
    }

    // Update is called once per frame
    protected virtual void Update() {
#if UNITY_EDITOR
        if(!Application.isPlaying || alwaysUpdate || repositionNow) {
            Reposition();
            repositionNow = false;
        }
#else
		if(alwaysUpdate || repositionNow) {
			Reposition();
            repositionNow = false;
		}
#endif
    }
}
