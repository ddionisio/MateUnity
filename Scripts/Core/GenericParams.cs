using UnityEngine;
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
    }
}
