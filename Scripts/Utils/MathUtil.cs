using UnityEngine;

namespace M8 {
    public struct MathUtil {
        public enum Side {
            None,
            Left,
            Right
        }

        public const float TwoPI = 2.0f * Mathf.PI;

        public static int CellToIndex(int row, int col, int numCols) {
            return (row * numCols) + col;
        }

        public static void CellToRowCol(int index, int numCols, out int row, out int col) {
            row = index / numCols;
            col = index % numCols;
        }

        //-------------- 2D --------------

        public static void Limit(ref Vector2 v, float limit) {
            float d = v.magnitude;
            if(d > limit) {
                v /= d;
                v *= limit;
            }
        }

        public static Vector2 Limit(Vector2 v, float limit) {
            Limit(ref v, limit);
            return v;
        }

        public static Vector2 Reflect(Vector2 v, Vector2 n) {
            return v - (2.0f * Vector2.Dot(v, n)) * n;
        }

        public static Vector2 Slide(Vector2 v, Vector2 n) {
            return v - Vector2.Dot(v, n) * n;
        }

        public static float CheckSideSign(Vector2 up1, Vector2 up2) {
            return Cross(up1, up2) < 0 ? -1 : 1;
        }

        /// <summary>
        /// Checks which side up1 is in relation to up2
        /// </summary>
        public static Side CheckSide(Vector2 up1, Vector2 up2) {
            float s = Cross(up1, up2);
            return s == 0 ? Side.None : s < 0 ? Side.Right : Side.Left;
        }

        public static Vector2 Rotate(Vector2 v, float r) {
            float c = Mathf.Cos(r);
            float s = Mathf.Sin(r);

            return new Vector2(v.x * c + v.y * s, -v.x * s + v.y * c);
        }

        public static float Cross(Vector2 v1, Vector2 v2) {
            return (v1.x * v2.y) - (v1.y * v2.x);
        }

        /// <summary>
        /// Caps given destDir with angleLimit (degree) on either side of srcDir, returns which side the destDir is capped relative to srcDir.
        /// </summary>
        /// <returns>
        /// The side destDir is relative to srcDir. (-1 or 1)
        /// </returns>
        public static float DirCap(Vector2 srcDir, ref Vector2 destDir, float angleLimit) {

            float side = CheckSideSign(srcDir, destDir);

            float angle = Mathf.Acos(Vector2.Dot(srcDir, destDir));

            float limitAngle = angleLimit * Mathf.Deg2Rad;

            if(angle > limitAngle) {
                destDir = Rotate(srcDir, -side * limitAngle);
            }

            return side;
        }

        public static Vector2 Steer(Vector2 velocity, Vector2 desired, float cap, float factor) {
            return Limit(desired - velocity, cap) * factor;
        }

        //-------------- 3D --------------

        public static void Limit(ref Vector3 v, float limit) {
            float d = v.magnitude;
            if(d > limit) {
                v /= d;
                v *= limit;
            }
        }

        public static Vector3 Limit(Vector3 v, float limit) {
            Limit(ref v, limit);
            return v;
        }
    }

    public struct Ease {
        public static float In(float t, float tMax, float start, float delta) {
            return start + delta * _in(t / tMax);
        }

        private static float _in(float r) {
            return r * r * r;
        }

        public static float Out(float t, float tMax, float start, float delta) {
            return start + delta * _out(t / tMax);
        }

        private static float _out(float r) {
            float ir = r - 1.0f;
            return ir * ir * ir + 1.0f;
        }

        public static float OutElastic(float t, float tMax, float start, float delta) {
            return start + (delta * _outElastic(t / tMax));
        }

        private static float _outElastic(float ratio) {
            if(ratio == 0.0f || ratio == 1.0f) return ratio;

            float p = 0.3f;
            float s = p / 4.0f;
            return -1.0f * Mathf.Pow(2.0f, -10.0f * ratio) * Mathf.Sin((ratio - s) * 2.0f * Mathf.PI / p) + 1.0f;
        }

        public static float InBounce(float t, float tMax, float start, float delta) {
            return start + (delta * _inBounce(t / tMax));
        }

        private static float _inBounce(float ratio) {
            return 1.0f - _outBounce(1.0f - ratio);
        }

        private static float _outBounce(float ratio) {
            float s = 7.5625f;
            float p = 2.75f;
            float l;
            if(ratio < (1.0f / p))
                l = s * Mathf.Pow(ratio, 2.0f);
            else {
                if(ratio < (2.0f / p)) {
                    ratio = ratio - (1.5f / p);
                    l = s * Mathf.Pow(ratio, 2.0f) + 0.75f;
                }
                else {
                    if(ratio < (2.5f / p)) {
                        ratio = ratio - (2.25f / p);
                        l = s * Mathf.Pow(ratio, 2.0f) + 0.9375f;
                    }
                    else {
                        ratio = ratio - (2.65f / p);
                        l = s * Mathf.Pow(ratio, 2.0f) + 0.984375f;
                    }
                }
            }
            return l;
        }

        public static float InElastic(float t, float tMax, float start, float delta) {
            return start + (delta * _inElastic(t / tMax));
        }

        private static float _inElastic(float ratio) {
            if(ratio == 0.0f || ratio == 1.0f) return ratio;

            float p = 0.3f;
            float s = p / 4.0f;
            float invRatio = ratio - 1.0f;
            return -1 * Mathf.Pow(2.0f, 10.0f * invRatio) * Mathf.Sin((invRatio - s) * 2.0f * Mathf.PI / p);
        }
    }
}
