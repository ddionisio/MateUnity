using UnityEngine;

namespace M8 {
    public struct Gizmo {
        public static void ArrowFourLine(Vector3 pos, Vector3 direction, float length, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f) {
            Vector3 end = pos + direction * length;

            Gizmos.DrawLine(pos, end);

            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Gizmos.DrawRay(end, right * arrowHeadLength);
            Gizmos.DrawRay(end, left * arrowHeadLength);

            Vector3 up = Quaternion.LookRotation(direction) * Quaternion.Euler(180 + arrowHeadAngle, 0, 0) * new Vector3(0, 0, 1);
            Vector3 down = Quaternion.LookRotation(direction) * Quaternion.Euler(180 - arrowHeadAngle, 0, 0) * new Vector3(0, 0, 1);
            Gizmos.DrawRay(end, up * arrowHeadLength);
            Gizmos.DrawRay(end, down * arrowHeadLength);
        }

        public static void Arrow(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f) {
            Gizmos.DrawRay(pos, direction);

            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
            Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
        }

        public static void Arrow(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f) {
            Gizmos.color = color;
            Gizmos.DrawRay(pos, direction);

            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
            Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
        }

        public static void ArrowLine(Vector3 start, Vector3 end, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f) {
            Gizmos.DrawLine(start, end);

            Vector3 dir = (end - start).normalized;

            Vector3 right = Quaternion.LookRotation(dir) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Vector3 left = Quaternion.LookRotation(dir) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Gizmos.DrawRay(end, right * arrowHeadLength);
            Gizmos.DrawRay(end, left * arrowHeadLength);
        }

        public static void ArrowLine(Vector3 start, Vector3 end, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f) {
            Gizmos.color = color;

            Gizmos.DrawLine(start, end);

            Vector3 dir = (end - start).normalized;

            Vector3 right = Quaternion.LookRotation(dir) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Vector3 left = Quaternion.LookRotation(dir) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Gizmos.DrawRay(end, right * arrowHeadLength);
            Gizmos.DrawRay(end, left * arrowHeadLength);
        }

        public static void ArrowLine2D(Vector2 start, Vector2 end, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f) {
            Gizmos.DrawLine(start, end);

            Vector2 dir = (end - start).normalized;

            Vector2 right = MathUtil.Rotate(dir, 180f - arrowHeadAngle);
            Vector2 left = MathUtil.Rotate(dir, -180f + arrowHeadAngle);
            
            Gizmos.DrawRay(end, right * arrowHeadLength);
            Gizmos.DrawRay(end, left * arrowHeadLength);
        }

        public static void ArrowLine2D(Vector3 start, Vector3 end, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f) {
            Gizmos.color = color;

            Gizmos.DrawLine(start, end);

            Vector2 dir = (end - start).normalized;

            Vector2 right = MathUtil.Rotate(dir, 180f - arrowHeadAngle);
            Vector2 left = MathUtil.Rotate(dir, -180f + arrowHeadAngle);

            Gizmos.DrawRay(end, right * arrowHeadLength);
            Gizmos.DrawRay(end, left * arrowHeadLength);
        }

