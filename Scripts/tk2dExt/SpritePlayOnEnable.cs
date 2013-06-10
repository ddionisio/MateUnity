using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/tk2D/PlayOnEnable")]
public class SpritePlayOnEnable : MonoBehaviour {

    public tk2dSpriteAnimator sprite;

    public float minDelay;
    public float maxDelay;

    public bool stopOnDisable = true;

    private bool mStarted = false;

	void OnEnable() {
        if(mStarted)
            DoIt();
	}

    void OnDisable() {
        if(sprite != null)
            sprite.Stop();

        CancelInvoke("PlayDelayed");
    }
	
    void Awake() {
        if(sprite == null)
            sprite = GetComponent<tk2dSpriteAnimator>();

        sprite.playAutomatically = false;
    }

    void Start() {
        mStarted = true;
        DoIt();
    }

    void DoIt() {
        if(maxDelay > 0 || minDelay > 0) {
            if(minDelay < maxDelay)
                Invoke("PlayDelayed", Random.Range(minDelay, maxDelay));
            else
                Invoke("PlayDelayed", minDelay);
        }
        else {
            sprite.Play();
        }
    }
	
    void PlayDelayed() {
        sprite.Play();
    }
}
