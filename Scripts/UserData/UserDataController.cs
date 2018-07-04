using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    /// <summary>
    /// Add this to allow control of UserData during runtime such as: loading on awake, auto save
    /// </summary>
    [AddComponentMenu("M8/Core/UserData Controller")]
    public class UserDataController : MonoBehaviour {
        [SerializeField]
        UserData _userData;

        [Header("Flags")]

        [SerializeField]
        bool _loadOnAwake;
        [SerializeField]
        bool _saveOnSceneChange;
        [SerializeField]
        bool _saveOnAppExit;

        void OnApplicationQuit() {
            if(_saveOnAppExit)
                _userData.Save();
        }

        void OnDestroy() {
            if(_saveOnSceneChange && SceneManager.isInstantiated) {
                SceneManager.instance.sceneChangeCallback -= OnSceneChanged;
            }
        }

        void Awake() {
            if(_loadOnAwake)
                _userData.Load();

            if(_saveOnSceneChange && SceneManager.isInstantiated) {
                SceneManager.instance.sceneChangeCallback += OnSceneChanged;
            }
        }

        void OnSceneChanged(string nextScene) {
            _userData.Save();
        }
    }
}