using UnityEngine;
using System.Collections;

namespace M8 {
    [AddComponentMenu("M8/Game Object/Persistent")]
    public class GOPersistent : MonoBehaviour {
        [SerializeField]
        bool _dontDestroyOnLoad = true;

        void Awake() {
            if(_dontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);
        }
    }
}