using System;
using UnityEngine;

namespace M8 {
    /// <summary>
    /// Used for paths that are relative to Resources
    /// </summary>
    public class ResourcePathAttribute : Attribute {
        public readonly string path;

        public ResourcePathAttribute(string path) {
            this.path = path;
        }
    }
}