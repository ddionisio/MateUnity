using UnityEngine;

namespace M8 {
    public abstract class UserSetting<T> : SingletonBehaviour<T> where T : MonoBehaviour {
        [SerializeField]
        protected UserData userData; //this is where to grab the settings, set to blank to grab from current gameObject

        [SerializeField]
        bool _loadOnInstance = true;

        public delegate void Callback(T us);

        public event Callback changeCallback;

        public abstract void Load();

        public virtual void Save() {
            userData.Save();
        }

        protected override void OnInstanceInit() {
            if(userData == null)
                userData = GetComponent<UserData>();

            if(_loadOnInstance)
                Load();
        }

        protected void RelaySettingsChanged() {
            if(changeCallback != null)
                changeCallback(this as T);
        }
    }
}