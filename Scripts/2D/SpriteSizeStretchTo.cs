using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
	[AddComponentMenu("M8/Sprite/SizeStretchTo")]
	public class SpriteSizeStretchTo : MonoBehaviour {
		public enum Dir {
			None,
			Up,
			Right,
			Both //will stretch in both width and height based on area from start to end root
		}

		public SpriteRenderer source;

		public Transform startRoot; //start can also be source, position adjustment will be disregarded
		public Transform endRoot;

		public Dir dir;
		public bool reverse;

		public Vector2 ofs;
		public Vector2 sizeOfs;

		void Awake() {
			if(!source)
				source = GetComponent<SpriteRenderer>();

			if(!startRoot)
				startRoot = transform;
		}

		void Update() {
			if(source && source.sprite && endRoot) {
				var sourceTrans = source.transform;

				var sprRectPixel = source.sprite.rect;
				var sprPivotPixel = source.sprite.pivot;
				var sprPivot = new Vector2(sprPivotPixel.x / sprRectPixel.width, sprPivotPixel.y / sprRectPixel.height);

				Vector2 spos = startRoot.position;
				Vector2 epos = endRoot.position;
								
				if(dir == Dir.Both) {
					//apply size
					var s = new Vector2(Mathf.Abs(epos.x - spos.x) + sizeOfs.x, Mathf.Abs(epos.y - spos.y) + sizeOfs.y);

					source.size = s;

					//apply position
					if(sourceTrans != startRoot) {
						var srcPos = sourceTrans.position;
						sourceTrans.position = new Vector3(Mathf.Min(spos.x, epos.x) + sprPivot.x * s.x + ofs.x, Mathf.Min(spos.y, epos.y) + sprPivot.y * s.y + ofs.y, srcPos.z);
					}

					//apply orientation
					sourceTrans.up = Vector2.up;
				}
				else {
					var s = source.size;
					var d = epos - spos;

					//apply dir
					var len = d.magnitude;
					if(len > 0) {
						d /= len;

						switch(dir) {
							case Dir.Up:
								sourceTrans.up = reverse ? -d : d;
								break;
							case Dir.Right:
								sourceTrans.right = reverse ? -d : d;
								break;
						}
					}

					//apply size
					switch(dir) {
						case Dir.Up:
							s.y = len + sizeOfs.y;
							break;
						case Dir.Right:
							s.x = len + sizeOfs.x;
							break;
					}

					source.size = s;

					//apply position
					if(sourceTrans != startRoot) {
						var srcPos = new Vector3(spos.x, spos.y, sourceTrans.position.z);

						if(ofs.x != 0f)
							srcPos += sourceTrans.right * ofs.x;

						if(ofs.y != 0f)
							srcPos += sourceTrans.up * ofs.y;

						switch(dir) {
							case Dir.Up:
								srcPos += sourceTrans.up * sprPivot.y * s.y;
								break;
							case Dir.Right:
								srcPos += sourceTrans.right * sprPivot.x * s.x;
								break;
						}

						sourceTrans.position = srcPos;
					}
				}
			}
		}
	}
}