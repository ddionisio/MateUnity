using UnityEngine;

public abstract class UserData : MonoBehaviour {
    private static UserData mInstance = null;

    public static UserData instance { get { return mInstance; } }

    public abstract void Save();

    public abstract void Delete();

    public abstract int GetInt(string name, int defaultValue = 0);

    public abstract void SetInt(string name, int value);

    void OnDisable() {
        Save();
    }

    void OnDestroy() {
        mInstance = null;
    }

    protected virtual void Awake() {
        mInstance = this;
    }
}
