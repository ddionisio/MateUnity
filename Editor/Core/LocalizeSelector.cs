using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace M8 {
    public class LocalizeSelector : EditorWindow {
        public const string projConfigKey = "localizeSelector";

        private static Localize mLocalize;

        public static Localize localize {
            get {
                if(!mLocalize)
                    LoadFromProjectConfig();

                return mLocalize;
            }
        }

        public static LocalizeSelector Open() {
            return EditorWindow.GetWindow(typeof(LocalizeSelector)) as LocalizeSelector;
        }

        static void LoadFromProjectConfig() {
            mLocalize = ProjectConfig.GetObject<Localize>(projConfigKey);
        }

        static void SaveToProjectConfig() {
            ProjectConfig.SetObject(projConfigKey, mLocalize);
        }

        void OnEnable() {
            if(!mLocalize)
                LoadFromProjectConfig();
        }

        void OnGUI() {
            GUILayout.Label("Select a Localize (usu. from core.prefab)");
            Localize localize = EditorGUILayout.ObjectField(mLocalize, typeof(Localize), false) as Localize;

            if(mLocalize != localize) {
                mLocalize = localize;
                SaveToProjectConfig();
            }
        }
    }
}