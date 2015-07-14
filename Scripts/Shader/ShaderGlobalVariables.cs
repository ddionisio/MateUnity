using UnityEngine;
using System.Collections;

namespace M8 {
    /// <summary>
    /// Use this to apply global variables for shader during initialization or manually applying.
    /// </summary>
    [AddComponentMenu("M8/Shader/Global Variables")]
    public class ShaderGlobalVariables : MonoBehaviour {
        public enum Type {
            Float,
            Vector,
            Color,
            Texture
        }

        [System.Serializable]
        public class Data {
            public string name;

            public Type type;

            [SerializeField]
            private Vector4 mVal4;
            [SerializeField]
            private Texture mValTex;

            public float val { get { return mVal4.x; } set { mVal4.x = value; } }
            public Vector4 val4 { get { return mVal4; } set { mVal4 = value; } }
            public Color color { get { return new Color(mVal4.x, mVal4.y, mVal4.z, mVal4.w); } set { mVal4.Set(value.r, value.g, value.b, value.a); } }
            public Texture texture { get { return mValTex; } set { mValTex = value; } }

            public Data Clone() {
                return new Data() { name=name, type=type, mVal4=mVal4, mValTex=mValTex };
            }

            public void Apply() {
                switch(type) {
                    case Type.Float:
                        Shader.SetGlobalFloat(name, val);
                        break;
                    case Type.Vector:
                        Shader.SetGlobalVector(name, val4);
                        break;
                    case Type.Color:
                        Shader.SetGlobalColor(name, color);
                        break;
                    case Type.Texture:
                        Shader.SetGlobalTexture(name, texture);
                        break;
                }
            }
        }

        public Data[] values;
        public bool applyOnAwake = true;

        public void Apply() {
            if(values != null) {
                for(int i = 0; i < values.Length; i++)
                    values[i].Apply();
            }
        }

        void Awake() {
            if(applyOnAwake)
                Apply();
        }
    }
}