using UnityEngine;

namespace M8 {
    public struct ColorUtil {
        public static Color Lerp(Color[] colors, float t) {
            if(colors.Length == 1)
                return colors[0];

            int end = colors.Length - 1;

            if(t >= 1.0f) {
                return colors[end];
            }
            else if(t <= 0.0f) {
                return colors[0];
            }
            else {
                float indOfs = t * end;
                int ind = Mathf.FloorToInt(indOfs);

                if(ind == end)
                    return colors[ind];

                return Color.Lerp(colors[ind], colors[ind + 1], indOfs - ind);
            }
        }

        public static Color Lerp(Color[] colors, int start, int count, float t) {
            if(count == 1)
                return colors[start];

            int endOfs = count - 1;

            if(t >= 1.0f) {
                return colors[start + endOfs];
            }
            else if(t <= 0.0f) {
                return colors[start];
            }
            else {
                float fIndOfs = t * endOfs;
                int indOfs = Mathf.FloorToInt(fIndOfs);

                int ind = start + indOfs;

                if(indOfs >= endOfs)
                    return colors[ind];

                return Color.Lerp(colors[ind], colors[ind + 1], fIndOfs - ind);
            }
        }

        public static Color LerpRepeat(Color[] colors, float t) {
            if(colors.Length == 1)
                return colors[0];

            int end = colors.Length - 1;

            float indOfs = t * end;
            int ind = Mathf.FloorToInt(indOfs) % end;
            
            if(ind < 0) ind = end-ind;

            int indNext = (ind + 1) % end;

            return Color.Lerp(colors[ind], colors[indNext], indOfs - ind);
        }
    }
}
