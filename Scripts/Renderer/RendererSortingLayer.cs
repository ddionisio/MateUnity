using UnityEngine;
using System.Collections;

namespace M8 {
    [AddComponentMenu("M8/Renderer/SortingLayer")]
    [RequireComponent(typeof(Renderer))]
    public class RendererSortingLayer : MonoBehaviour {
        [SerializeField, SortingLayerAttribute]
        string _sortingLayerName;
        [SerializeField]
        int _sortingOrder;

        void Awake() {
            Renderer r = GetComponent<Renderer>();
            r.sortingLayerName = _sortingLayerName;
            r.sortingOrder = _sortingOrder;
        }
    }
}