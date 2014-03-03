using UnityEngine;

[AddComponentMenu("M8/Core/UserSlotData")]
public class UserSlotData : UserData {
    public const string LocalizeParamUserName = "username";

    public const int MaxNameLength = 16;

    public const string PrefixKey = "usd";

    public int loadSlotOnStart = -1; //use for debug

    private int mSlot = -1;
    private string mName;

    public static int currentSlot { get { return instance ? ((UserSlotData)instance).mSlot : -1; } }

    public string slotName {
        get { return mName; }
        set {
            mName = !string.IsNullOrEmpty(value) ? value.Length > MaxNameLength ? value.Substring(0, MaxNameLength) : value : "";
        }
    }

    public int slot { get { return mSlot; } set { SetSlot(value, false); } }

    public void SetSlot(int slot, bool forceLoad) {
        if(mSlot != slot) {
            Save(); //save previous slot

            mSlot = slot;
            if(slot >= 0) {
                mKey = PrefixKey + mSlot;

                Load();
            }
        }
        else if(forceLoad)
            Load();
    }

    public static void CreateSlot(int slot, string name) {
        UserSlotData u = instance as UserSlotData;
        if(u) {
            if(IsSlotExist(slot))
                DeleteSlot(slot);

            u.SetSlot(slot, false);
            u.slotName = name;
            u.Save();
        }
    }

    public static bool IsSlotExist(int slot) {
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

    public static void DeleteValue(int slot, string key) {
        PlayerPrefs.DeleteKey(PrefixKey + slot + "_" + key);
    }

    public static bool HasSlotValue(int slot, string key) {
        return PlayerPrefs.HasKey(PrefixKey + slot + "_" + key);
    }

    public static void DeleteSlot(int slot) {
        PlayerPrefs.DeleteKey(PrefixKey + slot + "i");
        PlayerPrefs.DeleteKey(PrefixKey + slot + "name");

        //TODO: delete global slot values
    }

    public static void LoadSlot(int slot, bool forceLoad) {
        UserSlotData u = instance as UserSlotData;
        if(u) {
            u.SetSlot(slot, forceLoad);
        }
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

            mSlot = -1;
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
