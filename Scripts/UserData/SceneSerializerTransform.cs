using UnityEngine;
using System.Collections;

namespace M8 {
    [AddComponentMenu("M8/Serializer/Transform")]
    public class SceneSerializerTransform : MonoBehaviour {
        [System.Flags]
        public enum Axis {
            X=0x1,
            Y=0x2,
            Z=0x4
        }

        public SceneSerializer serializer;

        public bool isLocal;

        public Axis positionFlags = Axis.X | Axis.Y | Axis.Z;
        public Axis rotationFlags = Axis.X | Axis.Y | Axis.Z;

        private UserData mUserData;

        void OnDestroy() {
            if(serializer) {
                serializer.loadedCallback -= OnUserDataLoaded;
                serializer.saveCallback -= OnUserDataSave;
            }
        }

        void Awake() {
            if(!serializer)
                serializer = GetComponent<SceneSerializer>();

            if(serializer.isLoaded)
                OnUserDataLoaded();

            serializer.loadedCallback += OnUserDataLoaded;
            serializer.saveCallback += OnUserDataSave;
        }

        void OnUserDataLoaded() {
            Vector3 pos = isLocal ? transform.localPosition : transform.position;
            if((positionFlags & Axis.X) != 0) pos.x = serializer.GetFloat("px", pos.x);
            if((positionFlags & Axis.Y) != 0) pos.y = serializer.GetFloat("py", pos.y);
            if((positionFlags & Axis.Z) != 0) pos.z = serializer.GetFloat("pz", pos.z);

            Vector3 rot = isLocal ? transform.localEulerAngles : transform.eulerAngles;
            if((rotationFlags & Axis.X) != 0) rot.x = serializer.GetFloat("rx", rot.x);
            if((rotationFlags & Axis.Y) != 0) rot.y = serializer.GetFloat("ry", rot.y);
            if((rotationFlags & Axis.Z) != 0) rot.z = serializer.GetFloat("rz", rot.z);

            if(isLocal) {
                transform.localPosition = pos;
                transform.localEulerAngles = rot;
            }
            else {
                transform.position = pos;
                transform.eulerAngles = rot;
            }
        }

        void OnUserDataSave() {
            Vector3 pos = isLocal ? transform.localPosition : transform.position;
            if((positionFlags & Axis.X) != 0) serializer.SetFloat("px", pos.x);
            if((positionFlags & Axis.Y) != 0) serializer.SetFloat("py", pos.y);
            if((positionFlags & Axis.Z) != 0) serializer.SetFloat("pz", pos.z);

            Vector3 rot = isLocal ? transform.localEulerAngles : transform.eulerAngles;
            if((rotationFlags & Axis.X) != 0) serializer.SetFloat("rx", rot.x);
            if((rotationFlags & Axis.Y) != 0) serializer.SetFloat("ry", rot.y);
            if((rotationFlags & Axis.Z) != 0) serializer.SetFloat("rz", rot.z);
        }
    }
}