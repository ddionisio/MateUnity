using UnityEngine;

namespace M8 {
    public struct MathUtil {
        public enum Side {
            None,
            Left,
            Right
        }

        public const float TwoPI = 2.0f * Mathf.PI;
        public const float HalfPI = 0.5f * Mathf.PI;
                
        //------------- Interpolates ---------------

        /// <summary>
        /// Cosine interpolation
        /// </summary>
        public static float Clerp(float start, float end, float t) {
            float ft = t * Mathf.PI;
            float f = (1.0f - Mathf.Cos(ft))*0.5f;
            return start*(1.0f-f) + end*f;
        }

        //-------------- 2D --------------

        public static void Limit(ref Vector2 v, float limit) {
            float dSqr = v.sqrMagnitude;
            if(dSqr > limit * limit) {
                v /= Mathf.Sqrt(dSqr);
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

        public static Vector2 Rotate(Vector2 v, float radians) {
            float c = Mathf.Cos(radians);
            float s = Mathf.Sin(radians);

            return new Vector2(v.x * c + v.y * s, -v.x * s + v.y * c);
        }

        public static Vector2 RotateAngle(Vector2 v, float angle) {
            return Rotate(v, angle * Mathf.Deg2Rad);
        }

        public static float Cross(Vector2 v1, Vector2 v2) {
            return (v1.x * v2.y) - (v1.y * v2.x);
        }

        public static Vector2 Perpendicular(Vector2 v) {
            return new Vector2(-v.y, v.x);
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

        public static Vector2 Hermite(Vector2 v1, Vector2 t1, Vector2 v2, Vector2 t2, float s) {
            float s2 = s * s, s3 = s2 * s;

            float h1 = 2 * s3 - 3 * s2 + 1;          // calculate basis function 1
            float h2 = -2 * s3 + 3 * s2;              // calculate basis function 2
            float h3 = s3 - 2 * s2 + s;         // calculate basis function 3
            float h4 = s3 - s2;              // calculate basis function 4

            return new Vector2(
                h1 * v1.x + h2 * v2.x + h3 * t1.x + h4 * t2.x,
                h1 * v1.y + h2 * v2.y + h3 * t1.y + h4 * t2.y);
        }

        public static Vector2 CatMullRom(Vector2 v1, Vector2 v2, Vector2 v3, Vector2 v4, float s) {
            Vector2 t1 = new Vector2((v3.x - v1.x) * 0.5f, (v3.y - v1.y) * 0.5f);
            Vector2 t2 = new Vector2((v4.x - v2.x) * 0.5f, (v4.y - v2.y) * 0.5f);

            return Hermite(v2, t1, v3, t2, s);
        }

        /// <summary>
        /// Compute the Barycentric (a,b,c) based on given p in relation to triangle (p0, p1, p2)
        /// </summary>
        public static Vector3 Barycentric(Vector2 p, Vector2 p0, Vector2 p1, Vector2 p2) {
            Vector2 v0 = p1 - p0, v1 = p2 - p0, v2 = p - p0;
            float d00 = Vector2.Dot(v0, v0);
            float d01 = Vector2.Dot(v0, v1);
            float d11 = Vector2.Dot(v1, v1);
            float d20 = Vector2.Dot(v2, v0);
            float d21 = Vector2.Dot(v2, v1);
            float denom = d00*d11 - d01*d01;
            Vector3 ret;
            ret.y = (d11 * d20 - d01 * d21) / denom;
            ret.z = (d00 * d21 - d01 * d20) / denom;
            ret.x = 1.0f - ret.y - ret.z;
            return ret;
        }

        public static Vector2 Bezier(Vector2 p0, Vector2 p1, Vector2 p2, float t) {
            if(t == 0f) return p0;
            if(t == 1f) return p2;

            float tInv = (1 - t);
            float a = tInv * tInv;
            float b = 2 * tInv * t;
            float c = t * t;

            return new Vector2(a * p0.x + b * p1.x + c * p2.x, a * p0.y + b * p1.y + c * p2.y);
        }

        //-------------- 3D --------------

        public static bool CompareApprox(Vector3 v1, Vector3 v2, float approx) {
            if(Mathf.Abs(v2.x - v1.x) > approx) return false;

            if(Mathf.Abs(v2.y - v1.y) > approx) return false;

            if(Mathf.Abs(v2.z - v1.z) > approx) return false;

            return true;
        }

        /// <summary>
        /// Caps given destDir with angleLimit (degree) of srcDir, returns dir.
        /// </summary>
        /// <returns>
        /// The capped dir
        /// </returns>
        public static Vector3 DirCap(Vector3 srcDir, Vector3 destDir, float angleLimit) {
            float angle = Mathf.Acos(Vector3.Dot(srcDir, destDir));

            if(Mathf.Abs(angle) > angleLimit * Mathf.Deg2Rad) {
                Vector3 cross = Vector3.Cross(srcDir, destDir);
                Quaternion r = Quaternion.AngleAxis(angleLimit, cross);
                return r * srcDir;
            }

            return destDir;
        }

        public static Vector3 Steer(Vector3 velocity, Vector3 desired, float cap, float factor) {
            return Limit(desired - velocity, cap) * factor;
        }

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

        public static Vector3 Reflect(Vector3 v, Vector3 n) {
            return v - (2.0f * Vector3.Dot(v, n)) * n;
        }

        public static Vector3 Slide(Vector3 v, Vector3 n) {
            return v - Vector3.Dot(v, n) * n;
        }

        public static float DistanceSqr(Vector3 pt1, Vector3 pt2) {
            return Vector3.SqrMagnitude(pt1 - pt2);
        }

        public static Vector3 MidPoint(Vector3 pt1, Vector3 pt2) {
            return new Vector3((pt1.x + pt2.x) * 0.5f, (pt1.y + pt2.y) * 0.5f, (pt1.z + pt2.z) * 0.5f);
        }

        /// <summary>
        /// get the square distance from a point to a line segment.
        /// </summary>
        /// <param name="lineP1">line segment start point</param>
        /// <param name="lineP2">line segment end point</param>
        /// <param name="point">point to get distance to</param>
        /// <param name="closestPoint">set to either 1, 2, or 4, determining which end the point is closest to (p1, p2, or the middle)</param>
        /// <returns></returns>
        public static float DistanceToLineSqr(Vector3 lineP1, Vector3 lineP2, Vector3 point, out int closestPoint) {
            Vector3 v = lineP2 - lineP1;
            Vector3 w = point - lineP1;

            float c1 = Vector3.Dot(w, v);

            if(c1 <= 0) {//closest point is p1
                closestPoint = 1;
                return DistanceSqr(point, lineP1);
            }

            float c2 = Vector3.Dot(v, v);

            if(c2 <= c1) {//closest point is p2
                closestPoint = 2;
                return DistanceSqr(point, lineP2);
            }

            float b = c1 / c2;
            Vector3 pb = lineP1 + b * v;

            closestPoint = 4;
            return DistanceSqr(point, pb);
        }

        /// <summary>
        /// Get the angle in degrees between given forward axis to target position. WorldToLocal determines the
        /// axis' space relative to given point.
        /// If you want to create a rotation, use Quaternion.AngleAxis(angle, Vector3.up);
        /// </summary>
        public static float AngleForwardAxisDir(Matrix4x4 worldToLocal, Vector3 axis, Vector3 dir) {
            dir = worldToLocal.MultiplyVector(dir);
            dir.y = 0.0f;

            float s = M8.MathUtil.CheckSideSign(new Vector2(dir.x, dir.z), new Vector2(axis.x, axis.z));

            float angle = Vector3.Angle(axis, dir);

            return s * angle;
        }

        /// <summary>
        /// Get the angle in degrees between given forward axis to target position. WorldToLocal determines the
        /// axis' space relative to given point.
        /// If you want to create a rotation, use Quaternion.AngleAxis(angle, Vector3.up);
        /// </summary>
        public static float AngleForwardAxis(Matrix4x4 worldToLocal, Vector3 point, Vector3 axis, Vector3 target) {
            return AngleForwardAxisDir(worldToLocal, axis, target - point);
        }

        public static bool RotateToUp(Vector3 up, Vector3 right, Vector3 forward, ref Quaternion rotate) {
            Vector3 f = Vector3.Cross(up, right);
            if(f == Vector3.zero) {
                Vector3 l = Vector3.Cross(up, forward);
                f = Vector3.Cross(l, up);
            }

            if(f != Vector3.zero) {
                rotate = Quaternion.LookRotation(f, up);
                return true;
            }

            return false;
        }

        public static bool VectorEqualApproximately(Vector3 lhs, Vector3 rhs) {
            return Mathf.Approximately(lhs.x, rhs.x) && Mathf.Approximately(lhs.y, rhs.y) && Mathf.Approximately(lhs.z, rhs.z);
        }

        /// <summary>
        /// Compute the Barycentric (a,b,c) based on given p in relation to triangle (p0, p1, p2)
        /// </summary>
        public static Vector3 Barycentric(Vector3 p, Vector3 p0, Vector3 p1, Vector3 p2) {
            Vector3 v0 = p1 - p0, v1 = p2 - p0, v2 = p - p0;
            float d00 = Vector3.Dot(v0, v0);
            float d01 = Vector3.Dot(v0, v1);
            float d11 = Vector3.Dot(v1, v1);
            float d20 = Vector3.Dot(v2, v0);
            float d21 = Vector3.Dot(v2, v1);
            float denom = d00*d11 - d01*d01;
            Vector3 ret;
            ret.y = (d11 * d20 - d01 * d21) / denom;
            ret.z = (d00 * d21 - d01 * d20) / denom;
            ret.x = 1.0f - ret.y - ret.z;
            return ret;
        }
    }

	//-------------- Bounds --------------
    public struct BoundsUtil {
        public static bool Intersect(Bounds b1, Bounds b2, out Bounds bOut) {
            bool ret = b1.Intersects(b2);
            if(ret) {
                bOut = new Bounds();
                bOut.SetMinMax(
                    Vector3.Max(b1.min, b2.min),
                    Vector3.Min(b1.max, b2.max));
            }
            else
                bOut = b1;
            return ret;
        }
    }

    //-------------- Easing --------------

    public struct Easing {
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
