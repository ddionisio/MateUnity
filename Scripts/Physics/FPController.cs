using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//TODO: right now it assumes a sphere collider
[AddComponentMenu("M8/Physics/First-Person RigidBodyController")]
public class FPController : RigidBodyController {
    [SerializeField]
    Transform _eye;

    public Vector3 eyeOfs;
    public float eyeLockOrientSpeed = 180.0f; //when we lock the eye again, this is the speed to re-orient based on dirHolder
    public float eyeLockPositionDelay = 1.0f; //reposition delay when we lock the eye again

    public bool moveNormalized = true; //use false for analogue

    public float jumpImpulse = 5.0f;
    public float jumpWaterForce = 5.0f;
    public float jumpForce = 50.0f;
    public float jumpDelay = 0.15f;

    public float lookSensitivity = 2.0f;
    public bool lookYInvert = false;
    public float lookYAngleMin = -90.0f;
    public float lookYAngleMax = 90.0f;

    public string ladderTag = "Ladder";
    public LayerMask ladderLayer;
    public float ladderOrientSpeed = 270.0f;
    public float ladderDrag = 20.0f;
    public float ladderJumpForce = 10.0f;

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

    private GravityController mGravityCtrl;

    private int mLadderCounter;
    private bool mLadderLastGravity;
    private Vector3 mLadderUp;
    private Quaternion mLadderRot;

    private bool mEyeLocked = true;
    private bool mEyeOrienting = false;
    private Vector3 mEyeOrientVel;

    private WaitForFixedUpdate mWaitUpdate = new WaitForFixedUpdate();

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

    public bool isOnLadder { get { return mLadderCounter > 0; } }

    public GravityController gravityController { get { return mGravityCtrl; } }

    public Transform eye {
        get { return _eye; }
    }

    /// <summary>
    /// This determines whether or not the eye will be set to the dirHolder's transform
    /// </summary>
    public bool eyeLocked {
        get { return _eye != null && mEyeLocked; }
        set {
            if(mEyeLocked != value && _eye != null) {
                mEyeLocked = value;

                if(mEyeLocked) {
                    //move eye orientation to dirHolder
                    if(!mEyeOrienting) {
                        mEyeOrienting = true;
                        StartCoroutine(EyeOrienting());
                    }

                    mEyeOrientVel = Vector3.zero;
                }
                else {
                    mLookCurRot = 0.0f;
                    mEyeOrienting = false;
                }
            }
        }
    }

    public override void ResetCollision() {
        base.ResetCollision();

        if(mLadderCounter > 0) {
            if(mGravityCtrl != null)
                mGravityCtrl.enabled = true;
            else
                rigidbody.useGravity = mLadderLastGravity;

            mLadderCounter = 0;
        }
    }

    protected override void WaterExit() {
        if(mJump) {
            if(jumpImpulse > 0.0f)
                rigidbody.AddForce(dirHolder.up * jumpImpulse, ForceMode.Impulse);

            mJumpLastTime = Time.fixedTime;
        }
    }
        
    protected override void OnTriggerEnter(Collider col) {
        base.OnTriggerEnter(col);

        if(M8.Util.CheckLayerAndTag(col.gameObject, ladderLayer, ladderTag)) {
            mLadderUp = col.transform.up;

            if(!M8.MathUtil.RotateToUp(mLadderUp, transform.right, transform.forward, ref mLadderRot))
                transform.up = mLadderUp;

            mLadderCounter++;
        }

        if(isOnLadder) {
            if(mGravityCtrl != null) {
                StartCoroutine(LadderOrientUp());
                mGravityCtrl.enabled = false;
            }
            else {
                mLadderLastGravity = rigidbody.useGravity;
                rigidbody.useGravity = false;
            }
        }
    }

    protected override void OnTriggerStay(Collider col) {
        base.OnTriggerStay(col);

        if(M8.Util.CheckLayerAndTag(col.gameObject, ladderLayer, ladderTag)) {
            if(mLadderCounter == 0) {
                mLadderCounter++;

                if(mGravityCtrl != null) {
                    StartCoroutine(LadderOrientUp());
                    mGravityCtrl.enabled = false;
                }
                else {
                    mLadderLastGravity = rigidbody.useGravity;
                    rigidbody.useGravity = false;
                }
            }

            if(mLadderUp != col.transform.up) {
                mLadderUp = col.transform.up;

                if(!M8.MathUtil.RotateToUp(mLadderUp, transform.right, transform.forward, ref mLadderRot))
                    transform.up = mLadderUp;
            }
        }
    }

    protected override void OnTriggerExit(Collider col) {
        base.OnTriggerExit(col);

        if(M8.Util.CheckLayerAndTag(col.gameObject, ladderLayer, ladderTag)) {
            mLadderCounter--;
        }

        if(!isOnLadder) {
            if(mGravityCtrl != null)
                mGravityCtrl.enabled = true;
            else
                rigidbody.useGravity = mLadderLastGravity;
        }
    }

