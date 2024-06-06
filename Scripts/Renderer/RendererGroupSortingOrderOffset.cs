using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
	[AddComponentMenu("M8/Renderer/GroupSortingOrderOffset")]
	public class RendererGroupSortingOrderOffset : MonoBehaviour {
		public Renderer[] renderers;

		[SerializeField]
		int _sortingOrderOffset = 0;

		[SerializeField]
		bool _applyOnAwake;

		public int sortingOrderOffset {
			get { return _sortingOrderOffset; }
			set {
				if(_sortingOrderOffset != value || !mIsApplied) {
					ApplyOffset(value);
				}
			}
		}

		private int[] mSortingOrderDefaults;

		private bool mIsApplied = false;

		public void ApplyOffset(int offset) {
			if(renderers == null || renderers.Length == 0 || (mSortingOrderDefaults != null && renderers.Length != mSortingOrderDefaults.Length)) {
				Revert();
				InitDefaultData();
			}
			else if(mSortingOrderDefaults == null)
				InitDefaultData();

			for(int i = 0; i < renderers.Length; i++) {
				if(renderers[i])
					renderers[i].sortingOrder = mSortingOrderDefaults[i] + offset;
			}

			mIsApplied = true;
			_sortingOrderOffset = offset;
		}

		public void Revert() {
			if(mIsApplied) {
				mIsApplied = false;

				if(renderers == null || mSortingOrderDefaults == null)
					return;

				for(int i = 0; i < renderers.Length; i++) {
					if(renderers[i])
						renderers[i].sortingOrder = mSortingOrderDefaults[i];
				}

				_sortingOrderOffset = 0;
			}
		}

		void OnDestroy() {
			Revert();
		}

		void Awake() {
			if(_applyOnAwake)
				ApplyOffset(_sortingOrderOffset);
		}

		private void InitDefaultData() {
			if(renderers == null || renderers.Length == 0)
				renderers = GetComponentsInChildren<Renderer>(true);

			mSortingOrderDefaults = new int[renderers.Length];

			for(int i = 0; i < renderers.Length; i++)
				mSortingOrderDefaults[i] = renderers[i].sortingOrder;
		}
	}
}