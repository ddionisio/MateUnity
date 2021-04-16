using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    [AddComponentMenu("M8/Renderer/MaterialSetProperty")]
    [RequireComponent(typeof(Renderer))]
    public class RendererMaterialSetProperty : MonoBehaviour {
        public enum ValueType {
            None = -1,
            Color,
            Vector,
            Float,
            Range,
            TexEnv,
            TexOfs,
            TexScale
        }

        [System.Serializable]
        public struct PropertyInfo {
            public string name;

            public ValueType valueType;
            public Vector4 valueVector;
            public Texture valueTexture;

            public void Apply(Material mat) {
                switch(valueType) {
                    case ValueType.Color:
                        mat.SetColor(name, new Color(valueVector.x, valueVector.y, valueVector.z, valueVector.w));
                        break;
                    case ValueType.Vector:
                        mat.SetVector(name, valueVector);
                        break;
                    case ValueType.Float:
                        mat.SetFloat(name, valueVector.x);
                        break;
                    case ValueType.Range:
                        mat.SetFloat(name, valueVector.x);
                        break;
                    case ValueType.TexEnv:
                        mat.SetTexture(name, valueTexture);
                        break;
                    case ValueType.TexOfs:
                        mat.SetTextureOffset(name, valueVector);
                        break;
                    case ValueType.TexScale:
                        mat.SetTextureScale(name, valueVector);
                        break;
                }
            }

            public void SetValueFrom(Material mat) {
                switch(valueType) {
                    case ValueType.Color:
                        valueVector = mat.GetColor(name);
                        break;
                    case ValueType.Vector:
                        valueVector = mat.GetVector(name);
                        break;
                    case ValueType.Float:
                    case ValueType.Range:
                        valueVector.x = mat.GetFloat(name);
                        break;
                    case ValueType.TexEnv:
                        valueTexture = mat.GetTexture(name);
                        break;
                    case ValueType.TexOfs:
                        valueVector = mat.GetTextureOffset(name);
                        break;
                    case ValueType.TexScale:
                        valueVector = mat.GetTextureScale(name);
                        break;
                }
            }
        }

        public PropertyInfo[] properties;
        public bool applyOnAwake = true;

        public Renderer renderTarget { 
            get {
                if(!mRenderer)
                    mRenderer = GetComponent<Renderer>();
                return mRenderer;
            } 
        }

        private Renderer mRenderer;
        private Material mMaterial;

        public void Apply() {
            if(!mMaterial) {
                if(!mRenderer)
                    mRenderer = GetComponent<Renderer>();

                mMaterial = new Material(mRenderer.sharedMaterial);
                mRenderer.sharedMaterial = mMaterial;
            }

            for(int i = 0; i < properties.Length; i++)
                properties[i].Apply(mMaterial);
        }

        void OnDestroy() {
            if(mMaterial)
                DestroyImmediate(mMaterial);
        }

        void Awake() {
            if(applyOnAwake)
                Apply();
        }
    }
}