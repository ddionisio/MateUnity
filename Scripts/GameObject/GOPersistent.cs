using UnityEngine;
using System.Collections;

namespace M8 {
    [AddComponentMenu("M8/Game Object/Persistent")]
    public class GOPersistent : MonoBehaviour {
        void Awake() {
            DontDestroyOnLoad(gameObject);
        }
    }
}