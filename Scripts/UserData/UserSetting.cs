using UnityEngine;

namespace M8 {
    public abstract class UserSetting<T> : SingletonBehaviour<T> where T : MonoBehaviour {
        [SerializeField]
        UserData _userData = null; //this is where to grab the settings, set to blank to grab from current gameObject

        [SerializeField]
        bool _loadOnInstance = true;

        public UserData userData { get { return _userData; } }

        public delegate void Callback(T us);

        public event Callback changeCallback;

        public abstract void Load();

        public virtual void Save() {
            userData.Save();
        }
        
        protected override void OnInstanceInit() {
            if(_loadOnInstance) {
                if(!userData) {
                    Debug.LogError("userData is null.");
                    return;
                }

                if(!userData.isLoaded)
                    userData.Load();

                Load();
            }
        }

        protected void RelaySettingsChanged() {
            if(changeCallback != null)
                changeCallback(this as T);
        }
    }
}