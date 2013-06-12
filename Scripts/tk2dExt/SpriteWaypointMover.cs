using UnityEngine;
using System.Collections;

/// <summary>
/// Simple sprite controller when using waypoint mover
/// </summary>
[AddComponentMenu("M8/tk2D/SpriteWaypointMover")]
public class SpriteWaypointMover : MonoBehaviour {
    public WaypointMover target;

    public tk2dBaseSprite sprite;
    public tk2dSpriteAnimator anim;

    public bool isFacingLeft; //is the default facing of sprite left?

    public string moveAnim;
    public string idleAnim;

    private tk2dSpriteAnimationClip mMoveClip;
    private tk2dSpriteAnimationClip mIdleClip;

    void Awake() {
        if(target == null)
            target = GetComponent<WaypointMover>();

        target.moveBeginCallback += OnWaypointMove;
        target.movePauseCallback += OnWaypointPause;

        mMoveClip = anim.GetClipByName(moveAnim);
        mIdleClip = anim.GetClipByName(idleAnim);
    }
    
	// Use this for initialization
	void Start () {
        SetAnimToIdle();
	}

    void SetAnimToIdle() {
        if(anim != null) {
            if(mIdleClip != null)
                anim.Play(mIdleClip);
            else if(mMoveClip != null)
                anim.Play(mMoveClip);
        }
    }

    void SetAnimToMove() {
        if(anim != null) {
            if(mMoveClip != null)
                anim.Play(mMoveClip);
        }
    }

    void OnWaypointMove(WaypointMover wm) {
        SetAnimToMove();

        bool isLeft = wm.dir.x < 0.0f;

        sprite.FlipX = isFacingLeft ? !isLeft : isLeft;
    }

    void OnWaypointPause(WaypointMover wm) {
        SetAnimToIdle();
    }
}
