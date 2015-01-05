using UnityEngine;

namespace M8 {
    public abstract class UserSetting<T> : SingletonBehaviour<T> where T : MonoBehaviour {
        [SerializeField]
        protected UserData userData; //this is where to grab the settings, set to blank to grab from current gameObject

        public delegate void Callback(T us);

        public event Callback changeCallback;

        public void Save() {
            userData.Save();
        }

        protected override void Awake() {
            base.Awake();

            if(userData == null)
                userData = GetComponent<UserData>();
        }

        protected void RelaySettingsChanged() {
            if(changeCallback != null)
                changeCallback(this as T);
        }
    }
}