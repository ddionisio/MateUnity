using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/2D/MatRepeatTileScrollScale")]
public class MatRepeatTileScrollScale : MonoBehaviour {
    public Renderer target;
    public Shader shader; //assumes M8/RepeatTileScroll
    public Texture texture;

    public float pixelPerUnit = 32.0f;

    [SerializeField]
    float _speedX;

    [SerializeField]
    float _speedY;

    [SerializeField]
    Color _color = Color.white;

    private Color mColor;
    private Transform mTrans = null;
    private Material mMat = null;
    private Vector2 mCurScale = Vector2.zero;
    private Vector2 mTextureSize = Vector2.zero;

    public Color defaultColor { get { return _color; } }
    public Color color {
        get { return mColor; }
        set {
            if(mColor != value) {
                mColor = value;
                mMat.SetColor("modColor", mColor);
            }
        }
    }

    void OnEnable() {
        mCurScale = Vector2.zero;
    }

    void Awake() {
        InitMat();
    }
    
    // Update is called once per frame
    void Update() {
        if(mMat != null) {
            Vector2 s = mTrans.localScale;
            if(mCurScale.x != s.x) {
                mMat.SetFloat("tileX", s.x * (pixelPerUnit / mTextureSize.x));
                mCurScale.x = s.x;
            }
            if(mCurScale.y != s.y) {
                mMat.SetFloat("tileY", s.y * (pixelPerUnit / mTextureSize.y));
                mCurScale.y = s.y;
            }
        }
    }

    void InitMat() {
        if(target == null)
            target = renderer;

        if(target != null && target.material != null && shader != null && texture != null) {
            mMat = new Material(shader);
            mMat.mainTexture = texture;

            mMat.SetFloat("speedX", _speedX);
            mMat.SetFloat("speedY", _speedY);
            mMat.SetColor("modColor", _color);

            mColor = _color;

            mTextureSize = new Vector2(texture.width, texture.height);

            target.material = mMat;

            mTrans = target.transform;
        }
    }
}
