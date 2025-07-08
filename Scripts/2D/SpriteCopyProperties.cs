using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    /// <summary>
    /// Copy property of source sprite
    /// </summary>
    [AddComponentMenu("M8/Sprite/CopyProperties")]
	[ExecuteInEditMode]
	public class SpriteCopyProperties : MonoBehaviour {
		[Header("Sprite Renders")]
		public SpriteRenderer sourceSpriteRender;
		public SpriteRenderer destinationSpriteRender;

		[Header("Copy Properties")]
		public bool copySprite = true;
		public bool copyColor = false;
		public bool copyFlipX = true;
		public bool copyFlipY = true;
		public bool copySize = true;

		void Awake() {
			if(!destinationSpriteRender)
				destinationSpriteRender = GetComponent<SpriteRenderer>();
		}

		void Update() {
			if(sourceSpriteRender && destinationSpriteRender) {
				if(copySprite) {
					if(destinationSpriteRender.sprite != sourceSpriteRender.sprite)
						destinationSpriteRender.sprite = sourceSpriteRender.sprite;
				}

				if(copyColor) {
					if(destinationSpriteRender.color != sourceSpriteRender.color)
						destinationSpriteRender.color = sourceSpriteRender.color;
				}

				if(copyFlipX)
					destinationSpriteRender.flipX = sourceSpriteRender.flipX;

				if(copyFlipY)
					destinationSpriteRender.flipY = sourceSpriteRender.flipY;

				if(copySize) {
					if(destinationSpriteRender.size != sourceSpriteRender.size)
						destinationSpriteRender.size = sourceSpriteRender.size;
				}
			}
		}
	}
}