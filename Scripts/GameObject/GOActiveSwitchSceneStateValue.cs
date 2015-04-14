using UnityEngine;
using System.Collections;

namespace M8 {
    /// <summary>
    /// Use this to determine which game object is active based on scene state counter.
    /// Or the inverse, which game object is deactive based on scene state counter.
    /// Value has to be within the target array index
    /// </summary>
    [AddComponentMenu("M8/Game Object/Active Switch SceneState Value")]
    public class GOActiveSwitchSceneStateValue : MonoBehaviour {
        public string variable;
        public bool global;

        public bool setActive; // true = targets[value] is visible, others not. vise-versa

        public GameObject[] targets;

        private bool mStarted;

        void OnEnable() {
            if(mStarted) {
                Apply();
            }
        }

        // Use this for initialization
        void Start() {
            StartCoroutine(DoIt());
        }

        IEnumerator DoIt() {
            yield return new WaitForFixedUpdate();

            mStarted = true;

            Apply();

            yield break;
        }

        private void Apply() {
            int val = GetValue();

            for(int i = 0; i < targets.Length; i++) {
                targets[i].SetActive(i == val ? setActive : !setActive);
            }
        }

        private int GetValue() {
            return (global ? SceneState.instance.global : SceneState.instance.local).GetValue(variable);
        }
    }
}