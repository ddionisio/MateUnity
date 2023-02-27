using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace M8 {
    public struct GenericParamArg {
        public string key;
        public object value;

        public GenericParamArg(string aKey, object aValue) {
            key = aKey;
            value = aValue;
        }
    }

    public class GenericParams : Dictionary<string, object> {
        public GenericParams(params GenericParamArg[] args) : base(args.Length) {
            for(int i = 0; i < args.Length; i++)
                Add(args[i].key, args[i].value);
        }

        public new object this[string key] {
            get {
                object val;
                base.TryGetValue(key, out val);
                return val;
            }

            set {
                if(ContainsKey(key))
                    base[key] = value;
                else
                    Add(key, value);
            }
        }

        public T GetValue<T>(string key) {
            return (T)this[key];
        }

        public bool TryGetValue<T>(string key, out T val) {
            object valObj;
            if(base.TryGetValue(key, out valObj)) {
                val = (T)valObj;
                return true;
            }
            
            val = default(T);
            return false;
        }

        /// <summary>
        /// Merge other to this container. If isOverride=true, then override matching items with other's.
        /// </summary>
        public void Merge(GenericParams other, bool isOverride) {
            if(other == null)
                return;

            foreach(var pair in other) {
                if(ContainsKey(pair.Key)) {
                    if(isOverride)
                        this[pair.Key] = pair.Value;
                }
                else
                    Add(pair.Key, pair.Value);
            }
        }
    }

    /// <summary>
    /// Use this as a field for components to customize parameters via inspector
    /// </summary>
    [System.Serializable]
    public struct GenericParamSerialized {
        public enum ValueType {
            Boolean,
            Int,
            Float,
            String,
            Vector2,
            Vector3,
            Vector4,
            Object
        }

        public string key;

        public ValueType type;

        public int iVal;
        public Vector4 vectorVal;
        public string sVal;
        public Object oVal;

        public static GenericParams GenerateParams(GenericParamSerialized[] paramFields) {
            var parms = new GenericParams();
            for(int i = 0; i < paramFields.Length; i++)
                paramFields[i].ApplyTo(parms);
            return parms;
        }

        public static void ApplyAll(GenericParams parms, GenericParamSerialized[] paramFields) {
            for(int i = 0; i < paramFields.Length; i++)
                paramFields[i].ApplyTo(parms);
        }

        public void ApplyTo(GenericParams parms) {
            switch(type) {
                case ValueType.Boolean:
                    parms[key] = iVal > 0 ? true : false;
                    break;
                case ValueType.Int:
                    parms[key] = iVal;
                    break;
                case ValueType.Float:
                    parms[key] = vectorVal.x;
                    break;
                case ValueType.String:
                    parms[key] = sVal;
                    break;
                case ValueType.Vector2:
                    parms[key] = new Vector2 { x = vectorVal.x, y = vectorVal.y };
                    break;
                case ValueType.Vector3:
                    parms[key] = new Vector3 { x = vectorVal.x, y = vectorVal.y, z = vectorVal.z };
                    break;
                case ValueType.Vector4:
                    parms[key] = vectorVal;
                    break;
                case ValueType.Object:
                    parms[key] = oVal;
                    break;
            }
        }
    }

    [System.Serializable]
    public class UnityEventGenericParams : UnityEvent<GenericParams> {
    }
}
