using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
	[AddComponentMenu("M8/Renderer/Material Set Property Block")]
	[RequireComponent(typeof(Renderer))]
	[ExecuteInEditMode]
	public class RendererMaterialSetPropertyBlock : MonoBehaviour {
		public enum ValueType {
			None = -1,

			Color,
			Vector,
			Float,
			Range,
			TexEnv,
			Int
		}

		[System.Serializable]
		public struct PropertyInfo {
			public int id;

			public ValueType valueType;
			public Vector4 valueVector;
			public Texture valueTexture;

			public void Apply(MaterialPropertyBlock propBlock) {
				switch(valueType) {
					case ValueType.Color:
						propBlock.SetColor(id, new Color(valueVector.x, valueVector.y, valueVector.z, valueVector.w));
						break;
					case ValueType.Vector:
						propBlock.SetVector(id, valueVector);
						break;
					case ValueType.Float:
						propBlock.SetFloat(id, valueVector.x);
						break;
					case ValueType.Range:
						propBlock.SetFloat(id, valueVector.x);
						break;
					case ValueType.TexEnv:
						if(valueTexture)
							propBlock.SetTexture(id, valueTexture);
						break;
					case ValueType.Int:
						propBlock.SetInt(id, (int)valueVector.x);
						break;
				}
			}

			public void SetValueFrom(Material mat) {
				switch(valueType) {
					case ValueType.Color:
						valueVector = mat.GetColor(id);
						break;
					case ValueType.Vector:
						valueVector = mat.GetVector(id);
						break;
					case ValueType.Float:
					case ValueType.Range:
						valueVector.x = mat.GetFloat(id);
						break;
					case ValueType.TexEnv:
						valueTexture = mat.GetTexture(id);
						break;
					case ValueType.Int:
						valueVector.x = mat.GetInt(id);
						break;
				}
			}
		}

		public PropertyInfo[] properties;

		public Renderer renderTarget {
			get {
				if(!mRenderer)
					mRenderer = GetComponent<Renderer>();

				return mRenderer;
			}
		}

		private Renderer mRenderer;
		private MaterialPropertyBlock mMatPropBlock;

		public void Apply() {
			if(properties != null && properties.Length > 0) {
				var render = renderTarget;

				if(mMatPropBlock == null)
					mMatPropBlock = new MaterialPropertyBlock();

				render.GetPropertyBlock(mMatPropBlock);

				for(int i = 0; i < properties.Length; i++)
					properties[i].Apply(mMatPropBlock);

				render.SetPropertyBlock(mMatPropBlock);
			}
		}

		void OnEnable() {
			Apply();
		}
	}
}