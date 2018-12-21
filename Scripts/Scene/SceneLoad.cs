using UnityEngine;

namespace M8 {
    /// <summary>
    /// Simple behaviour to load a scene via Execute, useful for UI, or some timeline editor
    /// </summary>
    [AddComponentMenu("M8/Core/SceneLoad")]
    public class SceneLoad : MonoBehaviour {
        [SerializeField]
        SceneAssetPath _scene = new SceneAssetPath();
        
        public void Execute() {
            SceneManager.instance.LoadScene(_scene.name);
        }
	}
}
