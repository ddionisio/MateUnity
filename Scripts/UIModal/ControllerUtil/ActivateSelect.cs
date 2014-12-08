using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace M8.UIModal.ControllerUtil {
    /// <summary>
    /// Add this along UIController to set given 'select' as selected in Unity's EventSystem
    /// </summary>
    [AddComponentMenu("M8/UI Modal/Controller/ActivateSelect")]
    [RequireComponent(typeof(Controller))]
    public class ActivateSelect : MonoBehaviour {
        public GameObject select;

        private Controller mCtrl;

        void Awake() {
            GetComponent<Controller>().onActiveCallback += OnActivate;
        }

        void OnActivate(bool yes) {
            if(yes) {
                EventSystem es = EventSystem.current;
                if(es)
                    es.SetSelectedGameObject(select);
            }
        }
    }
}