using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("M8/Audio/MusicManager")]
public class MusicManager : MonoBehaviour {
    public enum AutoPlayType {
        None,
        Order,
        Shuffled
    }

	[System.Serializable]
	public class MusicData {
		public string name;
		public AudioSource source;
		public float loopDelay = 0.0f;
        public bool loop = true;
	}

    public delegate void OnMusicFinish(string curMusicName);
	
	public MusicData[] music;
	
	public float changeFadeOutDelay;
	
	public string playOnStart;

    public AutoPlayType autoPlay = AutoPlayType.None;

    /// <summary>
    /// Callback when music is done playing.  Make sure the audio source 'loop' is set to false
    /// </summary>
    public event OnMusicFinish musicFinishCallback;
	
	private static MusicManager mInstance = null;
	
	private enum State {
		None,
		Playing,
		Changing
	}
	
	private const double rate = 44100;
	
	private Dictionary<string, MusicData> mMusic;
	private float mCurTime = 0;
	
	private State mState = State.None;
	
	private MusicData mCurMusic;
	private MusicData mNextMusic;

    private bool mMusicEnable = false;
    private int mCurAutoplayInd = -1;

	public static MusicManager instance {
		get {
			return mInstance;
		}
	}
	
	public bool IsPlaying() {
		return mState == State.Playing;
	}
	
	public void Play(string name, bool immediate) {
        mMusicEnable = Main.instance.userSettings.isMusicEnable;

		if(immediate) {
			Stop(false);
		}
		
		if(mCurMusic == null || immediate) {
            mCurMusic = mMusic[name];
            mCurMusic.source.volume = mMusicEnable ? 1.0f : 0.0f;
            mCurMusic.source.Play();
            SetState(State.Playing);
		}
		else {
            mNextMusic = mMusic[name];
            SetState(State.Changing);
		}

        //determine index for auto playlist
        if(autoPlay != AutoPlayType.None) {
            for(int i = 0; i < music.Length; i++) {
                if(music[i].name == name) {
                    mCurAutoplayInd = i;
                    break;
                }
            }
        }
	}
	
	public void Stop(bool fade) {
		if(mState != State.None) {
			if(fade) {
				mNextMusic = null;
				SetState(State.Changing);
			}
			else {
				mCurMusic.source.Stop();
				SetState(State.None);
			}
		}
	}

    private void AutoPlaylistNext() {
        mMusicEnable = Main.instance.userSettings.isMusicEnable;

        Stop(false);

        mCurAutoplayInd++;
        if(mCurAutoplayInd >= music.Length)
            mCurAutoplayInd = 0;

        mCurMusic = music[mCurAutoplayInd];
        mCurMusic.source.volume = mMusicEnable ? 1.0f : 0.0f;
        mCurMusic.source.Play();
        SetState(State.Playing);
    }
	
	void OnDestroy() {
        if(mInstance == this)
		    mInstance = null;

        musicFinishCallback = null;
	}
	
	void Awake() {
        if(mInstance == null) {
            mInstance = this;

            mMusic = new Dictionary<string, MusicData>(music.Length);
            foreach(MusicData dat in music) {
                mMusic.Add(dat.name, dat);
            }

            if(autoPlay == AutoPlayType.Shuffled)
                M8.ArrayUtil.Shuffle(music);
        }
        else
            DestroyImmediate(gameObject);
	}

	// Use this for initialization
	void Start () {
        if(!string.IsNullOrEmpty(playOnStart)) {
            Play(playOnStart, true);
        }
        else if(autoPlay != AutoPlayType.None) {
            mCurAutoplayInd = -1;
            AutoPlaylistNext();
        }
	}

    void UserSettingsChanged(UserSettings us) {
        mMusicEnable = us.isMusicEnable;

        if(mCurMusic != null) {
            switch(mState) {
                case State.Playing:
                    mCurMusic.source.volume = us.isMusicEnable ? 1.0f : 0.0f;
                    break;
            }
        }
    }
	
	void SetState(State state) {
		mState = state;
		mCurTime = 0;

        if(mState == State.None)
            mCurMusic = null;
	}
	
	// Update is called once per frame
	void Update () {
		switch(mState) {
		case State.None:
			break;
		case State.Playing:
			if(!(mCurMusic.source.loop || mCurMusic.source.isPlaying)) {
                string curName = mCurMusic.name;

                if(autoPlay != AutoPlayType.None)
                    AutoPlaylistNext();
                else if(mCurMusic.loop) //loop
                    mCurMusic.source.Play((ulong)System.Math.Round(rate * ((double)mCurMusic.loopDelay)));
                else {
                    SetState(State.None);
                }

                //callback
                if(musicFinishCallback != null)
                    musicFinishCallback(curName);
			}
			break;
		case State.Changing:
			mCurTime += Time.deltaTime;
			if(mCurTime >= changeFadeOutDelay) {
				mCurMusic.source.Stop();
				
				if(mNextMusic != null) {
					mCurMusic.source.volume = mMusicEnable ? 1.0f : 0.0f;
					mNextMusic.source.Play();
					
					mCurMusic = mNextMusic;
					mNextMusic = null;
					
					SetState(State.Playing);
				}
				else {
					SetState(State.None);
				}
			}
			else {
				mCurMusic.source.volume = mMusicEnable ? 1.0f - mCurTime/changeFadeOutDelay : 0.0f;
			}
			break;
		}
	}
}
