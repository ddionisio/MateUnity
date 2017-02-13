using UnityEngine;

namespace M8 {
    [System.Serializable]
    public class SceneAssetPath {
        public string name;
        public string path;
        
        public static string LoadableName(string path) {
            int startInd = path.LastIndexOf("/");
            if(startInd == -1)
                startInd = 0;
            else
                startInd++;

            int endInd = path.LastIndexOf(".unity");
            if(endInd == -1)
                endInd = path.Length - 1;

            if(startInd >= endInd)
                return path;
            
            return path.Substring(startInd, endInd - startInd);
        }

        public void Load() {
            SceneManager.instance.LoadScene(name);
        }
    }
}