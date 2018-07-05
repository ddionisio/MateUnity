using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    [System.Serializable]
    public struct JSONList<T> {
        [SerializeField]
        private List<T> items;

        public static List<T> FromJSON(string json) {
            return !string.IsNullOrEmpty(json) ? JsonUtility.FromJson<JSONList<T>>(json).items : new List<T>();
        }

        public static string ToJSON(List<T> _items, bool prettyPrint) {
            return JsonUtility.ToJson(new JSONList<T>() { items = _items }, prettyPrint);
        }
    }
}