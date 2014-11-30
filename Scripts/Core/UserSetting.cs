using UnityEngine;

public abstract class UserSetting : MonoBehaviour {
    [SerializeField]
    protected UserData userData; //this is where to grab the settings, set to blank to grab from current gameObject

    public delegate void Callback(UserSetting us);

    public event Callback changeCallback;
	
    public void Save() {
        userData.Save();
    }

    protected virtual void Awake() {
        if(userData == null)
            userData = GetComponent<UserData>();
    }

    protected void RelaySettingsChanged() {
        if(changeCallback != null)
            changeCallback(this);
    }
}