    protected override void OnDestroy() {
        inputEnabled = false;

        base.OnDestroy();
    }

    protected override void OnDisable() {
        base.OnDisable();

        mEyeOrienting = false;
    }

    protected override void Awake() {
        base.Awake();

        mGravityCtrl = GetComponent<GravityController>();
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

            float moveX, moveY;

            if(moveNormalized) {
                Vector2 moveVec = new Vector2(
                    moveInputX != InputManager.ActionInvalid ? input.GetAxis(player, moveInputX) : 0.0f,
                    moveInputY != InputManager.ActionInvalid ? input.GetAxis(player, moveInputY) : 0.0f);

                moveVec.Normalize();

                moveX = moveVec.x;
                moveY = moveVec.y;

            }
            else {
                moveX = moveInputX != InputManager.ActionInvalid ? input.GetAxis(player, moveInputX) : 0.0f;
                moveY = moveInputY != InputManager.ActionInvalid ? input.GetAxis(player, moveInputY) : 0.0f;
            }

            //movement
            moveForward = 0.0f;
            moveSide = 0.0f;

            if(isOnLadder) {
                //move forward upwards
                Move(dirRot, Vector3.up, Vector3.right, new Vector2(moveX, moveY), moveForce);
            }
            else if(isUnderWater && !isGrounded) {
                //Move based on eye
                Move(_eye.rotation, Vector3.forward, Vector3.right, new Vector2(moveX, moveY), moveForce);
            }
            else if(!isSlopSlide) {
                moveForward = moveY;
                moveSide = moveX;
            }

            //look
            mLookCurInputAxis.x = lookInputX != InputManager.ActionInvalid ? input.GetAxis(player, lookInputX) : 0.0f;
            mLookCurInputAxis.y = lookInputY != InputManager.ActionInvalid ? input.GetAxis(player, lookInputY) : 0.0f;

            if(mLookCurInputAxis.x != 0.0f) {
                dirRot *= Quaternion.AngleAxis(mLookCurInputAxis.x * lookSensitivity, Vector3.up);
                dirHolder.rotation = dirRot;
            }

            //look vertical
            if(mEyeLocked && !mEyeOrienting) {
                float vDelta = mLookCurInputAxis.y * lookSensitivity;

                mLookCurRot += vDelta;

                if(mLookCurRot < -360.0f)
                    mLookCurRot += 360.0f;
                else if(mLookCurRot > 360.0f)
                    mLookCurRot -= 360.0f;

                mLookCurRot = Mathf.Clamp(mLookCurRot, lookYAngleMin, lookYAngleMax);
            }

            //jump
            if(mJump) {
                if(isOnLadder) {
                    body.AddForce(dirRot * Vector3.up * ladderJumpForce);
                }
                else if(isUnderWater) {
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
        if(_eye != null && mEyeLocked && !mEyeOrienting) {
            Quaternion rot = dirHolder.rotation;

            if(mLookCurRot != 0.0f)
                rot *= Quaternion.AngleAxis(lookYInvert ? mLookCurRot : -mLookCurRot, Vector3.right);

            _eye.rotation = rot;

            Vector3 pos = dirHolder.position;

            if(eyeOfs.x != 0.0f)
                pos += dirHolder.right * eyeOfs.x;

            if(eyeOfs.y != 0.0f)
                pos += dirHolder.up * eyeOfs.y;

            if(eyeOfs.z != 0.0f)
                pos += dirHolder.forward * eyeOfs.z;

            _eye.position = pos;
        }

        base.FixedUpdate();

        if(isOnLadder)
            rigidbody.drag = ladderDrag;
    }

    void OnInputJump(InputManager.Info dat) {
        if(dat.state == InputManager.State.Pressed) {
            if(!mJump) {
                if(isUnderWater || isOnLadder) {
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

    IEnumerator LadderOrientUp() {
        while(isOnLadder) {
            if(transform.up != mLadderUp) {
                float step = ladderOrientSpeed * Time.fixedDeltaTime;
                rigidbody.MoveRotation(Quaternion.RotateTowards(transform.rotation, mLadderRot, step));
            }

            yield return mWaitUpdate;
        }
    }

    IEnumerator EyeOrienting() {
        while(mEyeOrienting) {
            yield return mWaitUpdate;

            bool posDone = _eye.position == dirHolder.position;
            if(!posDone) {
                _eye.position = Vector3.SmoothDamp(_eye.position, dirHolder.position, ref mEyeOrientVel, eyeLockPositionDelay, Mathf.Infinity, Time.fixedDeltaTime);
            }

            bool rotDone = _eye.rotation == dirHolder.rotation;
            if(!rotDone) {
                float step = eyeLockOrientSpeed * Time.fixedDeltaTime;
                _eye.rotation = Quaternion.RotateTowards(_eye.rotation, dirHolder.rotation, step);
            }

            mEyeOrienting = !(posDone && rotDone);
        }
    }
}
