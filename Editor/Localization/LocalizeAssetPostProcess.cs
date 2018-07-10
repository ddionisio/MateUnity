using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace M8 {
    public class LocalizeAssetPostProcess : AssetPostprocessor {        
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {
            if(!Localize.isInstantiated)
                return;

            for(int i = 0; i < importedAssets.Length; i++) {
                //check localize selector
                if(Localize.instance.IsLanguageFile(importedAssets[i]))
                    Localize.instance.Load();
            }
        }
    }
}