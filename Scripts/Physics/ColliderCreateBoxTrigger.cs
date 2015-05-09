using UnityEngine;
using System.Collections;

namespace M8 {
    /// <summary>
    /// Generate a box trigger from collider
    /// </summary>
    [AddComponentMenu("M8/Physics/ColliderCreateBoxTrigger")]
    public class ColliderCreateBoxTrigger : MonoBehaviour {
        public enum Anchor {
            Top,
            Bottom,
            Left,
            Right,
            Front,
            Back,
            Center
        }

        public enum DimType {
            Absolute, //val
            Relative, //size - val
            RelativeScale, //size*val
        }

        public Collider target; //if null, use gameobject's collider
        public Anchor anchor = Anchor.Top;

        public Vector3 size = Vector3.zero;
        public DimType sizeXType = DimType.Relative;
        public DimType sizeYType = DimType.Relative;
        public DimType sizeZType = DimType.Relative;

        private BoxCollider mColl;

        void Awake() {
            Vector3 c;
            Vector3 s;

            Generate(target ? target : GetComponent<Collider>(), out c, out s);

            mColl = gameObject.AddComponent<BoxCollider>();
            mColl.isTrigger = true;
            mColl.center = c;
            mColl.size = s;
        }

        float GenerateSize(DimType t, float srcS, float val) {
            switch(t) {
                case DimType.Relative:
                    return srcS - val;
                case DimType.RelativeScale:
                    return srcS*val;
                default:
                    return val;
            }
        }

        void Generate(Collider fromColl, out Vector3 c, out Vector3 s) {
            c = Vector3.zero;
            s = size;

            if(fromColl) {
                Vector3 bcLocal;
                Vector3 bsLocal;

                BoxCollider box = fromColl as BoxCollider;
                if(box) {
                    bcLocal = box.center;
                    bsLocal = box.size;
                }
                else {
                    Transform t = fromColl.transform;
                    Matrix4x4 WToL = t.worldToLocalMatrix;

                    Bounds b = fromColl.bounds;

                    bcLocal = WToL.MultiplyPoint3x4(b.center);
                    bsLocal = WToL.MultiplyVector(b.size);
                }

                s.x = GenerateSize(sizeXType, bsLocal.x, size.x);
                s.y = GenerateSize(sizeYType, bsLocal.y, size.y);
                s.z = GenerateSize(sizeZType, bsLocal.z, size.z);

                switch(anchor) {
                    case Anchor.Top:
                        c = new Vector3(bcLocal.x, bcLocal.y + bsLocal.y*0.5f + s.y*0.5f, bcLocal.z);
                        break;
                    case Anchor.Bottom:
                        c = new Vector3(bcLocal.x, bcLocal.y - bsLocal.y*0.5f - s.y*0.5f, bcLocal.z);
                        break;
                    case Anchor.Left:
                        c = new Vector3(bcLocal.x - bsLocal.x*0.5f - s.x*0.5f, bcLocal.y, bcLocal.z);
                        break;
                    case Anchor.Right:
                        c = new Vector3(bcLocal.x + bsLocal.x*0.5f + s.x*0.5f, bcLocal.y, bcLocal.z);
                        break;
                    case Anchor.Front:
                        c = new Vector3(bcLocal.x, bcLocal.y, bcLocal.z + bsLocal.z*0.5f + s.z*0.5f);
                        break;
                    case Anchor.Back:
                        c = new Vector3(bcLocal.x, bcLocal.y, bcLocal.z - bsLocal.z*0.5f - s.z*0.5f);
                        break;
                    default:
                        c = bcLocal;
                        break;
                }
            }
        }

        void OnDrawGizmosSelected() {
            Vector3 c;
            Vector3 s;

            Collider coll = target ? target : GetComponent<Collider>();
            if(coll) {
                Generate(coll, out c, out s);

                Gizmos.color = new Color(0f, 1f, 0f, 0.75f);
                M8.Gizmo.DrawWireCube(transform, c, s);
            }
        }
    }
}