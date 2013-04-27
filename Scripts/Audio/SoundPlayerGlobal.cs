using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("M8/Audio/SoundPlayerGlobal")]
public class SoundPlayerGlobal : MonoBehaviour {
    public delegate void OnSoundEnd(object param);

    [System.Serializable]
    public class SoundData {
        public string name;
        public AudioClip clip;
        public float delay;
        public float volume = 1.0f;
        public bool loop;
    }

    public SoundData[] sfx;

    public bool dontDestroy = false;

    public int max = 10;

    private static SoundPlayerGlobal mInstance = null;

    private Dictionary<string, SoundData> mSfx;

    private int mNextId = 0;

    public static SoundPlayerGlobal instance { get { return mInstance; } }

    public GameObject Play(string name, OnSoundEnd onEndCallback = null, object onEndParam = null) {
        SoundData dat;

        GameObject ret = null;

        if(mSfx.TryGetValue(name, out dat)) {
            ret = GetAvailable();
            if(ret != null) {
                ret.audio.clip = dat.clip;
                ret.audio.volume = dat.volume;
                ret.audio.loop = dat.loop;

                SoundPlayer sp = ret.GetComponent<SoundPlayer>();
                sp.defaultVolume = dat.volume;
                sp.playDelay = dat.delay;

                sp.Play();

                if(!dat.loop)
                    sp.StartCoroutine(OnSoundPlayFinish(dat.clip.length + dat.delay, ret, onEndCallback, onEndParam));

                ret.SetActive(true);
            }
            /*else {
                Debug.LogWarning("Ran out of available sound player for: " + name);
            }*/
        }
        else {
            Debug.LogWarning("sound player not found: " + name);
        }

        return ret;
    }

    GameObject GetAvailable() {
        GameObject ret = null;

        Transform thisT = transform;

        for(int i = 0; i < max; i++) {
            Transform t = thisT.GetChild(mNextId);
            GameObject go = t.gameObject;

            if(!go.activeSelf) {
                ret = go;
                ret.SetActive(true);

                mNextId++; if(mNextId == max) mNextId = 0;
                break;
            }
            else {
                mNextId++; if(mNextId == max) mNextId = 0;
            }
        }

        return ret;
    }

    /// <summary>
    /// Call this with GameObject returned by Play, normally use this for looping clip
    /// </summary>
    public void Stop(GameObject go) {
        //in case parent is set elsewhere
        if(go.transform.parent != transform)
            go.transform.parent = transform;

        go.SetActive(false);
    }

    IEnumerator OnSoundPlayFinish(float delay, GameObject go, OnSoundEnd endCallback, object endParam) {
        yield return new WaitForSeconds(delay);

        Stop(go);

        if(endCallback != null)
            endCallback(endParam);
    }

    void OnDestroy() {
        if(mInstance == this)
            mInstance = null;
    }

    void Awake() {
        if(mInstance == null) {
            mInstance = this;

            if(dontDestroy)
                DontDestroyOnLoad(gameObject);

            if(max <= 0)
                max = sfx.Length;

            mSfx = new Dictionary<string, SoundData>(sfx.Length);
            foreach(SoundData sd in sfx)
                mSfx.Add(sd.name, sd);

            //generate pool
            for(int i = 0; i < max; i++) {
                CreateSource(i);
            }

            mNextId = 0;
        }
        else
            DestroyImmediate(gameObject);
    }
    
    private GameObject CreateSource(int ind) {
        GameObject go = new GameObject(ind.ToString(), typeof(AudioSource), typeof(SoundPlayer));
        go.transform.parent = transform;
                
        go.SetActive(false);

        return go;
    }
}
