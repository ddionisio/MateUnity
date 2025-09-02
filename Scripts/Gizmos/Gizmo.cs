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

		public static void Arrow2D(Vector2 pos, Vector2 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f) {
            ArrowLine2D(pos, pos + direction, arrowHeadLength, arrowHeadAngle);
		}

		public static void DrawWireRect(Rect rect, float rotation) {
            var extent = rect.size * 0.5f;
            DrawWireRect(rect.position + extent, rotation, extent);
		}

		public static void DrawWireRect(Vector3 position, float rotation, Vector2 extent) {
            var rot = Quaternion.Euler(0f, 0f, rotation);
            Gizmos.DrawLine(position + rot * new Vector3(-extent.x, -extent.y, 0f), position + rot * new Vector3(extent.x, -extent.y, 0f));
            Gizmos.DrawLine(position + rot * new Vector3(-extent.x, -extent.y, 0f), position + rot * new Vector3(-extent.x, extent.y, 0f));
            Gizmos.DrawLine(position + rot * new Vector3(extent.x, extent.y, 0f), position + rot * new Vector3(extent.x, -extent.y, 0f));
            Gizmos.DrawLine(position + rot * new Vector3(extent.x, extent.y, 0f), position + rot * new Vector3(-extent.x, extent.y, 0f));
        }

        public static void DrawWireRect(Vector3[] worldCorners) {
            Gizmos.DrawLine(worldCorners[0], worldCorners[1]);
            Gizmos.DrawLine(worldCorners[1], worldCorners[2]);
            Gizmos.DrawLine(worldCorners[2], worldCorners[3]);
            Gizmos.DrawLine(worldCorners[3], worldCorners[0]);
        }

        public static void DrawWireCube(Vector3 position, Quaternion rotation, Vector3 extent) {
            // draw front
            Gizmos.DrawLine(position + rotation * new Vector3(-extent.x, -extent.y, extent.z), position + rotation * new Vector3(extent.x, -extent.y, extent.z));
            Gizmos.DrawLine(position + rotation * new Vector3(-extent.x, -extent.y, extent.z), position + rotation * new Vector3(-extent.x, extent.y, extent.z));
            Gizmos.DrawLine(position + rotation * new Vector3(extent.x, extent.y, extent.z), position + rotation * new Vector3(extent.x, -extent.y, extent.z));
            Gizmos.DrawLine(position + rotation * new Vector3(extent.x, extent.y, extent.z), position + rotation * new Vector3(-extent.x, extent.y, extent.z));
            // draw back
            Gizmos.DrawLine(position + rotation * new Vector3(-extent.x, -extent.y, -extent.z), position + rotation * new Vector3(extent.x, -extent.y, -extent.z));
            Gizmos.DrawLine(position + rotation * new Vector3(-extent.x, -extent.y, -extent.z), position + rotation * new Vector3(-extent.x, extent.y, -extent.z));
            Gizmos.DrawLine(position + rotation * new Vector3(extent.x, extent.y, -extent.z), position + rotation * new Vector3(extent.x, -extent.y, -extent.z));
            Gizmos.DrawLine(position + rotation * new Vector3(extent.x, extent.y, -extent.z), position + rotation * new Vector3(-extent.x, extent.y, -extent.z));
            // draw corners
            Gizmos.DrawLine(position + rotation * new Vector3(-extent.x, -extent.y, -extent.z), position + rotation * new Vector3(-extent.x, -extent.y, extent.z));
            Gizmos.DrawLine(position + rotation * new Vector3(extent.x, -extent.y, -extent.z), position + rotation * new Vector3(extent.x, -extent.y, extent.z));
            Gizmos.DrawLine(position + rotation * new Vector3(-extent.x, extent.y, -extent.z), position + rotation * new Vector3(-extent.x, extent.y, extent.z));
            Gizmos.DrawLine(position + rotation * new Vector3(extent.x, extent.y, -extent.z), position + rotation * new Vector3(extent.x, extent.y, extent.z));
        }

        public static void DrawWireCube(Transform t, Vector3 center, Vector3 extent) {
            Matrix4x4 mtx = t.localToWorldMatrix;

            // draw front
            Gizmos.DrawLine(mtx.MultiplyPoint3x4(center + new Vector3(-extent.x, -extent.y, extent.z)), mtx.MultiplyPoint3x4(center + new Vector3(extent.x, -extent.y, extent.z)));
            Gizmos.DrawLine(mtx.MultiplyPoint3x4(center + new Vector3(-extent.x, -extent.y, extent.z)), mtx.MultiplyPoint3x4(center + new Vector3(-extent.x, extent.y, extent.z)));
            Gizmos.DrawLine(mtx.MultiplyPoint3x4(center + new Vector3(extent.x, extent.y, extent.z)), mtx.MultiplyPoint3x4(center + new Vector3(extent.x, -extent.y, extent.z)));
            Gizmos.DrawLine(mtx.MultiplyPoint3x4(center + new Vector3(extent.x, extent.y, extent.z)), mtx.MultiplyPoint3x4(center + new Vector3(-extent.x, extent.y, extent.z)));
            // draw back
            Gizmos.DrawLine(mtx.MultiplyPoint3x4(center + new Vector3(-extent.x, -extent.y, -extent.z)), mtx.MultiplyPoint3x4(center + new Vector3(extent.x, -extent.y, -extent.z)));
            Gizmos.DrawLine(mtx.MultiplyPoint3x4(center + new Vector3(-extent.x, -extent.y, -extent.z)), mtx.MultiplyPoint3x4(center + new Vector3(-extent.x, extent.y, -extent.z)));
            Gizmos.DrawLine(mtx.MultiplyPoint3x4(center + new Vector3(extent.x, extent.y, -extent.z)), mtx.MultiplyPoint3x4(center + new Vector3(extent.x, -extent.y, -extent.z)));
            Gizmos.DrawLine(mtx.MultiplyPoint3x4(center + new Vector3(extent.x, extent.y, -extent.z)), mtx.MultiplyPoint3x4(center + new Vector3(-extent.x, extent.y, -extent.z)));
            // draw corners
            Gizmos.DrawLine(mtx.MultiplyPoint3x4(center + new Vector3(-extent.x, -extent.y, -extent.z)), mtx.MultiplyPoint3x4(center + new Vector3(-extent.x, -extent.y, extent.z)));
            Gizmos.DrawLine(mtx.MultiplyPoint3x4(center + new Vector3(extent.x, -extent.y, -extent.z)), mtx.MultiplyPoint3x4(center + new Vector3(extent.x, -extent.y, extent.z)));
            Gizmos.DrawLine(mtx.MultiplyPoint3x4(center + new Vector3(-extent.x, extent.y, -extent.z)), mtx.MultiplyPoint3x4(center + new Vector3(-extent.x, extent.y, extent.z)));
            Gizmos.DrawLine(mtx.MultiplyPoint3x4(center + new Vector3(extent.x, extent.y, -extent.z)), mtx.MultiplyPoint3x4(center + new Vector3(extent.x, extent.y, extent.z)));
        }

        public static void DrawStepLineX(Transform t, Vector3 localPoint, float length) {
			Matrix4x4 mtx = t.localToWorldMatrix;

            var ext = length * 0.5f;

            Gizmos.DrawLine(mtx.MultiplyPoint3x4(localPoint + new Vector3(-ext, 0f, 0f)), mtx.MultiplyPoint3x4(localPoint + new Vector3(ext, 0f, 0f)));
		}

		public static void DrawStepLineY(Transform t, Vector3 localPoint, float length) {
			Matrix4x4 mtx = t.localToWorldMatrix;

			var ext = length * 0.5f;

			Gizmos.DrawLine(mtx.MultiplyPoint3x4(localPoint + new Vector3(0f, -ext, 0f)), mtx.MultiplyPoint3x4(localPoint + new Vector3(0f, ext, 0f)));
		}

		public static void DrawStepLineZ(Transform t, Vector3 localPoint, float length) {
			Matrix4x4 mtx = t.localToWorldMatrix;

			var ext = length * 0.5f;

			Gizmos.DrawLine(mtx.MultiplyPoint3x4(localPoint + new Vector3(0f, 0f, -ext)), mtx.MultiplyPoint3x4(localPoint + new Vector3(0f, 0f, ext)));
		}
	}
}