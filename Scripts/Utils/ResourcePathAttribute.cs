using System;
using UnityEngine;

namespace M8 {
    public class ResourcePathAttribute : Attribute {
        public readonly string path;

        public ResourcePathAttribute(string path) {
            this.path = path;
        }
    }
}