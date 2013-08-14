using UnityEngine;
using System.Collections;

/// <summary>
/// Automatically set the tile X/Y of target material based on the target object's scale X/Y
/// </summary>
[AddComponentMenu("M8/2D/MatAutoTileScale")]
public class MatAutoTileScale : MonoBehaviour {
    [SerializeField]
    Renderer _target;

    [SerializeField]
    Texture _texture; //optional to change texture

    public float pixelPerUnit = 32.0f;

    [SerializeField]
    Color _color = Color.white;

    private Color mColor;
    private Transform mTrans = null;
    private Material mMat = null;
    private Vector2 mCurScale = Vector2.zero;
    private Vector2 mTextureSize = Vector2.zero;

    public Texture texture { 
        get { return _texture; } 
        set {
            if(_texture != value) {
                _texture = value;

                mMat.mainTexture = _texture;
                mTextureSize = new Vector2(_texture.width, _texture.height);
                mCurScale = Vector2.zero;
            }
        } 
    }
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
        if(_target == null)
            _target = renderer;

        if(_target != null) {
            mMat = _target.material;

            if(_texture != null)
                mMat.mainTexture = _texture;
            else
                _texture = mMat.mainTexture;

            mMat.SetColor("modColor", _color);

            mColor = _color;

            mTextureSize = new Vector2(_texture.width, _texture.height);

            mTrans = _target.transform;
        }
    }
}
