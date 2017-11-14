using UnityEngine;
using UnityEngine.SceneManagement;

namespace M8 {
    [System.Serializable]
    public struct SceneAssetPath {
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

        public bool isValid { get { return !string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(path); } }

        public SceneAssetPath(Scene s) {
            name = s.name;
            path = s.path;
        }

        public void Load() {
            SceneManager.instance.LoadScene(name);
        }

        public override int GetHashCode() {
            return name.GetHashCode() ^ path.GetHashCode();
        }

        public override bool Equals(object obj) {
            if(obj == null)
                return false;

            if(obj is Scene) {
                Scene s = (Scene)obj;
                return name == s.name && path == s.path;
            }
            else if(obj is SceneAssetPath) {
                SceneAssetPath s = (SceneAssetPath)obj;
                return name == s.name && path == s.path;
            }

            return base.Equals(obj);
        }


        public static bool operator ==(SceneAssetPath a, SceneAssetPath b) {
            return a.name == b.name && a.path == b.path;
        }

        public static bool operator !=(SceneAssetPath a, SceneAssetPath b) {
            return !(a == b);
        }

        public static bool operator ==(SceneAssetPath a, Scene b) {
            return a.name == b.name && a.path == b.path;
        }

        public static bool operator !=(SceneAssetPath a, Scene b) {
            return !(a == b);
        }
    }
}