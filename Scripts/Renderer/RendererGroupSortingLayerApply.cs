using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    [AddComponentMenu("M8/Renderer/GroupSortingLayerApply")]
    public class RendererGroupSortingLayerApply : MonoBehaviour {
		[SerializeField]
		Renderer[] _renderers;
		[SerializeField]
		bool _fillRenderersOnAwake = false;

		[SerializeField, SortingLayer]
		string _sortingLayerName = "default";

		[SerializeField]
		bool _apply;

		private bool mIsInit;
		private int mSortingLayerId;

		private int[] mSortingLayerIdDefaults;

		public bool apply {
			get { return _apply; }
			set {
				if(_apply != value) {
					_apply = value;

					if(_apply)
						Apply();
					else
						Revert();
				}
			}
		}

		public string sortingLayerName {
			get { return _sortingLayerName; }
			set {
				if(_sortingLayerName != value) {
					_sortingLayerName = value;
					mSortingLayerId = SortingLayer.NameToID(_sortingLayerName);
				}
			}
		}

		public int sortingLayerID {
			get { return mSortingLayerId; }
			set {
				if(mSortingLayerId != value) {
					mSortingLayerId = value;
					_sortingLayerName = SortingLayer.IDToName(mSortingLayerId);
				}
			}
		}

		public void Apply() {
			if(!_apply) {
				if(_fillRenderersOnAwake && (_renderers == null || _renderers.Length == 0))
					_renderers = GetComponentsInChildren<Renderer>(true);

				if(!mIsInit) Init();

				for(int i = 0; i < _renderers.Length; i++) {
					if(_renderers[i])
						_renderers[i].sortingLayerID = mSortingLayerId;
				}

				_apply = true;
			}
		}

		public void Revert() {
			if(_apply) {
				for(int i = 0; i < _renderers.Length; i++) {
					if(_renderers[i])
						_renderers[i].sortingLayerID = mSortingLayerIdDefaults[i];
				}

				_apply = false;
			}
		}

		void Awake() {
			if(_fillRenderersOnAwake)
				_renderers = GetComponentsInChildren<Renderer>(true);

			if(!mIsInit) Init();

			if(_apply) Apply();
		}

		private void Init() {
			mSortingLayerId = SortingLayer.NameToID(_sortingLayerName);

			mSortingLayerIdDefaults = new int[_renderers.Length];

			for(int i = 0; i < mSortingLayerIdDefaults.Length; i++)
				mSortingLayerIdDefaults[i] = _renderers[i].sortingLayerID;

			mIsInit = true;
		}
	}
}