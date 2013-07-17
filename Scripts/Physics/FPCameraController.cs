using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/Physics/First-Person Camera Controller")]
public class FPCameraController : MonoBehaviour {
    public Transform eye;
    public Transform attach; //this is the body that the eye is attached to, position of camera will be set to this

    public Vector3 offset;

    public Vector2 lookSensitivity;

    public bool lookYInvert = false;

    public Vector2 angleMin = new Vector2(-360.0f, -90.0f);
    public Vector2 angleMax = new Vector2(360.0f, 90.0f);

    public int player = 0;
    public int lookInputX = InputManager.ActionInvalid;
    public int lookInputY = InputManager.ActionInvalid;

    public bool startInputEnabled = false;

    private bool mInputEnabled = false;

    private Vector2 mCurLookInputAxis;
    private Vector2 mCurRot;

    public bool inputEnabled {
        get { return mInputEnabled; }
        set {
            if(mInputEnabled != value) {
                mInputEnabled = value;

                InputManager input = Main.instance != null ? Main.instance.input : null;
                if(input != null) {
                    if(mInputEnabled) {
                    }
                    else {
                    }
                }
            }
        }
    }

    public void ResetRotate() {
        mCurLookInputAxis = Vector2.zero;
        mCurRot = Vector2.zero;
    }

    void OnDestroy() {
        inputEnabled = false;
    }

    void Awake() {
        if(eye == null)
            eye = transform;
    }

    // Use this for initialization
    void Start() {
        inputEnabled = startInputEnabled;
    }

    // Update is called once per frame
    void Update() {
        if(mInputEnabled) {
            InputManager input = Main.instance.input;

            mCurLookInputAxis.x = lookInputX != InputManager.ActionInvalid ? input.GetAxis(player, lookInputX) : 0.0f;
            mCurLookInputAxis.y = lookInputY != InputManager.ActionInvalid ? input.GetAxis(player, lookInputY) : 0.0f;

            Vector2 delta = new Vector2(mCurLookInputAxis.x * lookSensitivity.x, mCurLookInputAxis.y * lookSensitivity.y);

            mCurRot += delta;

            if(mCurRot.x < -360.0f)
                mCurRot.x += 360.0f;
            else if(mCurRot.x > 360.0f)
                mCurRot.x -= 360.0f;

            if(mCurRot.y < -360.0f)
                mCurRot.y += 360.0f;
            else if(mCurRot.y > 360.0f)
                mCurRot.y -= 360.0f;

            mCurRot.x = Mathf.Clamp(mCurRot.x, angleMin.x, angleMax.x);
            mCurRot.y = Mathf.Clamp(mCurRot.y, angleMin.y, angleMax.y);
        }
        else {
            mCurLookInputAxis = Vector2.zero;
        }

        if(attach != null) {
            Quaternion rot = attach.rotation;

            if(mCurRot.x != 0.0f)
                rot *= Quaternion.AngleAxis(mCurRot.x, Vector3.up);

            if(mCurRot.y != 0.0f)
                rot *= Quaternion.AngleAxis(lookYInvert ? mCurRot.y : -mCurRot.y, Vector3.right);
            
            eye.rotation = rot;

            Vector3 pos = attach.position;

            if(offset.x != 0.0f)
                pos += attach.right * offset.x;

            if(offset.y != 0.0f)
                pos += attach.up * offset.y;

            if(offset.z != 0.0f)
                pos += attach.forward * offset.z;

            eye.position = pos;
        }
    }
}
