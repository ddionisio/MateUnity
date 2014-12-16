using UnityEngine;
using System.Collections;

namespace M8 {
    /// <summary>
    /// Use this if you want to have your first scene as a boot-up, then immediately load the next.
    /// </summary>
    [AddComponentMenu("M8/Core/SceneInitializer")]
    public class SceneInitializer : MonoBehaviour {
        [SerializeField]
        string _toScene = "start"; //the scene to load to once initScene is finish

        IEnumerator Start() {
            //wait for one update to ensure all initialization has occured
            yield return new WaitForFixedUpdate();

            SceneManager.instance.LoadSceneNoTransition(_toScene);
        }
    }
}