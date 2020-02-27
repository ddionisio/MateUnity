using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace M8 {
    /// <summary>
    /// Localize Edit Utility
    /// </summary>
    public struct LocalizeEdit {
        public static bool isLocalizeFileExists {
            get {
                //try default path
                var localizeResGUID = AssetDatabase.AssetPathToGUID(Localize.assetPath);
                if(!string.IsNullOrEmpty(localizeResGUID)) {
                    //check if at least one exists
                    var guids = AssetDatabase.FindAssets("t:" + typeof(Localize).Name);
                    return guids.Length > 0;
                }

                return true;
            }
        }
    }
}