using UnityEngine;

[AddComponentMenu("M8/Core/UserSlotData")]
public class UserSlotData : UserData {
    public const string LocalizeParamUserName = "username";

    public const int MaxNameLength = 16;

    public const string PrefixKey = "usd";

    public int loadSlotOnStart = -1; //use for debug

    private int mSlot = -1;
    private string mName;

    public string slotName {
        get { return mName; }
        set {
            mName = !string.IsNullOrEmpty(value) ? value.Length > MaxNameLength ? value.Substring(0, MaxNameLength) : value : "";
        }
    }

    public int curSlot { get { return mSlot; } }

    public void SetSlot(int slot, bool forceLoad) {
        if(mSlot != slot) {
            Save(); //save previous slot

            mSlot = slot;
            mKey = PrefixKey + mSlot;

            Load();
        }
        else if(forceLoad)
            Load();
    }

    public static bool IsSlotAvailable(int slot) {
        return PlayerPrefs.HasKey(PrefixKey + slot + "name");
    }

    public static string GetSlotName(int slot) {
        return PlayerPrefs.GetString(PrefixKey + slot + "name", "");
    }

    //individual unique value outside of setting the slot
    //use this to have information available before loading a slot (e.g. during slot select)
    public static int GetSlotValueInt(int slot, string key, int defaultVal = 0) {
        return PlayerPrefs.GetInt(PrefixKey + slot + "_" + key, defaultVal);
    }

    public static void SetSlotValueInt(int slot, string key, int val) {
        PlayerPrefs.SetInt(PrefixKey + slot + "_" + key, val);
    }

    public static float GetSlotValueFloat(int slot, string key, float defaultVal = 0.0f) {
        return PlayerPrefs.GetFloat(PrefixKey + slot + "_" + key, defaultVal);
    }
    
    public static void SetSlotValueFloat(int slot, string key, float val) {
        PlayerPrefs.GetFloat(PrefixKey + slot + "_" + key, val);
    }

    public static void DeleteSlot(int slot) {
        PlayerPrefs.DeleteKey(PrefixKey + slot + "i");
        PlayerPrefs.DeleteKey(PrefixKey + slot + "name");

        //TODO: delete global slot values
    }

    public override void Load() {
        if(mSlot != -1) {
            //name
            mName = PlayerPrefs.GetString(PrefixKey + mSlot + "name", "");

            base.Load();
        }
    }

    public override void Save() {
        if(mSlot != -1) {
            PlayerPrefs.SetString(PrefixKey + mSlot + "name", mName);

            base.Save();
        }
    }

    /// <summary>
    /// Make sure to set a new slot after this.
    /// </summary>
    public override void Delete() {
        if(mSlot != -1) {
            DeleteSlot(mSlot);

            base.Delete();
        }
    }

    protected override void Awake() {
        base.Awake();

        GameLocalize.RegisterParam(LocalizeParamUserName, OnGameLocalizeParamName);
    }

    protected override void Start() {
        if(loadOnStart && loadSlotOnStart != -1) {
            SetSlot(loadSlotOnStart, false);
        }
    }

    string OnGameLocalizeParamName(string key) {
        return mName;
    }
}
