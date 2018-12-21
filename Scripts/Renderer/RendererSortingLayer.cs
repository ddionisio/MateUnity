using UnityEngine;
using System.Collections;

namespace M8 {
    [AddComponentMenu("M8/Renderer/SortingLayer")]
    [RequireComponent(typeof(Renderer))]
    public class RendererSortingLayer : MonoBehaviour {
        [SerializeField, SortingLayerAttribute]
        string _sortingLayerName = "default";
        [SerializeField]
        int _sortingOrder = 0;

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