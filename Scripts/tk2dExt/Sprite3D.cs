using UnityEngine;
using System.Collections;

//determine dir based on angle between target's forward to camera's dir towards target
[AddComponentMenu("M8/tk2D/Sprite3D")]
public class Sprite3D : MonoBehaviour {
    public enum State {
        Playing,
        Paused,
        Stopped,
        Invalid
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

    public class DirData {
        public tk2dSpriteAnimationClip[] clips;
        public bool hFlip;
    }

    private DirData[] mDirs; //each direction holds the clip we want
    private int mCurDir;
    private int mCurClip = -1;
    private State mState = State.Invalid;
    private Transform mCamTrans;
    private float mAngleOfs;

    public void Play(string clipName) {
        int clipInd = string.IsNullOrEmpty(clipName) ? -1 : GetClipInd(clipName);
        if(clipInd != -1)
            Play(clipInd);
    }

    public void Play(int clipInd) {
        mCurClip = clipInd;
        anim.Play(mDirs[mCurDir].clips[clipInd]);

        anim.Sprite.FlipX = mDirs[mCurDir].hFlip;

        mState = State.Playing;
    }

    public void Pause() {
        anim.Pause();
        mState = State.Paused;
    }

    public void Stop() {
        anim.Stop();
        mState = State.Stopped;
    }

    void Awake() {
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
        if(mState != State.Invalid)
            Play(playOnStart);
    }

    // Update is called once per frame
    void Update() {
        //determine dir
        switch(mState) {
            case State.Playing:
            case State.Paused:
                if(mCurClip != -1) {
                    int newDir = GetDir();
                    if(mCurDir != newDir) {
                        mCurDir = newDir;
                        Play(mCurClip);
                    }
                }
                break;
        }
    }

    int GetClipInd(string name) {
        for(int i = 0; i < clips.Length; i++) {
            if(clips[i] == name)
                return i;
        }
        return -1;
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

        mState = State.Stopped;
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
}
