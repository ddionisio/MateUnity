using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/Game Object/Persistent")]
public class GOPersistent : MonoBehaviour {
    void Awake() {
        DontDestroyOnLoad(gameObject);
    }
}
