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

        private Renderer mRenderer;

        public void Apply() {
            if(!mRenderer)
                mRenderer = GetComponent<Renderer>();

            mRenderer.sortingLayerName = _sortingLayerName;
            mRenderer.sortingOrder = _sortingOrder;
        }

        void Awake() {
            Apply();
        }
    }
}