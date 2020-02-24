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
                var localizeAssetPath = Localize.assetPath;                
                var localizeResGUID = AssetDatabase.AssetPathToGUID(localizeAssetPath);
                if(!string.IsNullOrEmpty(localizeResGUID)) {
                    //try finding file
                    var resourcePath = Localize.resourcePath;
                    var filenames = AssetDatabase.FindAssets("l:"+resourcePath);
                    for(int i = 0; i < filenames.Length; i++) {
                        if(filenames[i].Contains(resourcePath))
                            return true;
                    }

                    return false;
                }

                return true;
            }
        }
    }
}