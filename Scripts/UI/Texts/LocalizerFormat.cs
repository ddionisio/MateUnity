using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace M8.UI.Texts {
    [AddComponentMenu("M8/UI/Texts/Localizer Format")]
    [RequireComponent(typeof(Text))]
    public class LocalizerFormat : Localizer {
        public string format = "{0}";

        protected override void ApplyToLabel(string text) {
            uiText.text = string.Format(format, text);
        }
    }
}