using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//TODO: right now it assumes a sphere collider
[AddComponentMenu("M8/Physics/First-Person RigidBodyController (with Look)")]
public class FPController : RigidBodyController {
    public Transform eye;
    public Vector3 eyeOfs;

    public float jumpImpulse = 5.0f;
    public float jumpWaterForce = 5.0f;
    public float jumpForce = 50.0f;
    public float jumpDelay = 0.15f;

    public float lookSensitivity = 2.0f;
    public bool lookYInvert = false;
    public float lookYAngleMin = -90.0f;
    public float lookYAngleMax = 90.0f;

    public int player = 0;
    public int moveInputX = InputManager.ActionInvalid;
    public int moveInputY = InputManager.ActionInvalid;
    public int lookInputX = InputManager.ActionInvalid;
    public int lookInputY = InputManager.ActionInvalid;
    public int jumpInput = InputManager.ActionInvalid;

    public bool startInputEnabled = false;

    private bool mInputEnabled = false;

    private bool mJump = false;
    private float mJumpLastTime = 0.0f;

    private Vector2 mLookCurInputAxis;
    private float mLookCurRot;

    public bool inputEnabled {
        get { return mInputEnabled; }
        set {
            if(mInputEnabled != value) {
                mInputEnabled = value;

                InputManager input = Main.instance != null ? Main.instance.input : null;
                if(input != null) {
                    if(mInputEnabled) {
                        input.AddButtonCall(player, jumpInput, OnInputJump);
                    }
                    else {
                        input.RemoveButtonCall(player, jumpInput, OnInputJump);
                    }
                }
            }
        }
    }

    protected override void WaterExit() {
        if(mJump) {
            if(jumpImpulse > 0.0f)
                rigidbody.AddForce(dirHolder.up * jumpImpulse, ForceMode.Impulse);

            mJumpLastTime = Time.fixedTime;
        }
    }
    
    protected override void OnDestroy() {
        inputEnabled = false;

        base.OnDestroy();
    }

    // Use this for initialization
    protected override void Start() {
        base.Start();

        inputEnabled = startInputEnabled;
    }

    // Update is called once per frame
    protected override void FixedUpdate() {
        Rigidbody body = rigidbody;
        Quaternion dirRot = dirHolder.rotation;

        if(mInputEnabled) {
            InputManager input = Main.instance.input;

            //movement
            moveForward = 0.0f;
            moveSide = 0.0f;

            if(isUnderWater && !isGrounded) {
                //Move based on eye
                Move(eye.rotation, new Vector2(input.GetAxis(player, moveInputX), input.GetAxis(player, moveInputY)), moveForce);
            }
            else if(!isSlopSlide) {
                if(moveInputX != InputManager.ActionInvalid)
                    moveSide = input.GetAxis(player, moveInputX);

                if(moveInputY != InputManager.ActionInvalid)
                    moveForward = input.GetAxis(player, moveInputY);
            }

            //look
            mLookCurInputAxis.x = lookInputX != InputManager.ActionInvalid ? input.GetAxis(player, lookInputX) : 0.0f;
            mLookCurInputAxis.y = lookInputY != InputManager.ActionInvalid ? input.GetAxis(player, lookInputY) : 0.0f;

            if(mLookCurInputAxis.x != 0.0f) {
                dirRot *= Quaternion.AngleAxis(mLookCurInputAxis.x * lookSensitivity, Vector3.up);
                dirHolder.rotation = dirRot;
            }

            //look vertical
            float vDelta = mLookCurInputAxis.y * lookSensitivity;

            mLookCurRot += vDelta;

            if(mLookCurRot < -360.0f)
                mLookCurRot += 360.0f;
            else if(mLookCurRot > 360.0f)
                mLookCurRot -= 360.0f;

            mLookCurRot = Mathf.Clamp(mLookCurRot, lookYAngleMin, lookYAngleMax);

            //jump
            if(mJump) {
                if(isUnderWater) {
                    body.AddForce(dirRot * Vector3.up * jumpWaterForce);
                }
                else {
                    if(Time.fixedTime - mJumpLastTime >= jumpDelay || (collisionFlags & CollisionFlags.Above) != 0) {
                        mJump = false;
                    }
                    else {
                        body.AddForce(dirRot * Vector3.up * jumpForce);
                    }
                }
            }
        }
        else {
            moveForward = 0.0f;
            moveSide = 0.0f;
            mLookCurInputAxis = Vector2.zero;
            mJump = false;
        }

        //set eye rotation
        if(eye != null) {
            Quaternion rot = dirHolder.rotation;

            if(mLookCurRot != 0.0f)
                rot *= Quaternion.AngleAxis(lookYInvert ? mLookCurRot : -mLookCurRot, Vector3.right);

            eye.rotation = rot;

            Vector3 pos = dirHolder.position;

            if(eyeOfs.x != 0.0f)
                pos += dirHolder.right * eyeOfs.x;

            if(eyeOfs.y != 0.0f)
                pos += dirHolder.up * eyeOfs.y;

            if(eyeOfs.z != 0.0f)
                pos += dirHolder.forward * eyeOfs.z;

            eye.position = pos;
        }

        base.FixedUpdate();
    }

    void OnInputJump(InputManager.Info dat) {
        if(dat.state == InputManager.State.Pressed) {
            if(!mJump) {
                if(isUnderWater) {
                    mJump = true;
                }
                else if(isGrounded && !isSlopSlide) {
                    if(jumpImpulse > 0.0f)
                        rigidbody.AddForce(dirHolder.up * jumpImpulse, ForceMode.Impulse);

                    mJump = true;
                    mJumpLastTime = Time.fixedTime;
                }
            }
        }
        else if(dat.state == InputManager.State.Released) {
            mJump = false;
        }
    }
}
