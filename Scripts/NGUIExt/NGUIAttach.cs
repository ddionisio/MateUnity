using UnityEngine;
using System.Collections;

/// <summary>
/// Attach UI to an object in a separate space.
/// </summary>
[AddComponentMenu("M8/NGUI/Attach")]
public class NGUIAttach : MonoBehaviour {
    public Vector2 offset;

    [SerializeField]
    private Transform mTarget;

    private Transform mTrans;
    private Camera mTargetCam;
    private Camera mUICam;
    private Vector3 mPos;
    private bool mVisible = true;
    private float mZ;

    public Transform target {
        get {
            return mTarget;
        }

        set {
            if(mTarget != value) {
                mTarget = value;
                ApplyTargetData();
            }
        }
    }

    void Awake() {
        mTrans = transform;
        mUICam = NGUITools.FindCameraForLayer(gameObject.layer);

        mZ = mTrans.position.z;

        ApplyTargetData();
    }

	// Update is called once per frame
	void LateUpdate () {
        if(mTarget != null) {
            mPos = mTargetCam.WorldToViewportPoint(mTarget.position);

            //TODO: bound check?
            bool visible = (mPos.z > 0.0f && mPos.x > 0.0f && mPos.x < 1.0f && mPos.y > 0.0f && mPos.y < 1.0f);

            if(mVisible != visible) {
                mVisible = visible;
                //deactivate stuff?
            }

            //if(mVisible) {
                mPos = mUICam.ViewportToWorldPoint(mPos);
                mPos.x += offset.x;
                mPos.y += offset.y;
                mPos.z = mZ;
                mTrans.position = mPos;
            //}
        }
	
	}

    private void ApplyTargetData() {
        if(mTarget != null) {
            mTargetCam = NGUITools.FindCameraForLayer(mTarget.gameObject.layer);
        }
    }
}
