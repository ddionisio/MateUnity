using UnityEngine;
using System.Collections;

/// <summary>
/// Make sure each clip is set to play Once
/// </summary>
[AddComponentMenu("M8/tk2D/AnimPeriodicPlay")]
public class SpriteAnimPeriodicPlay : MonoBehaviour {
    public tk2dSpriteAnimator target;

    public string[] clips;

    public string defaultClip;

    public float beginDelay;
    public float beginDelayRandAdd;

    public float periodDelay;
    public float periodDelayRandAdd;

    public bool playOnEnable;
    public bool shuffle = true;

    private bool mStarted;
    private bool mPlaying;

    private tk2dSpriteAnimationClip mDefaultClip;
    private tk2dSpriteAnimationClip[] mClips;
    private int mCurClipInd;
    private float mLastTime;
    private float mCurDelay;
    private bool mPlayNextClip;

    public void Play(bool begin) {
        if(!mPlaying) {
            mPlaying = true;

            if(begin)
                mCurDelay = beginDelay + Random.value*beginDelayRandAdd;

            if(begin && mCurDelay > 0) {
                target.Play(mDefaultClip);

                mLastTime = Time.time;
            }
            else {
                mLastTime = 0;
            }

            mPlayNextClip = true;
        }
    }

    public void Stop() {
        mCurClipInd = 0;
        mPlaying = false;
        target.Play(mDefaultClip);
    }

    void OnEnable() {
        target.AnimationCompleted += OnAnimComplete;

        if(mStarted && playOnEnable)
            Play(true);
    }

    void OnDisable() {
        if(target) {
            target.AnimationCompleted -= OnAnimComplete;

            Stop();
            target.Stop();
        }
    }

    void Awake() {
        if(target == null)
            target = GetComponent<tk2dSpriteAnimator>();

        mDefaultClip = target.GetClipByName(defaultClip);

        mClips = new tk2dSpriteAnimationClip[clips.Length];
        for(int i = 0; i < mClips.Length; i++) {
            mClips[i] = target.GetClipByName(clips[i]);
        }

        if(shuffle)
            M8.ArrayUtil.Shuffle(mClips);
    }

	// Use this for initialization
	void Start () {
        mStarted = true;
        if(playOnEnable)
            Play(true);
	}
	
	// Update is called once per frame
	void Update () {
        if(mPlaying) {
            if(mPlayNextClip && Time.time - mLastTime > mCurDelay) {
                target.Play(mClips[mCurClipInd]);
                mPlayNextClip = false;
            }
        }
	}

    void OnAnimComplete(tk2dSpriteAnimator anim, tk2dSpriteAnimationClip clip) {
        target.Play(mDefaultClip);

        mPlayNextClip = true;
        mLastTime = Time.time;
        mCurDelay = periodDelay + Random.value*periodDelayRandAdd;

        mCurClipInd++;
        if(mCurClipInd == mClips.Length) {
            mCurClipInd = 0;
            
            if(shuffle)
                M8.ArrayUtil.Shuffle(mClips);
        }
    }
}
