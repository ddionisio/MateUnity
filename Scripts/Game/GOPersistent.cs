using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/Game/GOPersistent")]
public class GOPersistent : MonoBehaviour {
    void Awake() {
        DontDestroyOnLoad(gameObject);
    }
}
