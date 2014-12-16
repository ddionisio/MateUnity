using UnityEngine;
using System.Collections;

namespace M8 {
    /// <summary>
    /// This allows for individual axis to be set for scale, position, rotation
    /// </summary>
    [AddComponentMenu("M8/Transform/Property")]
    [ExecuteInEditMode]
    public class TransProperty : MonoBehaviour {

        public float localScaleX {
            get { return transform.localScale.x; }
            set {
                Vector3 s = transform.localScale;
                s.x = value;
                transform.localScale = s;
            }
        }

        public float localScaleY {
            get { return transform.localScale.y; }
            set {
                Vector3 s = transform.localScale;
                s.y = value;
                transform.localScale = s;
            }
        }

        public float localScaleZ {
            get { return transform.localScale.z; }
            set {
                Vector3 s = transform.localScale;
                s.z = value;
                transform.localScale = s;
            }
        }

        public float localPositionX {
            get { return transform.localPosition.x; }
            set {
                Vector3 s = transform.localPosition;
                s.x = value;
                transform.localPosition = s;
            }
        }

        public float localPositionY {
            get { return transform.localPosition.y; }
            set {
                Vector3 s = transform.localPosition;
                s.y = value;
                transform.localPosition = s;
            }
        }

        public float localPositionZ {
            get { return transform.localPosition.z; }
            set {
                Vector3 s = transform.localPosition;
                s.z = value;
                transform.localPosition = s;
            }
        }

        public float positionX {
            get { return transform.position.x; }
            set {
                Vector3 s = transform.position;
                s.x = value;
                transform.position = s;
            }
        }

        public float positionY {
            get { return transform.position.y; }
            set {
                Vector3 s = transform.position;
                s.y = value;
                transform.position = s;
            }
        }

        public float positionZ {
            get { return transform.position.y; }
            set {
                Vector3 s = transform.position;
                s.z = value;
                transform.position = s;
            }
        }

        public float localEulerX {
            get { return transform.localEulerAngles.x; }
            set {
                Vector3 s = transform.localEulerAngles;
                s.x = value;
                transform.localEulerAngles = s;
            }
        }

        public float localEulerY {
            get { return transform.localEulerAngles.y; }
            set {
                Vector3 s = transform.localEulerAngles;
                s.y = value;
                transform.localEulerAngles = s;
            }
        }

        public float localEulerZ {
            get { return transform.localEulerAngles.z; }
            set {
                Vector3 s = transform.localEulerAngles;
                s.z = value;
                transform.localEulerAngles = s;
            }
        }

        public float eulerX {
            get { return transform.eulerAngles.x; }
            set {
                Vector3 s = transform.eulerAngles;
                s.x = value;
                transform.eulerAngles = s;
            }
        }

        public float eulerY {
            get { return transform.eulerAngles.y; }
            set {
                Vector3 s = transform.eulerAngles;
                s.y = value;
                transform.eulerAngles = s;
            }
        }

        public float eulerZ {
            get { return transform.eulerAngles.z; }
            set {
                Vector3 s = transform.eulerAngles;
                s.z = value;
                transform.eulerAngles = s;
            }
        }
    }
}