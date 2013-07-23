using UnityEngine;
using System.Collections;

/// <summary>
/// Cheap projection without clipping. Set this as child for things like shadow blobs.
/// </summary>
[AddComponentMenu("M8/tk2D/SpriteProject")]
public class SpriteProject : MonoBehaviour {
    public const float checkDelay = 0.2f;

    public Transform source; //this is to determine direction to check and position
    public Vector3 sourceAxis = -Vector3.up; //the axis to use based on source's orientation
    public float offset = 0.0001f; //slightly move up from the plane

    public tk2dBaseSprite target; //the target projection

    public LayerMask layers;

    public float distance = 2.0f;

    public float fallOffStart = 1.0f;

    public Color fallOffColor = new Color(1.0f, 1.0f, 1.0f, 0.1f);

    public float fallOffScale = 0.3f;

    private Vector3 mOrigScale;
    private Color mOrigColor;

    private Vector3 mFarScale;

    private RaycastHit mHit;

    void OnDisable() {
        target.renderer.enabled = true;
        target.color = mOrigColor;
        target.scale = mOrigScale;
    }

    void Awake() {
        if(target == null)
            target = GetComponent<tk2dBaseSprite>();

        mOrigScale = target.scale;
        mOrigColor = target.color;

        mFarScale = mOrigScale * fallOffScale;
    }

    void Update() {
        Vector3 dir = source.rotation * sourceAxis;

        if(Physics.Raycast(source.position, dir, out mHit, distance, layers)) {
            target.renderer.enabled = true;

            target.transform.position = mHit.point + mHit.normal * offset;
            target.transform.forward = -mHit.normal;

            if(mHit.distance > fallOffStart) {
                float d = (mHit.distance - fallOffStart) / (distance - fallOffStart);
                target.color = Color.Lerp(mOrigColor, fallOffColor, d);
                target.scale = Vector3.Lerp(mOrigScale, mFarScale, d);
            }
            else {
                target.color = mOrigColor;
                target.scale = mOrigScale;
            }
        }
        else {
            target.renderer.enabled = false;
        }
    }
}
