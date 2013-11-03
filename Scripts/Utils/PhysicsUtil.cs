using UnityEngine;
//using System.Collections.Generic;

namespace M8 {
    public struct PhysicsUtil {
        public const CollisionFlags anyCollisionFlags = CollisionFlags.Above | CollisionFlags.Below | CollisionFlags.Sides;

        const float defaultSphereCosCheck = 0.86602540378443864676372317075294f;
        const float defaultBoxCosCheck = 0.70710678118654752440084436210485f;

        //cheaper version if you pre-cache the cos(angleLimit)
        public static CollisionFlags GetCollisionFlagsSphereCos(Vector3 up, Vector3 center, float cosLimit, Vector3 contactPoint) {
            Vector3 delta = contactPoint - center;
            delta.Normalize();

            float dot = Vector3.Dot(up, delta);

            if(dot < -cosLimit)
                return CollisionFlags.Below;
            else if(dot > cosLimit)
                return CollisionFlags.Above;

            return CollisionFlags.Sides;
        }

        public static CollisionFlags GetCollisionFlagsSphere(Vector3 up, Vector3 center, float angleLimit, Vector3 contactPoint) {
            return GetCollisionFlagsSphereCos(up, center, Mathf.Cos(angleLimit), contactPoint);
        }

        public static CollisionFlags GetCollisionFlagsSphere(Vector3 up, Vector3 center, Vector3 contactPoint) {
            return GetCollisionFlagsSphereCos(up, center, defaultSphereCosCheck, contactPoint);
        }

        public static CollisionFlags GetCollisionFlagsCapsule(Transform t, float height, float radius, float aboveOfs, float belowOfs, Vector3 localCenter, Vector3 contactPoint) {
            CollisionFlags ret = CollisionFlags.None;

            float midLine = (height - (radius * 2.0f)) * 0.5f;
            float upLine = midLine + aboveOfs;
            float downLine = midLine + belowOfs;

            Vector3 lp = t.worldToLocalMatrix.MultiplyPoint(contactPoint) - localCenter;


            if(lp.y > upLine) {
                ret |= CollisionFlags.Above;
                //Debug.Log("lp: " + lp);
            }

            if(lp.y < -downLine) {
                ret |= CollisionFlags.Below;
            }

            if(lp.y >= -downLine && lp.y <= upLine)
                ret |= CollisionFlags.Sides;

            return ret;
        }

        public static CollisionFlags GetCollisionFlagsCapsule(Vector3 up, Vector3 scale, Vector3 center, float height, float radius, Vector3 contactPoint) {
            Vector3 p = (up * ((height - (radius * 2.0f)) * 0.5f));
            p = Vector3.Scale(p, scale);
            Vector3 bottom = center - p;
            Vector3 top = center + p;

            int cPt = 0;
            M8.MathUtil.DistanceToLineSqr(bottom, top, contactPoint, out cPt);

            if(cPt == 1)
                return CollisionFlags.Below;
            else if(cPt == 2)
                return CollisionFlags.Above;

            return CollisionFlags.Sides;
        }

        public static CollisionFlags GetCollisionFlagsBox(Vector3 up, Vector3 center, Vector3 contactPoint) {
            Vector3 delta = contactPoint - center;
            delta.Normalize();

            float dot = Vector3.Dot(up, delta);

            if(dot < -defaultBoxCosCheck)
                return CollisionFlags.Below;
            else if(dot > defaultBoxCosCheck)
                return CollisionFlags.Above;

            return CollisionFlags.Sides;
        }

        public static int[] GetLayerIndices(int layerMask) {
            int[] layers = new int[32];
            int numLayers = 0;

            for(int i = 0; layerMask != 0; layerMask >>= 1, i++) {
                if((layerMask & 1) != 0) {
                    layers[numLayers] = i;
                    numLayers++;
                }
            }
                        
            if(numLayers > 0) {
                System.Array.Resize(ref layers, numLayers);
                return layers;
            }

            return null;
        }
    }
}