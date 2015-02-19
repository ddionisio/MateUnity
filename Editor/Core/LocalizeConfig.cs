using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace M8 {
    public class LocalizeConfig : EditorWindow {
        public const string PrefKlass = "Localize";
        public const string PrefFileMapper = "localize";

        public const float valueTextAreaMinHeight = 150f;
        public const float valueTextAreaMaxHeight = 500f;

        const string editKeyControl = "EditKey";
        const string editItemControl1 = "EditItem1";
        const string editItemControl2 = "EditItem2";

        public enum Mode {
            EditPaths,
            EditItems,
            SelectComponent
        }

        public class Data {
            public string value = "";
            public string[] param;

            //editor specifics
            public bool paramIsRef = false; //if true, parameters are set to null upon save.
        }

        public struct ItemHeader {
            public Language language;
            public bool isPlatform;
            public RuntimePlatform platform;

            public TextAsset text;

            public bool IsEqualTo(ItemHeader other) {
                return IsEqualTo(other.language, other.isPlatform, other.platform);
            }

            public bool IsEqualTo(Language aLanguage, bool aIsPlatform, RuntimePlatform aPlatform) {
                return language == aLanguage && isPlatform == aIsPlatform && (!isPlatform || platform == aPlatform);
            }

            public int CompareTo(ItemHeader other) {
                if(language == other.language) {
                    return isPlatform ? other.isPlatform ? (int)platform - (int)other.platform : 1 : -1;
                }
                else {
                    if(language == Language.Default) return -1;
                    else if(other.language == Language.Default) return 1;

                    return (int)language - (int)other.language;
                }
            }
        }

        public class Item {
            public ItemHeader header;

            public bool changed; //if true, this item has some modified values
            public bool paramFoldout = true;

            public Dictionary<string, Data> items;

            public string GenerateNewItem(ref int index) {
                if(items == null)
                    items = new Dictionary<string, Data>();

                string ret = "";

                do {
                    ret = string.Format("key_{0:000}", index);
                    index++;
                } while(items.ContainsKey(ret));

                changed = true;

                items.Add(ret, new Data());

                return ret;
            }

            public void AddNewItem(string newKey) {
                if(items == null)
                    items = new Dictionary<string, Data>();

                changed = true;

                items.Add(newKey, new Data());
            }
        }

        public class ItemComparer : IComparer<Item> {
            public int Compare(Item itm1, Item itm2) {
                if(itm1 == null)
                    return itm2 == null ? 0 : -1;
                else {
                    if(itm2 == null) return 1;

                    return itm1.header.CompareTo(itm2.header);
                }
            }
        }

        private static Localize mLocalize;

        private static Item mEditItemBase;
        private static string[] mEditItemBaseKeyTexts;
        private static int[] mEditItemBaseKeyInds;
        private static int mEditItemBaseKeyInd;

        private Mode mMode;

        private string[] mModeTexts = new string[] { "Edit Paths", "Edit Items", "Select Component" };
        private int[] mModeInds = new int[] { 0, 1, 2 };

        private Vector2 mEditPathsScroll;

        private List<Item> mEditItems = new List<Item>();
        private ItemComparer mEditItemComparer = new ItemComparer();

        private string[] mEditItemTexts;
        private int[] mEditItemInds;
        private int mEditItemInd;

        private List<ItemHeader> mEditItemLoads = new List<ItemHeader>(); //items that need loading

        private Vector2 mEditItemsScroll;
        private bool mEditItemsLocalizeFoldout;
        private string mEditItemsKeyText; //when editing key
        private bool mEditItemsKeyEdit;
        private bool mEditItemsKeyEditIsNew;

        private string mEditItemParamText = "";
        private string mEditItemLocalizeParamText = "";

        private int mEditItemsCurKeyGen;

        private TextEditor mTE = new TextEditor();

        public static LocalizeConfig Open(Localize localize) {
            LocalizeConfig win = EditorWindow.GetWindow(typeof(LocalizeConfig)) as LocalizeConfig;

            if(localize) {
                if(mLocalize != localize && SaveLocalizeObjectPath(localize))
                    mLocalize = localize;
            }
            else if(!mLocalize) {
                //see if we can load from previous
                mLocalize = LoadLocalizeObjectFromPath();
            }

            if(mLocalize) {
                //check if base path is already set
                if(mLocalize.baseFile)
                    win.mMode = Mode.EditItems;
                else
                    win.mMode = Mode.EditPaths;

                win.LoadAllItems();

                GenerateBaseKeyItems();

                win.InitCurrentMode();
            }
            else {
                //ask to select component
                win.mMode = Mode.SelectComponent;
            }

            return win;
        }

        public static string DrawSelector(string key) {
            //load localize if not yet set
            if(!mLocalize) {
                mLocalize = LoadLocalizeObjectFromPath();

                if(mLocalize) {
                    mEditItemBase = new Item { header=new ItemHeader { text=mLocalize.baseFile, language=Language.Default, isPlatform=false } };
                    LoadItem(mEditItemBase);
                }
                else { //no localize setup
                    if(GUILayout.Button("Configure Localization"))
                        Open(null);

                    Color prevClr = GUI.color;
                    GUI.color = Color.red;
                    GUILayout.Label("Localization is not found.", GUI.skin.box);
                    GUI.color = prevClr;

                    return "";
                }
            }

            //get current index
            int selectInd = 0;
            for(int i = 0; i < mEditItemBaseKeyTexts.Length; i++) {
                if(mEditItemBaseKeyTexts[i] == key) {
                    selectInd = i;
                    break;
                }
            }

            //selection
            GUILayout.BeginHorizontal();

            selectInd = EditorGUILayout.IntPopup(selectInd, mEditItemBaseKeyTexts, mEditItemBaseKeyInds);

            if(EditorExt.Utility.DrawSimpleButton("E", "Configure localization.")) {
                Open(null);

                mEditItemBaseKeyInd = selectInd;
            }

            GUILayout.EndHorizontal();

            return mEditItemBaseKeyTexts[selectInd];
        }

        public static string GetBaseValue(string key) {
            if(!mLocalize) {
                mLocalize = LoadLocalizeObjectFromPath();

                if(mLocalize) {
                    mEditItemBase = new Item { header=new ItemHeader { text=mLocalize.baseFile, language=Language.Default, isPlatform=false } };
                    LoadItem(mEditItemBase);
                }
                else //no localize setup
                    return "";
            }

            //grab value
            Data dat = null;
            return mEditItemBase != null && mEditItemBase.items.TryGetValue(key, out dat) ? dat.value : "";
        }

        [MenuItem("M8/Localize")]
        static void DoIt() {
            Open(null);
        }

        static bool SaveLocalizeObjectPath(Localize l) {
            if(l) {
                PrefabType prefabType = PrefabUtility.GetPrefabType(l.gameObject);
                switch(prefabType) {
                    case PrefabType.Prefab:
                    case PrefabType.PrefabInstance:
                        Object obj = PrefabUtility.GetPrefabObject(l.gameObject);
                        if(obj)//save path
                            EditorPrefs.SetString(EditorExt.Utility.PreferenceKey(PrefKlass, PrefFileMapper), AssetDatabase.GetAssetPath(obj));
                        else
                            Debug.LogWarning(string.Format("{0} needs to be from a prefab or prefab instance.", l.name));
                        return true;
                    default:
                        Debug.LogWarning(string.Format("{0} needs to be from a prefab or prefab instance.", l.name));
                        break;
                }
            }

            return false;
        }

        static Localize LoadLocalizeObjectFromPath() {
            string path = EditorPrefs.GetString(EditorExt.Utility.PreferenceKey(PrefKlass, PrefFileMapper), "");
            if(!string.IsNullOrEmpty(path)) {
                GameObject go = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
                if(go != null) {
                    Localize[] locs = go.GetComponentsInChildren<Localize>(true);
                    return locs.Length > 0 ? locs[0] : null;
                }
            }

            return null;
        }

        static void GenerateBaseKeyItems() {
            if(mEditItemBase != null) {
                mEditItemBaseKeyTexts = new string[mEditItemBase.items.Count];
                mEditItemBaseKeyInds = new int[mEditItemBase.items.Count];

                int i = 0;
                foreach(var pair in mEditItemBase.items) {
                    mEditItemBaseKeyTexts[i] = pair.Key;
                    mEditItemBaseKeyInds[i] = i;
                    i++;
                }

                System.Array.Sort(mEditItemBaseKeyTexts);
            }

            mEditItemBaseKeyInd = 0;
        }

        static void LoadItem(Item item) {
            //clear up items
            item.items = new Dictionary<string, Data>();

            //load up the entries
            string textData = item.header.text.text;
            if(!string.IsNullOrEmpty(textData)) {
                fastJSON.JSON.Parameters.UseExtensions = false;

                try {
                    List<Localize.Entry> tableEntries = fastJSON.JSON.ToObject<List<Localize.Entry>>(textData);
                    foreach(var entry in tableEntries)
                        item.items.Add(entry.key, new Data { value=entry.text, param=entry.param, paramIsRef=entry.param==null });
                }
                catch(System.Exception e) {
                    Debug.LogError(e.ToString());
                }
            }

            //generate base keys
            if(item == mEditItemBase)
                GenerateBaseKeyItems();

            item.changed = false;
        }

        void OnEnable() {
        }

        void OnGUI() {
            //mode select
            Mode prevMode = mMode;

            if(mLocalize) {
                mMode = (Mode)EditorGUILayout.IntPopup((int)mMode, mModeTexts, mModeInds);
            }
            else {
                mMode = Mode.SelectComponent;

                GUI.enabled = false;
                EditorGUILayout.IntPopup((int)Mode.SelectComponent, mModeTexts, mModeInds);
                GUI.enabled = true;
            }

            if(prevMode != mMode)
                InitCurrentMode();

            EditorExt.Utility.DrawSeparator();

            switch(mMode) {
                case Mode.EditPaths:
                    DoEditPaths();
                    break;

                case Mode.EditItems:
                    DoEditItems();
                    break;

                case Mode.SelectComponent:
                    DoSelectComponent();
                    break;
            }
        }

        void InitCurrentMode() {
            switch(mMode) {
                case Mode.EditItems:
                    //Remove items that are no longer in component
                    bool hasRemoves = RemoveInvalidItems();
                    bool hasLoads = LoadItems();

                    if(hasRemoves || hasLoads) {
                        //sort loaded items
                        mEditItems.Sort(mEditItemComparer);
                    }

                    //generate localize items for UI
                    GenerateLocalizedUIItems();

                    mEditItemsKeyEdit = false;
                    mEditItemsKeyEditIsNew = false;
                    mEditItemsCurKeyGen = 0;
                    break;
            }
        }

        void PathItemEdit(Language lang, ref TextAsset textAsset, ref Localize.TableDataPlatform[] platforms) {
            TextAsset eTextAsset = EditorGUILayout.ObjectField(textAsset, typeof(TextAsset), false) as TextAsset;
            if(textAsset != eTextAsset) {
                textAsset = eTextAsset;
                if(textAsset)
                    AddItemLoad(textAsset, lang, false, RuntimePlatform.WindowsEditor);
                else
                    RemoveItemLoad(lang, false, RuntimePlatform.WindowsEditor);
            }

            EditorGUILayout.BeginVertical(GUI.skin.box);

            GUILayout.Label("Platforms");

            if(platforms != null) {
                int removeInd = -1;

                for(int i = 0; i < platforms.Length; i++) {
                    EditorGUILayout.BeginHorizontal();

                    platforms[i].platform = (RuntimePlatform)EditorGUILayout.EnumPopup(platforms[i].platform);

                    platforms[i].file = EditorGUILayout.ObjectField(platforms[i].file, typeof(TextAsset), false) as TextAsset;

                    if(EditorExt.Utility.DrawRemoveButton())
                        removeInd = i;
                    else if(GUI.changed)
                        AddItemLoad(platforms[i].file, lang, true, platforms[i].platform);

                    EditorGUILayout.EndHorizontal();
                }

                if(removeInd != -1) {
                    RemoveItemLoad(lang, true, platforms[removeInd].platform);

                    M8.ArrayUtil.RemoveAt(ref platforms, removeInd);
                    GUI.changed = true;
                }
            }

            if(GUILayout.Button("Add Platform")) {
                if(platforms != null)
                    System.Array.Resize(ref platforms, platforms.Length + 1);
                else
                    platforms = new Localize.TableDataPlatform[1];

                platforms[platforms.Length - 1] = new Localize.TableDataPlatform();
                GUI.changed = true;
            }

            EditorGUILayout.EndVertical();
        }

        void AddItemLoad(TextAsset text, Language lang, bool isPlatform, RuntimePlatform platform) {
            //check if exists
            int itemInd = -1;
            for(int i = 0; i < mEditItemLoads.Count; i++) {
                if(mEditItemLoads[i].IsEqualTo(lang, isPlatform, platform)) {
                    itemInd = i;
                    break;
                }
            }

            ItemHeader itemAdd = new ItemHeader() { text=text, language=lang, isPlatform=isPlatform, platform=platform };

            if(itemInd == -1)
                mEditItemLoads.Add(itemAdd);
            else
                mEditItemLoads[itemInd] = itemAdd;
        }

        void RemoveItemLoad(Language lang, bool isPlatform, RuntimePlatform platform) {
            for(int i = 0; i < mEditItemLoads.Count; i++) {
                if(mEditItemLoads[i].IsEqualTo(lang, isPlatform, platform)) {
                    mEditItemLoads.RemoveAt(i);
                    break;
                }
            }
        }

        void DoEditPaths() {
            EditorGUI.BeginChangeCheck();

            mEditPathsScroll = GUILayout.BeginScrollView(mEditPathsScroll);

            //base
            EditorGUILayout.BeginVertical(GUI.skin.box);

            GUILayout.Label("Base");

            PathItemEdit(Language.Default, ref mLocalize.baseFile, ref mLocalize.basePlatforms);

            EditorGUILayout.EndVertical();

            //languages
            if(mLocalize.tables != null) {
                int removeInd = -1;

                for(int i = 0; i < mLocalize.tables.Length; i++) {
                    EditorGUILayout.BeginVertical(GUI.skin.box);

                    EditorGUILayout.BeginHorizontal();

                    Language lang = (Language)EditorGUILayout.EnumPopup(mLocalize.tables[i].language);

                    //language changed
                    if(mLocalize.tables[i].language != lang) {
                        Language prevLang = mLocalize.tables[i].language;
                        mLocalize.tables[i].language = lang;

                        //refresh any pending loads
                        for(int j = 0; j < mEditItemLoads.Count; j++) {
                            if(mEditItemLoads[j].language == prevLang)
                                mEditItemLoads[j] = new ItemHeader { text=mEditItemLoads[j].text, language=lang, isPlatform=mEditItemLoads[j].isPlatform, platform=mEditItemLoads[j].platform };
                        }
                    }

                    if(EditorExt.Utility.DrawRemoveButton())
                        removeInd = i;

                    EditorGUILayout.EndHorizontal();

                    PathItemEdit(mLocalize.tables[i].language, ref mLocalize.tables[i].file, ref mLocalize.tables[i].platforms);

                    EditorGUILayout.EndVertical();
                }

                if(removeInd != -1) {
                    M8.ArrayUtil.RemoveAt(ref mLocalize.tables, removeInd);
                    GUI.changed = true;
                }
            }

            if(GUILayout.Button("Add Language")) {
                if(mLocalize.tables != null)
                    System.Array.Resize(ref mLocalize.tables, mLocalize.tables.Length + 1);
                else
                    mLocalize.tables = new Localize.TableData[1];

                mLocalize.tables[mLocalize.tables.Length - 1] = new Localize.TableData();
                GUI.changed = true;
            }

            GUILayout.EndScrollView();

            if(EditorGUI.EndChangeCheck()) {
                EditorUtility.SetDirty(mLocalize);
            }
        }

        void LoadItem(TextAsset text, Language lang, bool isPlatform, RuntimePlatform platform) {
            Item item = null;

            if(lang == Language.Default && !isPlatform) {
                if(mEditItemBase == null)
                    mEditItemBase = new Item { header=new ItemHeader { text=text, language=lang, isPlatform=isPlatform } };

                item = mEditItemBase;
            }
            else {
                //grab existing item. if not, create it
                bool itemFound = false;
                for(int i = 0; i < mEditItems.Count; i++) {
                    if(mEditItems[i].header.IsEqualTo(lang, isPlatform, platform)) {
                        item = mEditItems[i];
                        itemFound = true;
                    }
                }

                if(!itemFound) {
                    item = new Item { header=new ItemHeader { text=text, language=lang, isPlatform=isPlatform, platform=platform } };
                    mEditItems.Add(item);
                }
            }

            LoadItem(item);
        }

        void SaveItem(Item item) {
            if(item.header.text) {
                //generate entries
                List<Localize.Entry> tableEntries = new List<Localize.Entry>();

                foreach(var pair in item.items) {
                    Localize.Entry entry = new Localize.Entry { key=pair.Key, text=pair.Value.value, param=item != mEditItemBase && pair.Value.paramIsRef ? null : pair.Value.param };
                    tableEntries.Add(entry);
                }

                //save
                fastJSON.JSON.Parameters.UseExtensions = false;
                string output = fastJSON.JSON.ToNiceJSON(tableEntries, fastJSON.JSON.Parameters);

                string path = AssetDatabase.GetAssetPath(item.header.text);
                File.WriteAllText(path, output);
            }

            item.changed = false;
        }

        bool RemoveInvalidItems() {
            bool ret = false;

            for(int i = 0; i < mEditItems.Count; i++) {
                if(mEditItems[i].header.language == Language.Default) {
                    if(mEditItems[i].header.isPlatform) {
                        bool found = false;
                        for(int p = 0; p < mLocalize.basePlatforms.Length; p++) {
                            if(mLocalize.basePlatforms[p].platform == mEditItems[i].header.platform) {
                                found = true;
                                break;
                            }
                        }
                        if(!found) {
                            ret = true;
                            mEditItems.RemoveAt(i);
                            i--;
                        }
                    }
                }
                else {
                    bool found = false;
                    for(int l = 0; l < mLocalize.tables.Length; l++) {
                        if(mLocalize.tables[l].language == mEditItems[i].header.language) {
                            if(mEditItems[i].header.isPlatform) {
                                for(int p = 0; p < mLocalize.tables[l].platforms.Length; p++) {
                                    if(mLocalize.tables[l].platforms[p].platform == mEditItems[i].header.platform) {
                                        found = true;
                                        break;
                                    }
                                }
                            }
                            else
                                found = true;
                            break;
                        }
                    }
                    if(!found) {
                        ret = true;
                        mEditItems.RemoveAt(i);
                        i--;
                    }
                }
            }

            return ret;
        }

        void GenerateLocalizedUIItems() {
            int c = mEditItems.Count;
            mEditItemTexts = new string[c];
            mEditItemInds = new int[c];
            for(int i = 0; i < c; i++) {
                mEditItemTexts[i] = mEditItems[i].header.isPlatform ?
                                string.Format("{0} ({1})", mEditItems[i].header.language.ToString(), mEditItems[i].header.platform.ToString())
                              : mEditItems[i].header.language.ToString();

                mEditItemInds[i] = i;
            }

            mEditItemInd = 0;
        }

        bool LoadItems() {
            bool ret = false;

            if(mEditItemLoads.Count > 0) {
                //load
                foreach(ItemHeader header in mEditItemLoads) {
                    //load actual data from file
                    if(header.text)
                        LoadItem(header.text, header.language, header.isPlatform, header.platform);
                    else { //remove
                        for(int i = 0; i < mEditItems.Count; i++) {
                            if(mEditItems[i].header.IsEqualTo(header)) {
                                mEditItems.RemoveAt(i);
                                break;
                            }
                        }
                    }
                }

                mEditItemLoads.Clear();

                ret = true;
            }


            return ret;
        }

        void LoadAllItems() {
            mEditItemBase = null;
            mEditItemBaseKeyTexts = null;
            mEditItemBaseKeyInds = null;

            mEditItems.Clear();

            if(mLocalize) {
                //base
                if(mLocalize.baseFile)
                    LoadItem(mLocalize.baseFile, Language.Default, false, RuntimePlatform.WindowsEditor);

                if(mLocalize.basePlatforms != null) {
                    for(int i = 0; i < mLocalize.basePlatforms.Length; i++)
                        LoadItem(mLocalize.basePlatforms[i].file, Language.Default, true, mLocalize.basePlatforms[i].platform);
                }

                //others
                if(mLocalize.tables != null) {
                    for(int i = 0; i < mLocalize.tables.Length; i++) {
                        if(mLocalize.tables[i].file)
                            LoadItem(mLocalize.tables[i].file, mLocalize.tables[i].language, false, RuntimePlatform.WindowsEditor);
                        if(mLocalize.tables[i].platforms != null) {
                            for(int p = 0; p < mLocalize.tables[i].platforms.Length; p++) {
                                if(mLocalize.tables[i].platforms[p].file)
                                    LoadItem(mLocalize.tables[i].platforms[p].file, mLocalize.tables[i].language, true, mLocalize.tables[i].platforms[p].platform);
                            }
                        }
                    }
                }
            }
        }

        void AddNewBaseKey() {
            string newKey = mEditItemBase.GenerateNewItem(ref mEditItemsCurKeyGen);

            int newCount = mEditItemBaseKeyTexts.Length + 1;

            System.Array.Resize(ref mEditItemBaseKeyTexts, newCount);
            System.Array.Resize(ref mEditItemBaseKeyInds, newCount);

            mEditItemBaseKeyTexts[newCount - 1] = newKey;
            mEditItemBaseKeyInds[newCount - 1] = newCount - 1;

            System.Array.Sort(mEditItemBaseKeyTexts);

            mEditItemBaseKeyInd = System.Array.IndexOf(mEditItemBaseKeyTexts, newKey);
        }

        void DrawItemData(string controlName, Item item, Data dat, ref string paramLastText) {
            //content
            GUILayout.Label("Value");

            GUI.SetNextControlName(controlName);

            if(dat != null) {
                string editText = EditorGUILayout.TextArea(dat.value, GUILayout.MinHeight(valueTextAreaMinHeight), GUILayout.MaxHeight(valueTextAreaMaxHeight));
                if(editText != dat.value) {
                    dat.value = editText;

                    item.changed = true;
                }

                //params
                GUILayout.BeginVertical(GUI.skin.box);

                item.paramFoldout = EditorGUILayout.Foldout(item.paramFoldout, "Params");
                if(item.paramFoldout) {
                    bool isRef;

                    if(item != mEditItemBase) {
                        isRef = GUILayout.Toggle(dat.paramIsRef, "Use Base");
                        if(dat.paramIsRef != isRef) {
                            dat.paramIsRef = isRef;
                            item.changed = true;
                        }
                    }
                    else
                        isRef = false;

                    if(!isRef) {
                        if(EditorExt.Utility.DrawStringArrayAlt(ref dat.param, ref paramLastText))
                            item.changed = true;
                    }
                }

                GUILayout.EndVertical();
            }
            else {
                EditorGUILayout.TextArea("", GUILayout.MinHeight(valueTextAreaMinHeight), GUILayout.MaxHeight(valueTextAreaMaxHeight));

                GUILayout.BeginVertical(GUI.skin.box);

                EditorGUILayout.Foldout(false, "Params");

                GUILayout.EndVertical();
            }
        }

        void DoEditItems() {
            if(mLocalize.baseFile == null || mEditItemBase == null) {
                GUILayout.Label("Base File is not set!");
                return;
            }

            mEditItemsScroll = GUILayout.BeginScrollView(mEditItemsScroll);

            string baseCurKey = null;

            //display base
            if(mEditItemBaseKeyTexts.Length > 0) {
                baseCurKey = mEditItemBaseKeyTexts[mEditItemBaseKeyInd];

                Data baseCurDat = mEditItemBase.items[baseCurKey];

                //key
                GUILayout.Label("Key");

                if(mEditItemsKeyEdit) {
                    GUILayout.BeginHorizontal();

                    GUI.SetNextControlName(editKeyControl);

                    mEditItemsKeyText = EditorGUILayout.TextField(mEditItemsKeyText);

                    GUI.enabled = !string.IsNullOrEmpty(mEditItemsKeyText) && !mEditItemBase.items.ContainsKey(mEditItemsKeyText);

                    //change key
                    if(EditorExt.Utility.DrawSimpleButton("√", "Accept")) {
                        if(mEditItemsKeyEditIsNew) {
                            mEditItemBase.AddNewItem(mEditItemsKeyText);

                            int newCount = mEditItemBaseKeyTexts.Length + 1;

                            System.Array.Resize(ref mEditItemBaseKeyTexts, newCount);
                            System.Array.Resize(ref mEditItemBaseKeyInds, newCount);

                            mEditItemBaseKeyTexts[newCount - 1] = mEditItemsKeyText;
                            mEditItemBaseKeyInds[newCount - 1] = newCount - 1;

                            System.Array.Sort(mEditItemBaseKeyTexts);

                            mEditItemBaseKeyInd = System.Array.IndexOf(mEditItemBaseKeyTexts, mEditItemsKeyText);
                        }
                        else {
                            //remove old key
                            mEditItemBase.items.Remove(baseCurKey);

                            //regenerate keys
                            mEditItemBaseKeyTexts[mEditItemBaseKeyInd] = mEditItemsKeyText;
                            System.Array.Sort(mEditItemBaseKeyTexts);
                            mEditItemBaseKeyInd = System.Array.IndexOf(mEditItemBaseKeyTexts, mEditItemsKeyText);

                            //re-add to items
                            mEditItemBase.items.Add(mEditItemsKeyText, baseCurDat);

                            mEditItemBase.changed = true;
                        }

                        mEditItemsKeyEdit = false;
                        mEditItemsKeyEditIsNew = false;

                        EditorGUI.FocusTextInControl(editItemControl1);
                    }

                    GUI.enabled = true;

                    //cancel
                    if(EditorExt.Utility.DrawSimpleButton("X", "Cancel")) {
                        mEditItemsKeyEdit = false;

                        EditorGUI.FocusTextInControl(editItemControl1);
                    }

                    GUILayout.EndHorizontal();
                }
                else {
                    GUILayout.BeginHorizontal();

                    int ind = EditorGUILayout.IntPopup(mEditItemBaseKeyInd, mEditItemBaseKeyTexts, mEditItemBaseKeyInds);
                    if(mEditItemBaseKeyInd != ind) {
                        mEditItemBaseKeyInd = Mathf.Clamp(ind, 0, mEditItemBaseKeyTexts.Length-1);
                        mEditItemParamText = "";
                    }

                    //edit key
                    if(EditorExt.Utility.DrawSimpleButton("E", "Edit")) {
                        mEditItemsKeyText = mEditItemBaseKeyTexts[mEditItemBaseKeyInd];
                        mEditItemsKeyEdit = true;
                        EditorGUI.FocusTextInControl(editKeyControl);
                    }

                    //copy key text
                    if(EditorExt.Utility.DrawCopyButton("Copy key text")) {
                        mTE.content = new GUIContent(mEditItemBaseKeyTexts[mEditItemBaseKeyInd]);
                        mTE.SelectAll();
                        mTE.Copy();
                    }

                    //add
                    if(EditorExt.Utility.DrawAddButton()) {
                        mEditItemsKeyText = "";
                        mEditItemsKeyEdit = true;
                        mEditItemsKeyEditIsNew = true;
                        EditorGUI.FocusTextInControl(editKeyControl);
                    }

                    //remove
                    if(EditorExt.Utility.DrawRemoveButton()) {
                        M8.ArrayUtil.RemoveAt(ref mEditItemBaseKeyTexts, mEditItemBaseKeyInd);
                        M8.ArrayUtil.RemoveAt(ref mEditItemBaseKeyInds, mEditItemBaseKeyInd);

                        for(int i = mEditItemBaseKeyInd; i < mEditItemBaseKeyInds.Length; i++)
                            mEditItemBaseKeyInds[i] = i;

                        if(mEditItemBaseKeyInd > 0 && mEditItemBaseKeyInd >= mEditItemBaseKeyInds.Length)
                            mEditItemBaseKeyInd--;

                        mEditItemBase.items.Remove(baseCurKey);

                        mEditItemBase.changed = true;
                    }

                    GUILayout.EndHorizontal();
                }

                GUI.enabled = !mEditItemsKeyEdit;

                DrawItemData(editItemControl1, mEditItemBase, baseCurDat, ref mEditItemParamText);

                GUI.enabled = true;
            }
            else if(GUILayout.Button("Add new item")) {
                AddNewBaseKey();

                mEditItemsKeyText = "";
                mEditItemsKeyEdit = true;
                EditorGUI.FocusTextInControl(editKeyControl);
            }

            //Localize

            mEditItemsLocalizeFoldout = EditorGUILayout.Foldout(mEditItemsLocalizeFoldout, "Localize");
            if(mEditItemsLocalizeFoldout && mEditItems.Count > 0) {
                GUI.enabled = !(mEditItemsKeyEdit || string.IsNullOrEmpty(baseCurKey));

                if(mEditItemInd >= mEditItemTexts.Length)
                    mEditItemInd = 0;

                int ind = EditorGUILayout.IntPopup(mEditItemInd, mEditItemTexts, mEditItemInds);
                if(mEditItemInd != ind) {
                    mEditItemInd = Mathf.Clamp(ind, 0, mEditItemTexts.Length-1);
                    mEditItemLocalizeParamText = "";
                }

                Item localizeItem = mEditItems[mEditItemInd];

                Data localizeDat = null;

                if(GUI.enabled) {
                    if(!localizeItem.items.TryGetValue(baseCurKey, out localizeDat)) {
                        localizeDat = new Data { value="", param=null, paramIsRef=true };
                        localizeItem.items.Add(baseCurKey, localizeDat);
                    }
                }

                DrawItemData(editItemControl2, localizeItem, localizeDat, ref mEditItemLocalizeParamText);

                GUI.enabled = true;
            }

            GUILayout.EndScrollView();

            EditorExt.Utility.DrawSeparator();

            //save, revert
            bool hasChanges;
            if(mEditItemsKeyEdit)
                hasChanges = false;
            else {
                hasChanges = mEditItemBase.changed;
                if(!hasChanges) {
                    foreach(Item item in mEditItems) {
                        hasChanges = item.changed;
                        if(hasChanges)
                            break;
                    }
                }
            }

            GUI.enabled = hasChanges;

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUI.backgroundColor = Color.green;
            if(GUILayout.Button("Save", GUILayout.Width(100f))) {
                if(mEditItemBase.changed)
                    SaveItem(mEditItemBase);

                foreach(Item item in mEditItems) {
                    if(item.changed)
                        SaveItem(item);
                }

                AssetDatabase.Refresh();
            }

            GUILayout.Space(8f);

            GUI.backgroundColor = Color.red;
            if(GUILayout.Button("Revert", GUILayout.Width(100f))) {
                if(mEditItemBase.changed)
                    LoadItem(mEditItemBase);


                foreach(Item item in mEditItems) {
                    if(item.changed)
                        LoadItem(item);
                }

                GUIUtility.keyboardControl = 0;
                GUIUtility.hotControl = 0;
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUI.enabled = true;
        }

        void DoSelectComponent() {
            GUILayout.Label("Grab from a core prefab.");
            Localize localize = EditorGUILayout.ObjectField(mLocalize, typeof(Localize), false) as Localize;
            if(mLocalize != localize) {
                mLocalize = localize;

                if(localize) {
                    //save if it's from a prefab
                    if(SaveLocalizeObjectPath(localize)) {
                        mLocalize = localize;

                        LoadAllItems();
                        GenerateBaseKeyItems();
                    }
                }

                Repaint();
            }
        }
    }
}