        public static void DrawWireCube(Vector3 position, Vector3 size) {
            var half = size / 2;
            // draw front
            Gizmos.DrawLine(position + new Vector3(-half.x, -half.y, half.z), position + new Vector3(half.x, -half.y, half.z));
            Gizmos.DrawLine(position + new Vector3(-half.x, -half.y, half.z), position + new Vector3(-half.x, half.y, half.z));
            Gizmos.DrawLine(position + new Vector3(half.x, half.y, half.z), position + new Vector3(half.x, -half.y, half.z));
            Gizmos.DrawLine(position + new Vector3(half.x, half.y, half.z), position + new Vector3(-half.x, half.y, half.z));
            // draw back
            Gizmos.DrawLine(position + new Vector3(-half.x, -half.y, -half.z), position + new Vector3(half.x, -half.y, -half.z));
            Gizmos.DrawLine(position + new Vector3(-half.x, -half.y, -half.z), position + new Vector3(-half.x, half.y, -half.z));
            Gizmos.DrawLine(position + new Vector3(half.x, half.y, -half.z), position + new Vector3(half.x, -half.y, -half.z));
            Gizmos.DrawLine(position + new Vector3(half.x, half.y, -half.z), position + new Vector3(-half.x, half.y, -half.z));
            // draw corners
            Gizmos.DrawLine(position + new Vector3(-half.x, -half.y, -half.z), position + new Vector3(-half.x, -half.y, half.z));
            Gizmos.DrawLine(position + new Vector3(half.x, -half.y, -half.z), position + new Vector3(half.x, -half.y, half.z));
            Gizmos.DrawLine(position + new Vector3(-half.x, half.y, -half.z), position + new Vector3(-half.x, half.y, half.z));
            Gizmos.DrawLine(position + new Vector3(half.x, half.y, -half.z), position + new Vector3(half.x, half.y, half.z));
        }

        public static void DrawWireCube(Transform t, Vector3 center, Vector3 size) {
            Matrix4x4 mtx = t.localToWorldMatrix;

            var half = size / 2;
            // draw front
            Gizmos.DrawLine(mtx.MultiplyPoint3x4(center + new Vector3(-half.x, -half.y, half.z)), mtx.MultiplyPoint3x4(center + new Vector3(half.x, -half.y, half.z)));
            Gizmos.DrawLine(mtx.MultiplyPoint3x4(center + new Vector3(-half.x, -half.y, half.z)), mtx.MultiplyPoint3x4(center + new Vector3(-half.x, half.y, half.z)));
            Gizmos.DrawLine(mtx.MultiplyPoint3x4(center + new Vector3(half.x, half.y, half.z)), mtx.MultiplyPoint3x4(center + new Vector3(half.x, -half.y, half.z)));
            Gizmos.DrawLine(mtx.MultiplyPoint3x4(center + new Vector3(half.x, half.y, half.z)), mtx.MultiplyPoint3x4(center + new Vector3(-half.x, half.y, half.z)));
            // draw back
            Gizmos.DrawLine(mtx.MultiplyPoint3x4(center + new Vector3(-half.x, -half.y, -half.z)), mtx.MultiplyPoint3x4(center + new Vector3(half.x, -half.y, -half.z)));
            Gizmos.DrawLine(mtx.MultiplyPoint3x4(center + new Vector3(-half.x, -half.y, -half.z)), mtx.MultiplyPoint3x4(center + new Vector3(-half.x, half.y, -half.z)));
            Gizmos.DrawLine(mtx.MultiplyPoint3x4(center + new Vector3(half.x, half.y, -half.z)), mtx.MultiplyPoint3x4(center + new Vector3(half.x, -half.y, -half.z)));
            Gizmos.DrawLine(mtx.MultiplyPoint3x4(center + new Vector3(half.x, half.y, -half.z)), mtx.MultiplyPoint3x4(center + new Vector3(-half.x, half.y, -half.z)));
            // draw corners
            Gizmos.DrawLine(mtx.MultiplyPoint3x4(center + new Vector3(-half.x, -half.y, -half.z)), mtx.MultiplyPoint3x4(center + new Vector3(-half.x, -half.y, half.z)));
            Gizmos.DrawLine(mtx.MultiplyPoint3x4(center + new Vector3(half.x, -half.y, -half.z)), mtx.MultiplyPoint3x4(center + new Vector3(half.x, -half.y, half.z)));
            Gizmos.DrawLine(mtx.MultiplyPoint3x4(center + new Vector3(-half.x, half.y, -half.z)), mtx.MultiplyPoint3x4(center + new Vector3(-half.x, half.y, half.z)));
            Gizmos.DrawLine(mtx.MultiplyPoint3x4(center + new Vector3(half.x, half.y, -half.z)), mtx.MultiplyPoint3x4(center + new Vector3(half.x, half.y, half.z)));
        }
    }
}