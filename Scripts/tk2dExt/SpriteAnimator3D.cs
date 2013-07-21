using UnityEngine;
using System.Collections;

//determine dir based on angle between target's forward to camera's dir towards target
[AddComponentMenu("M8/tk2D/SpriteAnimator3D")]
public class SpriteAnimator3D : MonoBehaviour {

    public class DirData {
        public tk2dSpriteAnimationClip[] clips;
        public bool hFlip;
    }

    public tk2dSpriteAnimator anim; //animation to set the direction
    public Transform target; //this determines where we are actually facing

    public string cameraTag = "MainCamera"; //the camera that determines what direction is facing

    [System.Serializable]
    public class DirInfo {
        public string postfix;
        public bool hFlip;
    }

    public string[] clips;

    public DirInfo[] dirs;

    public string playOnStart;
    public bool playOnEnable; //if true, plays playOnStart during onEnable

    public System.Action<SpriteAnimator3D> animationCompletedCallback;

    public System.Action<SpriteAnimator3D, int> animationEventTriggeredCallback;
        
    private DirData[] mDirs; //each direction holds the clip we want
    private int mCurDir;
    private int mCurClip = -1;
    private Transform mCamTrans;
    private float mAngleOfs;
    private float mFPS;
    private bool mStarted = false;

    public int curDir { get { return mCurDir; } }

    public bool paused { get { return anim.Paused; } set { anim.Paused = value; } }

    /// <summary>
    /// Get the current clip data playing. Note: this will change as the camera position and orientation changes.
    /// </summary>
    public tk2dSpriteAnimationClip curClip { get { return mCurClip != -1 ? mDirs[mCurDir].clips[mCurClip] : null; } }

    public int curClipIndex { get { return mCurClip; } }

    public string curClipName { get { return mCurClip != -1 ? clips[mCurClip] : ""; } }

    public float fps { get { return mFPS; } set { mFPS = value; } }

    public bool IsPlaying(string clipName) {
        return mCurClip != -1 ? clipName == clips[mCurClip] : false;
    }

    public int GetClipIndex(string clipName) {
        for(int i = 0; i < clips.Length; i++) {
            if(clips[i] == clipName)
                return i;
        }
        return -1;
    }

    public void Play(string clipName, float startTime = 0.0f) {
        int clipInd = string.IsNullOrEmpty(clipName) ? -1 : GetClipIndex(clipName);
        if(clipInd != -1)
            Play(clipInd);
    }

    public void Play(int clipInd, float startTime = 0.0f) {
        mCurClip = clipInd;
        
        tk2dSpriteAnimationClip clip = mDirs[mCurDir].clips[clipInd];

        if(clip != null) {
            anim.Play(clip, startTime, mFPS);

            anim.Sprite.FlipX = mDirs[mCurDir].hFlip;
        }
    }

    public void Resume() {
        anim.Resume();
    }

    public void Pause() {
        anim.Pause();
    }

    public void Stop() {
        anim.Stop();
    }

    void OnDestroy() {
        animationCompletedCallback = null;
        animationEventTriggeredCallback = null;

    }

    void OnEnable() {
        if(mStarted && playOnEnable)
            Play(playOnStart);
    }

    void Awake() {
        mFPS = tk2dSpriteAnimator.DefaultFps;

        anim.AnimationCompleted = OnAnimCompleted;
        anim.AnimationEventTriggered = OnAnimationEventTriggered;

        if(target == null)
            target = transform;

        GameObject camGO = GameObject.FindGameObjectWithTag(cameraTag);
        if(camGO != null)
            mCamTrans = camGO.transform;
        else
            Debug.LogError("Can't find camera with tag: " + cameraTag);

        if(dirs.Length > 0) {
            mAngleOfs = (360.0f / ((float)dirs.Length)) * 0.5f;

            if(clips != null && clips.Length > 0) {
                Init(clips);
            }
        }
    }
        
    // Use this for initialization
    void Start() {
        mStarted = true;
        Play(playOnStart);    
    }

    // Update is called once per frame
    void Update() {
        //determine dir
        if(mCurClip != -1) {
            int newDir = GetDir();
            if(mCurDir != newDir) {
                mCurDir = newDir;

                Play(mCurClip, anim.ClipTimeSeconds);
            }
        }
    }

    void Init(string[] clipNames) {
        mDirs = new DirData[dirs.Length];
        for(int i = 0; i < mDirs.Length; i++) {
            mDirs[i] = new DirData();

            mDirs[i].clips = new tk2dSpriteAnimationClip[clipNames.Length];

            for(int c = 0; c < clipNames.Length; c++) {
                mDirs[i].clips[c] = anim.GetClipByName(clipNames[c] + dirs[i].postfix);
            }

            mDirs[i].hFlip = dirs[i].hFlip;
        }
    }

    int GetDir() {
        Vector3 v = target.position - mCamTrans.position;
        Vector3 f = Vector3.forward;

        v = target.worldToLocalMatrix.MultiplyVector(v);
        v.y = 0.0f;

        float s = M8.MathUtil.CheckSideSign(new Vector2(v.x, v.z), new Vector2(f.x, f.z));

        float angle = Vector3.Angle(f, v);

        if(s < 0.0f)
            angle = 360.0f - angle;

        angle += mAngleOfs;
        if(angle > 360.0f)
            angle -= 360.0f;

        return Mathf.RoundToInt((angle/360.0f)*(mDirs.Length-1));
    }

    void OnAnimCompleted(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip) {
        if(animationCompletedCallback != null)
            animationCompletedCallback(this);
    }

    void OnAnimationEventTriggered(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip, int frame) {
        if(animationEventTriggeredCallback != null)
            animationEventTriggeredCallback(this, frame);
    }
}
