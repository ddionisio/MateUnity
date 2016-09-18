using UnityEngine;
using System.Collections.Generic;

namespace M8.UIModal {
    public struct ParamArg {
        public string key;
        public object value;

        public ParamArg(string aKey, object aValue) {
            key = aKey;
            value = aValue;
        }
    }

    public class Params : Dictionary<string, object> {
        public Params(params ParamArg[] args) : base(args.Length) {
            for(int i = 0; i < args.Length; i++)
                Add(args[i].key, args[i].value);
        }
    }
}
