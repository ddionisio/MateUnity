using UnityEngine;
using System.Collections;

namespace M8 {
    /// <summary>
    /// Convenience for loading from serialization (e.g. JSON)
    /// </summary>
    public class ResourcePackage {
        public ResourceManager.LoaderType type;
        public string path;
                
        public static void LoadToGroup(string resGroup, ResourcePackage[] packages, int priority) {
            if(packages == null || packages.Length <= 0) return;

            ResourceManager resMgr = ResourceManager.instance;

            //add packages
            for(int i = 0; i < packages.Length; i++) {
                ResourcePackage package = packages[i];
                resMgr.AddToGroup(resGroup, package.path, package.type, priority);
            }

            //load newly added packages in group
            resMgr.LoadGroup(resGroup);
        }
    }
}