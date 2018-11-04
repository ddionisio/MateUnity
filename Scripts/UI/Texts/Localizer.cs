using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace M8.UI.Texts {
    [AddComponentMenu("M8/UI/Texts/Localizer")]
    [RequireComponent(typeof(Text))]
    public class Localizer : MonoBehaviour {
        /// <summary>
        /// Localization key.
        /// </summary>
        [Localize]
        public string key;

        /// <summary>
        /// If set to true, only apply if key is available
        /// </summary>
        public bool isKeyExclusive;

        /// <summary>
        /// Set the parameters of the text for use with string format. If this is not null, string.Format is used.
        /// Make sure to call Localize again after changes to params
        /// </summary>
        public object[] parameters { get { return mParams; } set { mParams = value; } }

        public Text uiText { get; private set; }

        private bool mStarted = false;
        private object[] mParams = null;
        
        /// <summary>
        /// Localize the widget on enable, but only if it has been started already.
        /// </summary>

        void OnEnable() { Localize.instance.localizeCallback += Apply; if(mStarted) Apply(); }
        void OnDisable() { if(Localize.instance) Localize.instance.localizeCallback -= Apply; }

        /// <summary>
        /// Localize the widget on start.
        /// </summary>

        void Start() {
            mStarted = true;
            Apply();
        }

        /// <summary>
        /// Force-localize the widget.
        /// </summary>
        public void Apply() {
            if(uiText == null)
                uiText = GetComponent<Text>();

            if(isKeyExclusive) {
                //ensure key exists
                if(!string.IsNullOrEmpty(key) && Localize.instance.Exists(key))
                    ApplyToLabel(Localize.instance.GetText(key));
                else
                    uiText.text = "";
            }
            else {
                // If no localization key has been specified, use the label's text as the key
                if(string.IsNullOrEmpty(key)) key = uiText.text;

                // If we still don't have a key, use the widget's name
                string val = string.IsNullOrEmpty(key) ? Localize.instance.GetText(name) : Localize.instance.GetText(key);

                if(mParams != null)
                    val = string.Format(val, mParams);

                ApplyToLabel(val);
            }
        }

        protected virtual void ApplyToLabel(string text) {
            uiText.text = text;
        }
    }
}