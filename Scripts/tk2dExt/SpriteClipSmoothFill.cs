using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/tk2D/SpriteClipSmoothFill")]
public class SpriteClipSmoothFill : MonoBehaviour {
    public enum Mode {
        Once,
        Repeat,
        PingPong
    }

    public enum Type {
        X,
        Y,
        Both
    }

    public Mode mode = Mode.Once;
    public Type type = Type.X;
    public float start = 0.0f;
    public float end = 1.0f;
    public float delay = 1.0f;
    public bool playOnEnable = false;

    private bool mStarted;

    void OnEnable() {
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
