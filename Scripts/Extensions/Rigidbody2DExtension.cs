using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    public static class Rigidbody2DExtension {
#if !M8_PHYSICS2D_DISABLED
        public static void AddExplosionForce(this Rigidbody2D body, float explosionForce, Vector2 explosionPosition, float explosionRadius, bool applyWearOff, ForceMode2D mode) {
            var delta = (body.position - explosionPosition);
            var dist = delta.magnitude;

            if(dist > 0f) {
                if(applyWearOff) {
                    float wearoff = 1 - (dist / explosionRadius);
                    if(wearoff > 0f) {
                        var dir = delta / dist;
                        Vector3 baseForce = dir * (explosionForce * wearoff);
                        body.AddForce(baseForce, mode);
                    }
                }
                else {
                    var dir = delta / dist;
                    Vector3 baseForce = dir * explosionForce;
                    body.AddForce(baseForce, mode);
                }
            }
        }

        public static void AddExplosionForce(this Rigidbody2D body, float explosionForce, Vector2 explosionPosition, float explosionRadius, float upwardsModifier, bool applyWearOff, ForceMode2D mode) {
            var delta = (body.position - explosionPosition);

            float explodeDist = delta.magnitude;

            if(applyWearOff) {
                float wearoff = 1 - (explodeDist / explosionRadius);
                if(wearoff > 0f) {
                    float deltaDist;
                    if(upwardsModifier != 0f) {
                        delta.y += upwardsModifier;

                        deltaDist = delta.magnitude;
                    }
                    else
                        deltaDist = explodeDist;

                    if(deltaDist > 0f) {
                        var dir = delta / deltaDist;

                        Vector3 baseForce = dir * (explosionForce * wearoff);
                        body.AddForce(baseForce, mode);
                    }
                }
            }
            else {
                float deltaDist;
                if(upwardsModifier != 0f) {
                    delta.y += upwardsModifier;

                    deltaDist = delta.magnitude;
                }
                else
                    deltaDist = explodeDist;

                if(deltaDist > 0f) {
                    var dir = delta / deltaDist;

                    Vector3 baseForce = dir * explosionForce;
                    body.AddForce(baseForce, mode);
                }
            }
        }

        public static void AddExplosionForceAtPosition(this Rigidbody2D body, Vector2 bodyPosition, float explosionForce, Vector2 explosionPosition, float explosionRadius, bool applyWearOff, ForceMode2D mode) {
            var delta = (body.position - explosionPosition);
            var dist = delta.magnitude;

            if(dist > 0f) {
                if(applyWearOff) {
                    float wearoff = 1 - (dist / explosionRadius);
                    if(wearoff > 0f) {
                        var dir = delta / dist;
                        Vector3 baseForce = dir * (explosionForce * wearoff);
                        body.AddForceAtPosition(baseForce, bodyPosition, mode);
                    }
                }
                else {
                    var dir = delta / dist;
                    Vector3 baseForce = dir * explosionForce;
                    body.AddForceAtPosition(baseForce, bodyPosition, mode);
                }
            }
        }

        public static void AddExplosionForceAtPosition(this Rigidbody2D body, Vector2 bodyPosition, float explosionForce, Vector2 explosionPosition, float explosionRadius, float upwardsModifier, bool applyWearOff, ForceMode2D mode) {
            var delta = (body.position - explosionPosition);

            float explodeDist = delta.magnitude;

            if(applyWearOff) {
                float wearoff = 1 - (explodeDist / explosionRadius);
                if(wearoff > 0f) {
                    float deltaDist;
                    if(upwardsModifier != 0f) {
                        delta.y += upwardsModifier;

                        deltaDist = delta.magnitude;
                    }
                    else
                        deltaDist = explodeDist;

                    if(deltaDist > 0f) {
                        var dir = delta / deltaDist;

                        Vector3 baseForce = dir * (explosionForce * wearoff);
                        body.AddForceAtPosition(baseForce, bodyPosition, mode);
                    }
                }
            }
            else {
                float deltaDist;
                if(upwardsModifier != 0f) {
                    delta.y += upwardsModifier;

                    deltaDist = delta.magnitude;
                }
                else
                    deltaDist = explodeDist;

                if(deltaDist > 0f) {
                    var dir = delta / deltaDist;

                    Vector3 baseForce = dir * explosionForce;
                    body.AddForceAtPosition(baseForce, bodyPosition, mode);
                }
            }
        }
#endif
    }
}