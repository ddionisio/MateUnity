using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    public static class Rigidbody2DExtension {
        public static void AddExplosionForce(this Rigidbody2D body, float explosionForce, Vector2 explosionPosition, float explosionRadius, ForceMode2D mode) {
            var delta = (body.position - explosionPosition);
            var dist = delta.magnitude;
            
            float wearoff = 1 - (dist / explosionRadius);
            if(wearoff > 0f) {
                var dir = delta / dist;

                Vector3 baseForce = dir * (explosionForce * wearoff);
                body.AddForce(baseForce, mode);
            }
        }

        public static void AddExplosionForce(this Rigidbody2D body, float explosionForce, Vector2 explosionPosition, float explosionRadius, float upwardsModifier, ForceMode2D mode) {
            var delta = (body.position - explosionPosition);
                        
            float explodeDist = delta.magnitude;
                        
            float wearoff = 1 - (explodeDist / explosionRadius);
            if(wearoff > 0f) {
                float deltaDist;
                if(upwardsModifier != 0f) {
                    delta.y += upwardsModifier;

                    deltaDist = delta.magnitude;
                }
                else
                    deltaDist = explodeDist;

                var dir = delta / deltaDist;

                Vector3 baseForce = dir * (explosionForce * wearoff);                                
                body.AddForce(baseForce, mode);
            }
        }

        public static void AddExplosionForceAtPosition(this Rigidbody2D body, Vector2 bodyPosition, float explosionForce, Vector2 explosionPosition, float explosionRadius, ForceMode2D mode) {
            var delta = (body.position - explosionPosition);
            var dist = delta.magnitude;

            float wearoff = 1 - (dist / explosionRadius);
            if(wearoff > 0f) {
                var dir = delta / dist;

                Vector3 baseForce = dir * (explosionForce * wearoff);
                body.AddForceAtPosition(baseForce, bodyPosition, mode);
            }
        }

        public static void AddExplosionForceAtPosition(this Rigidbody2D body, Vector2 bodyPosition, float explosionForce, Vector2 explosionPosition, float explosionRadius, float upwardsModifier, ForceMode2D mode) {
            var delta = (body.position - explosionPosition);

            float explodeDist = delta.magnitude;

            float wearoff = 1 - (explodeDist / explosionRadius);
            if(wearoff > 0f) {
                float deltaDist;
                if(upwardsModifier != 0f) {
                    delta.y += upwardsModifier;

                    deltaDist = delta.magnitude;
                }
                else
                    deltaDist = explodeDist;

                var dir = delta / deltaDist;

                Vector3 baseForce = dir * (explosionForce * wearoff);
                body.AddForceAtPosition(baseForce, bodyPosition, mode);
            }
        }
    }
}