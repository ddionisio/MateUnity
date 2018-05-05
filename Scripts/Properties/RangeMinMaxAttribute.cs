using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    public class RangeMinMaxAttribute : PropertyAttribute {
        public readonly float minLimit;
        public readonly float maxLimit;

        //
        // Summary:
        //     Optional field to specify the min value field name (default: min).
        public string minFieldName { get; set; }

        //
        // Summary:
        //     Optional field to specify the max value field name (default: max).
        public string maxFieldName { get; set; }

        public RangeMinMaxAttribute(float minLimit, float maxLimit) {
            this.minLimit = minLimit;
            this.maxLimit = maxLimit;
        }
    }
}
