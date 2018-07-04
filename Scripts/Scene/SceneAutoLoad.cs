using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    /// <summary>
    /// Simple behaviour to load a scene on Start
    /// </summary>
    [AddComponentMenu("M8/Core/SceneAutoLoad")]
    public class SceneAutoLoad : MonoBehaviour {
        [SerializeField]
        SceneAssetPath _scene;

        [SerializeField]
        float _delay = 0f;

        [SerializeField]
        bool _destroyAfter;
        
        IEnumerator Start() {
            if(_delay > 0f)
                yield return new WaitForSeconds(_delay);

            SceneManager.instance.LoadScene(_scene.name);

            if(_destroyAfter)
                Destroy(gameObject);
        }
    }
}