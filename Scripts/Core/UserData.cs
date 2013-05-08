using UnityEngine;

[AddComponentMenu("M8/Core/UserData")]
public class UserData : MonoBehaviour {
    private static UserData mInstance = null;

    public static UserData instance { get { return mInstance; } }

    public virtual void Save() {
    }

    public virtual void Delete() {
        PlayerPrefs.DeleteAll();

        //resave stuff
        //TODO: might be better to broadcast it so anything that is not UserData related is resaved
        //instead of explicit calls
        if(Main.instance != null)
            Main.instance.userSettings.Save();
    }

    public virtual bool HasKey(string name) {
        return PlayerPrefs.HasKey(name);
    }

    public virtual int GetInt(string name, int defaultValue = 0) {
        return PlayerPrefs.GetInt(name, defaultValue);
    }

    public virtual void SetInt(string name, int value) {
        PlayerPrefs.SetInt(name, value);
    }

    public virtual float GetFloat(string name, float defaultValue = 0) {
        return PlayerPrefs.GetFloat(name, defaultValue);
    }

    public virtual void SetFloat(string name, float value) {
        PlayerPrefs.SetFloat(name, value);
    }

    public virtual string GetString(string name, string defaultValue = "") {
        return PlayerPrefs.GetString(name, defaultValue);
    }

    public virtual void SetString(string name, string value) {
        PlayerPrefs.SetString(name, value);
    }

    void OnDisable() {
        Save();
    }

    void OnDestroy() {
        if(mInstance == this)
            mInstance = null;
    }

    protected virtual void Awake() {
        if(mInstance == null)
            mInstance = this;
    }
}
