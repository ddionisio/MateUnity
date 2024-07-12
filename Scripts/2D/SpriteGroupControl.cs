using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
	[AddComponentMenu("M8/Sprite/GroupControl")]
	public class SpriteGroupControl : MonoBehaviour {
		[SerializeField]
		SpriteRenderer[] _spriteRenders;
		[SerializeField]
		bool _spriteRendersAutoFill;

		[SerializeField]
		Color _color;
		[SerializeField]
		bool _colorApply;

		[SerializeField]
		bool _flipX;
		[SerializeField]
		bool _flipXApply;

		[SerializeField]
		bool _flipY;
		[SerializeField]
		bool _flipYApply;

		[SerializeField]
		int _sortLayerID;
		[SerializeField]
		bool _sortLayerIDApply;

		[SerializeField]
		int _sortOrder;
		[SerializeField]
		bool _sortOrderApply;
				
		public Color color {
			get { return _color; }
			set {
				if(_color != value) {
					_color = value;

					if(!mIsInit) Awake();

					for(int i = 0; i < _spriteRenders.Length; i++) {
						var spr = _spriteRenders[i];
						if(spr)
							spr.color = _color;
					}
				}
			}
		}

		public bool flipX {
			get { return _flipX; }
			set {
				if(_flipX != value) {
					_flipX = value;

					if(!mIsInit) Awake();

					for(int i = 0; i < _spriteRenders.Length; i++) {
						var spr = _spriteRenders[i];
						if(spr)
							spr.flipX = _flipX;
					}
				}
			}
		}

		public bool flipY {
			get { return _flipY; }
			set {
				if(_flipY != value) {
					_flipY = value;

					if(!mIsInit) Awake();

					for(int i = 0; i < _spriteRenders.Length; i++) {
						var spr = _spriteRenders[i];
						if(spr)
							spr.flipY = _flipY;
					}
				}
			}
		}

		public int sortLayerID {
			get { return _sortLayerID; }
			set {
				if(_sortLayerID != value) {
					_sortLayerID = value;

					if(!mIsInit) Awake();

					for(int i = 0; i < _spriteRenders.Length; i++) {
						var spr = _spriteRenders[i];
						if(spr)
							spr.sortingLayerID = _sortLayerID;
					}
				}
			}
		}

		public string sortLayerName {
			get { return SortingLayer.IDToName(_sortLayerID); }
			set { sortLayerID = SortingLayer.NameToID(value); }
		}

		bool mIsInit;

		public void ApplyProperties(SpriteRenderer[] renderers) {
			for(int i = 0; i < renderers.Length; i++) {
				var spr = renderers[i];
				if(spr) {
					if(_colorApply)
						spr.color = _color;

					if(_sortLayerIDApply)
						spr.sortingLayerID = _sortLayerID;
					if(_sortOrderApply)
						spr.sortingOrder = _sortOrder;

					if(_flipXApply)
						spr.flipX = _flipX;
					if(_flipYApply)
						spr.flipY = _flipY;
				}
			}
		}

		public void ApplyProperties() {
			if(Application.isPlaying && !mIsInit) {
				if(_spriteRendersAutoFill)
					_spriteRenders = GetComponentsInChildren<SpriteRenderer>(true);

				mIsInit = true;
			}

			ApplyProperties(_spriteRenders);
		}

		void Awake() {
			if(!mIsInit) {
				if(_spriteRendersAutoFill)
					_spriteRenders = GetComponentsInChildren<SpriteRenderer>(true);

				ApplyProperties(_spriteRenders);

				mIsInit = true;
			}
		}
	}
}