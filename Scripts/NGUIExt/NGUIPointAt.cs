using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/NGUI/PointAt")]
public class NGUIPointAt : MonoBehaviour {
    public Transform pointer;
    public bool ignorePOIActive = false;

    private Transform mPOI;
    private Camera mPOICam;

    private Camera mUICam;

    private float mZ;

    public Transform GetPOI() {
        return mPOI;
    }

    public void SetPOI(Transform poi) {
        if(mPOI != poi) {
            mPOI = poi;

            if(mPOI != null) {
                mPOICam = NGUITools.FindCameraForLayer(mPOI.gameObject.layer);

                gameObject.SetActive(true);
            }
            else {
                mPOICam = null;

                gameObject.SetActive(false);

                pointer.gameObject.SetActive(false);
            }
        }
    }

    // Use this for initialization
    void Start() {
        mUICam = NGUITools.FindCameraForLayer(pointer.gameObject.layer);

        mZ = pointer.position.z;
    }

    // Update is called once per frame
    void Update() {
        if(mPOI != null && (ignorePOIActive || mPOI.gameObject.activeInHierarchy)) {
            Vector3 vp = mPOICam.WorldToViewportPoint(mPOI.position);

            bool isEdge = false;

            if(vp.x > 1) {
                vp.x = 1; isEdge = true;
            }
            else if(vp.x < 0) {
                vp.x = 0; isEdge = true;
            }

            if(vp.y > 1) {
                vp.y = 1; isEdge = true;
            }
            else if(vp.y < 0) {
                vp.y = 0; isEdge = true;
            }

            if(isEdge) {
                if(!pointer.gameObject.activeSelf) {
                    pointer.gameObject.SetActive(true);
                }

                Vector3 pos = mUICam.ViewportToWorldPoint(vp);
                pointer.position = new Vector3(pos.x, pos.y, mZ);
                pointer.up = new Vector3(vp.x - 0.5f, vp.y - 0.5f, 0.0f);
            }
            else {
                if(pointer.gameObject.activeSelf) {
                    pointer.gameObject.SetActive(false);
                }
            }
        }
        else {
            pointer.gameObject.SetActive(false);
        }
    }
}
