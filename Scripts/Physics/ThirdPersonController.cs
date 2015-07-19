using UnityEngine;
using System.Collections;

namespace M8 {
    [AddComponentMenu("M8/Physics/Third-Person Controller")]
    public class ThirdPersonController : RigidBodyController {
        [SerializeField]
        Transform _eye; //optional, can be set at runtime for cutscene, etc.

        public float eyeDistance;

        public float eyeMoveDelay;
        public float eyeLookAtDelay;

        public float lookPitchSpeed = 4.0f;
        public bool lookPitchInvert = false;
        public float lookPitchMin = -90.0f; //angle
        public float lookPitchMax = 90.0f;

        public float lookYawSpeed = 4.0f;
        public bool lookYawInvert = false;

        public int player = 0;
        public int moveInputX = InputManager.ActionInvalid;
        public int moveInputY = InputManager.ActionInvalid;
        public int lookInputPitch = InputManager.ActionInvalid;
        public int lookInputYaw = InputManager.ActionInvalid;

        public bool startInputEnabled = false;

        private bool mInputEnabled = false;

        private float mCurLookPitch;

        private Vector3 mCurEyeOrigin; //camera stand
        private Vector3 mCurEyePos; //camera destination

        private Vector3 mCurMoveForward; //the current forward movement dir, based on input

        private Coroutine mEyeFollowRoutine;

        public bool inputEnabled {
            get { return mInputEnabled; }
            set {
                mInputEnabled = value;
                if(mInputEnabled) {

                }
                else {

                }
            }
        }

        public Transform eye {
            get { return _eye; }
            set {
                _eye = value;

                InitEyeFollow();
            }
        }

        public Vector3 curMoveForward {
            get { return mCurMoveForward; }
        }

        public void ResetEye() {
            mCurLookPitch = 0f;

            mCurEyePos = mCurEyeOrigin = dirHolder.position - mCurMoveForward*eyeDistance;
        }

        protected override void OnDestroy() {
            inputEnabled = false;

            base.OnDestroy();
        }

        protected override void OnDisable() {
            if(mEyeFollowRoutine != null) {
                StopCoroutine(mEyeFollowRoutine);
                mEyeFollowRoutine = null;
            }

            base.OnDisable();
        }

        void Start() {
            inputEnabled = startInputEnabled;

            mCurMoveForward = dirHolder.forward;

            //initialize current eye origin
            ResetEye();

            InitEyeFollow();
        }
                
        protected override void FixedUpdate() {
            Vector3 outer = dirHolder.localPosition;
            Vector3 center = dirHolder.worldToLocalMatrix.MultiplyPoint3x4(mCurEyeOrigin);
            Vector2 ndir = new Vector2(outer.x - center.x, outer.z - center.z);

            //grab current velocity
            ComputeLocalVelocity(false);

            //rotate dir based on eye origin
            dirHolder.forward = dirHolder.localToWorldMatrix.MultiplyVector(new Vector3(ndir.x, 0f, ndir.y));

            //refresh current velocty based on rotation
            mBody.velocity = dirHolder.localToWorldMatrix.MultiplyVector(localVelocity);

            if(mInputEnabled) {
                InputManager input = InputManager.instance;

                float moveX, moveY;

                moveX = moveInputX != InputManager.ActionInvalid ? input.GetAxis(player, moveInputX) : 0.0f;
                moveY = moveInputY != InputManager.ActionInvalid ? input.GetAxis(player, moveInputY) : 0.0f;

                //determine dir rotation
                if(moveX != 0.0f) {
                    
                }

                if(!isSlopSlide) {
                    moveForward = moveY;
                    moveSide = moveX;
                }

                //update forward move dir
                mCurMoveForward = dirHolder.forward*moveY + dirHolder.right*moveX;
                mCurMoveForward.Normalize();
            }
            else {
                moveForward = 0.0f;
                moveSide = 0.0f;
            }

            base.FixedUpdate();

            //update eye position
            float ndirSqrMag = ndir.sqrMagnitude;
            if(ndirSqrMag != eyeDistance*eyeDistance) {

                mCurEyeOrigin = dirHolder.position - dirHolder.forward*eyeDistance;

                mCurEyePos = mCurEyeOrigin;
            }
        }

        void OnDrawGizmosSelected() {
            Gizmos.color = new Color(0f, 0.7f, 0f, 0.3f);
            Gizmos.DrawSphere(mCurEyeOrigin, 0.25f);
        }

        private void InitEyeFollow() {
            if(_eye) {
                if(mEyeFollowRoutine == null)
                    mEyeFollowRoutine = StartCoroutine(DoEyeFollow());
            }
        }

        IEnumerator DoEyeFollow() {
            Vector3 eyeVel = Vector3.zero, eyeLookVel = Vector3.zero;

            while(_eye) {
                float dt = Time.deltaTime;

                Vector3 pos = dirHolder.position;

                //move
                Vector3 destPos = mCurEyePos;
                Vector3 curPos = _eye.position;

                _eye.position = new Vector3(
                    Mathf.SmoothDamp(curPos.x, destPos.x, ref eyeVel.x, eyeMoveDelay, Mathf.Infinity, dt),
                    Mathf.SmoothDamp(curPos.y, destPos.y, ref eyeVel.y, eyeMoveDelay, Mathf.Infinity, dt),
                    Mathf.SmoothDamp(curPos.z, destPos.z, ref eyeVel.z, eyeMoveDelay, Mathf.Infinity, dt)
                    );

                //orient
                Vector3 destLook = pos - destPos; destLook.Normalize();
                Vector3 curLook = _eye.forward;

                //note: unity normalizes result
                _eye.forward = new Vector3(
                    Mathf.SmoothDamp(curLook.x, destLook.x, ref eyeLookVel.x, eyeLookAtDelay, Mathf.Infinity, dt),
                    Mathf.SmoothDamp(curLook.y, destLook.y, ref eyeLookVel.y, eyeLookAtDelay, Mathf.Infinity, dt),
                    Mathf.SmoothDamp(curLook.z, destLook.z, ref eyeLookVel.z, eyeLookAtDelay, Mathf.Infinity, dt)
                    );

                yield return null;
            }

            mEyeFollowRoutine = null;
        }
    }
}