using UnityEngine;
using System.Collections;

namespace M8 {
    [AddComponentMenu("M8/Game Object/PlatformActivator")]
    public class GOPlatformActivator : MonoBehaviour {

        [System.Serializable]
        public class DataPlatform {
            public RuntimePlatform platform;
            public GameObject go;
        }

        public DataPlatform[] platforms;

        void OnEnable() {
            DoIt();
        }

        void Awake() {
            DoIt();
        }

        private void DoIt() {
            foreach(DataPlatform dp in platforms) {
                dp.go.SetActive(dp.platform == Application.platform);
            }
        }
    }
}