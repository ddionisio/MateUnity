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

    [System.Serializable]
    public class UnityEventGenericParams : UnityEvent<GenericParams> {
    }
}
