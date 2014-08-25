using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/Renderer/SortingLayer")]
[RequireComponent(typeof(Renderer))]
public class RendererSortingLayer : MonoBehaviour {
    [SerializeField]
    string _sortingLayerName;
    [SerializeField]
    int _sortingOrder;

    void Awake() {
        renderer.sortingLayerName = _sortingLayerName;
        renderer.sortingOrder = _sortingOrder;
    }
}
