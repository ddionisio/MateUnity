using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace M8.UI.Texts {
    /// <summary>
    /// Use this to display time format
    /// </summary>
    [AddComponentMenu("M8/UI/Texts/Time")]
    public class TextTime : MonoBehaviour {
        public enum Flags {
            None = 0,
            Seconds = 0x1,
            Minutes = 0x2,
            Hours = 0x4
        }

        public Text target;

        [EnumMask]
        public Flags displayFlags;

        public char separator = ':';

        /// <summary>
        /// Current time value in seconds
        /// </summary>
        public float time {
            get { return mTime; }
            set {
                if(mTime != value) {
                    mTime = value;
                    ApplyText();
                }
            }
        }

        private float mTime;
        private System.Text.StringBuilder mStringBuff = new System.Text.StringBuilder();
        
        void OnEnable() {
            ApplyText();
        }

        void Awake() {
            if(!target)
                target = GetComponent<Text>();
        }

        private void ApplyText() {
            mStringBuff.Clear();

            int seconds = Mathf.RoundToInt(mTime);
            int minutes = Mathf.RoundToInt(seconds / 60f);
            int hours = Mathf.RoundToInt(minutes / 60f);

            if((displayFlags & Flags.Hours) != Flags.None) {
                mStringBuff.Append(hours.ToString("D2"));
            }

            if((displayFlags & Flags.Minutes) != Flags.None) {
                if(mStringBuff.Length > 0) {
                    minutes %= 60;

                    mStringBuff.Append(separator);
                }

                mStringBuff.Append(minutes.ToString("D2"));
            }

            if((displayFlags & Flags.Seconds) != Flags.None) {
                if(mStringBuff.Length > 0) {
                    seconds %= 60;

                    mStringBuff.Append(separator);
                }

                mStringBuff.Append(seconds.ToString("D2"));
            }

            target.text = mStringBuff.ToString();
        }
    }
}