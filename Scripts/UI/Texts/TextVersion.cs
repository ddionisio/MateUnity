using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace M8.UI.Texts {
    /// <summary>
    /// Use this to display the version from Unity project settings.
    /// </summary>
    [AddComponentMenu("M8/UI/Texts/Version")]
    public class TextVersion : MonoBehaviour {
        public Text target;

        public void Apply() {
            if(!target)
                target = GetComponent<Text>();

            if(target)
                target.text = Application.version;
        }

        void Awake() {
            Apply();
        }
    }
}
