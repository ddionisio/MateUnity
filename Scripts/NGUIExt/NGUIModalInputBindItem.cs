using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/NGUI/ModalInputBindItem")]
public class NGUIModalInputBindItem : MonoBehaviour {
    public UILabel nameLabel;
    public GameObject primary;
    public GameObject secondary;

    private UIButtonKeys mPrimaryBtnKeys;
    private UILabel mPrimaryLabel;
    private UIEventListener mPrimaryListener;

    private UIButtonKeys mSecondaryBtnKeys;
    private UILabel mSecondaryLabel;
    private UIEventListener mSecondaryListener;

    public UIButtonKeys primaryBtnKeys { get { return mPrimaryBtnKeys; } }
    public UILabel primaryLabel { get { return mPrimaryLabel; } }
    public UIEventListener primaryListener { get { return mPrimaryListener; } }
    
    public UIButtonKeys secondaryBtnKeys { get { return mSecondaryBtnKeys; } }
    public UILabel secondaryLabel { get { return mSecondaryLabel; } }
    public UIEventListener secondaryListener { get { return mSecondaryListener; } }

    public void Awake() {
        mPrimaryBtnKeys = primary.GetComponent<UIButtonKeys>();
        mPrimaryLabel = primary.GetComponentInChildren<UILabel>();
        mPrimaryListener = primary.GetComponent<UIEventListener>();
        
        mSecondaryBtnKeys = secondary.GetComponent<UIButtonKeys>();
        mSecondaryLabel = secondary.GetComponentInChildren<UILabel>();
        mSecondaryListener = secondary.GetComponent<UIEventListener>();
    }
}
