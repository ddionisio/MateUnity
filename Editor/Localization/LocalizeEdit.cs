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
                var localizeAssetPath = Localize.assetPath;                
                var localizeResGUID = AssetDatabase.AssetPathToGUID(localizeAssetPath);
                return !string.IsNullOrEmpty(localizeResGUID);
            }
        }
    }
}