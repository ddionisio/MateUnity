using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//TODO: right now it assumes a sphere collider
[AddComponentMenu("M8/Physics/First-Person RigidBodyController")]
public class FPMoveController : RigidBodyController {
    public float jumpForce = 50.0f;
    public float jumpDelay = 0.15f;

    public float turnSensitivity = 2.0f;

    public int player = 0;
    public int moveInputX = InputManager.ActionInvalid;
    public int moveInputY = InputManager.ActionInvalid;
    public int turnInput = InputManager.ActionInvalid;
    public int jumpInput = InputManager.ActionInvalid;

    public bool startInputEnabled = false;

    private float mCurInputTurnAxis;

    private bool mInputEnabled = false;

    private bool mJump = false;
    private float mJumpLastTime = 0.0f;
    
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

    void OnDestroy() {
        inputEnabled = false;
    }

    // Use this for initialization
    void Start() {
        inputEnabled = startInputEnabled;
    }

    // Update is called once per frame
    protected override void FixedUpdate() {
        Rigidbody body = rigidbody;
        Quaternion rot = transform.rotation;

        Quaternion dirRot = dirHolder.rotation;

        Vector2 newMoveAxis;

        if(mInputEnabled) {
            InputManager input = Main.instance.input;

            if(!isSlopSlide) {
                newMoveAxis.x = moveInputX != InputManager.ActionInvalid ? input.GetAxis(player, moveInputX) : 0.0f;
                newMoveAxis.y = moveInputY != InputManager.ActionInvalid ? input.GetAxis(player, moveInputY) : 0.0f;
            }
            else {
                newMoveAxis = Vector2.zero;
            }

            mCurInputTurnAxis = turnInput != InputManager.ActionInvalid ? input.GetAxis(player, turnInput) : 0.0f;

            if(mCurInputTurnAxis != 0.0f) {
                dirRot *= Quaternion.AngleAxis(mCurInputTurnAxis * turnSensitivity, Vector3.up);
                dirHolder.rotation = dirRot;
            }

            if(mJump) {
                if(Time.fixedTime - mJumpLastTime >= jumpDelay || (collisionFlags & CollisionFlags.Above) != 0) {
                    mJump = false;
                }
                else {
                    body.AddForce(rot * Vector3.up * jumpForce);
                }
            }
        }
        else {
            newMoveAxis = Vector2.zero;
            mCurInputTurnAxis = 0.0f;
        }

        moveAxis = newMoveAxis;

        base.FixedUpdate();
    }

    void OnInputJump(InputManager.Info dat) {
        if(dat.state == InputManager.State.Pressed) {
            if(isGrounded && !mJump && !isSlopSlide) {
                mJump = true;
                mJumpLastTime = Time.fixedTime;
            }
        }
        else if(dat.state == InputManager.State.Released) {
            mJump = false;
        }
    }
}
