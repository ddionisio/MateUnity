using UnityEngine;
using System.Collections;

namespace M8 {
    [AddComponentMenu("M8/Audio/SoundPlayerGlobalOnSelect")]
    public class SoundPlayerGlobalOnSelect : MonoBehaviour {
        public string id;

        public bool playAfterNextSelect; //prevents from playing when the ui first activates

        private bool mSelectPlayAfter;

        void OnEnable() {
        }

        void OnDisable() {
            mSelectPlayAfter = false;
        }

        void OnSelect(bool yes) {
            if(yes) {
                if(playAfterNextSelect) {
                    if(mSelectPlayAfter)
                        SoundPlayerGlobal.instance.Play(id);
                    else
                        mSelectPlayAfter = true;
                }
                else
                    SoundPlayerGlobal.instance.Play(id);
            }
        }
    }
}