using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/NGUI/PointAtDir")]
public class NGUIPointAtDir : MonoBehaviour {
    public Transform origin; //origin to compare direction towards POI

    public Transform pointer;

    public float distance;
    public bool hideInView = true; //hide pointer if POI is within view

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
        mUICam = NGUITools.FindCameraForLayer(origin.gameObject.layer);

        mZ = pointer.position.z;
    }

    // Update is called once per frame
    void Update() {
        if(mPOI != null && mPOI.gameObject.activeInHierarchy) {
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

            if(isEdge || !hideInView) {
                if(!pointer.gameObject.activeSelf) {
                    pointer.gameObject.SetActive(true);
                }

                Vector2 pos = mUICam.ViewportToWorldPoint(vp);
                Vector2 opos = origin.position;

                Vector2 dir = (pos - opos).normalized;

                pointer.position = new Vector3(opos.x + dir.x * distance, pos.y + dir.y * distance, mZ);
                pointer.localPosition = new Vector3(dir.x * distance, dir.y * distance, 0.0f);
                pointer.up = dir;
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
