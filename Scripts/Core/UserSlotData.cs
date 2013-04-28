using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[AddComponentMenu("M8/Core/UserSlotData")]
public class UserSlotData : UserData {
    public const string UserDataLoadCallName = "OnUserDataLoad";
    public const string LocalizeParamUserName = "username";

    public const int MaxNameLength = 16;
    public const string PrefixKey = "usd";

    public int loadSlotOnStart = -1; //use for debug

    private int mSlot = -1;
    private string mName;
    private Dictionary<string, int> mValueIs = null;

    public string slotName {
        get { return mName; }
        set {
            mName = !string.IsNullOrEmpty(value) ? value.Length > MaxNameLength ? value.Substring(0, MaxNameLength) : value : "";
        }
    }

    public int curSlot { get { return mSlot; } }

    public void SetSlot(int slot, bool forceLoad) {
        if(mSlot != slot || forceLoad) {
            Save(); //save previous slot

            mSlot = slot;

            //integers
            string dat = PlayerPrefs.GetString(PrefixKey + mSlot + "i", "");
            if(!string.IsNullOrEmpty(dat)) {
                BinaryFormatter bf = new BinaryFormatter();
                MemoryStream ms = new MemoryStream(Convert.FromBase64String(dat));
                mValueIs = (Dictionary<string, int>)bf.Deserialize(ms);
            }
            else {
                mValueIs = new Dictionary<string, int>(0);
            }

            //name
            mName = PlayerPrefs.GetString(PrefixKey + mSlot + "name", "");

            SceneManager.RootBroadcastMessage(UserDataLoadCallName, this, SendMessageOptions.DontRequireReceiver);
        }
    }

    public static bool IsSlotAvailable(int slot) {
        return PlayerPrefs.HasKey(PrefixKey + slot + "name");
    }

    public static string GetSlotName(int slot) {
        return PlayerPrefs.GetString(PrefixKey + slot + "name", "");
    }

    //individual unique value outside of setting the slot
    public static int GetSlotValueInt(int slot, string key, int defaultVal = 0) {
        return PlayerPrefs.GetInt(PrefixKey + slot + "_" + key, defaultVal);
    }

    public static void SetSlotValueInt(int slot, string key, int val) {
        PlayerPrefs.SetInt(PrefixKey + slot + "_" + key, val);
    }

    public static void DeleteSlot(int slot) {
        PlayerPrefs.DeleteKey(PrefixKey + slot + "i");
        PlayerPrefs.DeleteKey(PrefixKey + slot + "name");
    }

    public override void Save() {
        if(mSlot != -1) {
            if(mValueIs != null) {
                BinaryFormatter bf = new BinaryFormatter();
                MemoryStream ms = new MemoryStream();
                bf.Serialize(ms, mValueIs);
                PlayerPrefs.SetString(PrefixKey + mSlot + "i", Convert.ToBase64String(ms.GetBuffer()));
            }

            PlayerPrefs.SetString(PrefixKey + mSlot + "name", mName);
        }
    }

    /// <summary>
    /// Make sure to set a new slot after this.
    /// </summary>
    public override void Delete() {
        if(mSlot != -1) {
            DeleteSlot(mSlot);
            mSlot = -1;
            mValueIs = null;
        }
    }

    public override bool HasKey(string name) {
        //TODO: add check to other value containers
        return mValueIs != null && mValueIs.ContainsKey(name);
    }

    /// <summary>
    /// This will get given name from current user data.  Make sure data has been loaded beforehand.
    /// </summary>
    public override int GetInt(string name, int defaultValue = 0) {
        int dat;
        if(mValueIs != null && mValueIs.TryGetValue(name, out dat))
            return dat;

        return defaultValue;
    }

    /// <summary>
    /// This will set given name to current user data. Make sure data has been loaded beforehand.
    /// </summary>
    public override void SetInt(string name, int value) {
        mValueIs[name] = value;
    }

    public override string GetString(string name, string defaultValue = "") {
        Debug.LogError("Not yet implemented!!!");
        return defaultValue;
    }

    public override void SetString(string name, string value) {
        Debug.LogError("Not yet implemented!!!");
    }

    protected override void Awake() {
        base.Awake();

        GameLocalize.RegisterParam(LocalizeParamUserName, OnGameLocalizeParamName);
    }

    void Start() {
        if(loadSlotOnStart != -1) {
            SetSlot(loadSlotOnStart, false);
        }
    }

    string OnGameLocalizeParamName() {
        return mName;
    }
}
