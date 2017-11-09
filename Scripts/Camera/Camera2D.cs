using UnityEngine;
using System.Collections;

namespace M8 {
    [AddComponentMenu("M8/Camera/Camera2D")]
    [ExecuteInEditMode]
    public class Camera2D : MonoBehaviour {
        public enum ScaleMode {
            None,
            Width,
            Height,
            Visible,
            Stretch,
            PowerOfTwo,
            PixelPerfect,
            Fill
        }

        public enum Origin {
            BottomLeft,
            Center
        }

        public int fixedResolutionWidth = 640;
        public int fixedResolutionHeight = 360;

        public bool usePixelPerMeter = true;
        public float pixelPerMeter = 32.0f;
        public float orthographicSize = 10.0f;
        public float fov = 60.0f;
        public Rect viewRect = new Rect(0, 0, 1, 1);
        public TransparencySortMode transparencySortMode = TransparencySortMode.Default; //set to ortho for perspective camera for 2D

        public ScaleMode scaleMode = ScaleMode.Height;

        public float pixelScale = 1.0f; //used if no scale mode specified

        public bool usePixelOffset = false;
        public Vector2 pixelOffset = new Vector2(0.0f, 0.0f); //in pixels

        public Origin origin = Origin.Center;

        public bool useClipping = false;
        public Rect clipRegion = new Rect(0.0f, 0.0f, 100.0f, 100.0f);

        [SerializeField]
        float _zoom = 1.0f;

        private Rect mScreenExtent;
        private Rect mFixedScreenExtent;

        private Vector2 mResolution;

        private Camera mCamera;

        private static Camera2D mMain;

        public static Camera2D main {
            get {
                if(mMain == null) {
                    Camera cam = Camera.main;
                    mMain = cam != null ? cam.GetComponent<Camera2D>() : null;
                }
                return mMain;
            }
        }

        public Camera unityCamera {
            get {
                if(!mCamera)
                    mCamera = GetComponent<Camera>();

                return mCamera;
            }
        }

        public Rect screenExtent { get { return mScreenExtent; } }
        public Rect fixedScreenExtent { get { return mFixedScreenExtent; } }

        public Vector2 fixedResolution { get { return new Vector2(fixedResolutionWidth, fixedResolutionHeight); } }
        public Vector2 resolution { get { return mResolution; } }

        public float zoom { get { return _zoom; } set { _zoom = Mathf.Max(0.001f, value); } }

        /// <summary>
        /// Gets the size of the pixel in units based on this camera's resolution size, or pixel/meter
        /// </summary>
        /// <returns>The pixel size.</returns>
        /// <param name="distance">Distance.</param>
        public float getPixelSize(float distance) {
            if(mCamera.orthographic) {
                if(usePixelPerMeter)
                    return 1.0f / pixelPerMeter;
                else
                    return 2.0f*orthographicSize / fixedResolutionHeight;
            }
            else {
                return Mathf.Tan(fov*Mathf.Deg2Rad*0.5f)*distance*2.0f / fixedResolutionHeight;
            }
        }

        void OnEnable() {
            DoUpdate();
        }

        void OnPreCull() {
            DoUpdate();
        }

        void OnDestroy() {
            if(mMain == this)
                mMain = null;
        }

        void Awake() {
            if(unityCamera && !unityCamera.orthographic)
                mCamera.transparencySortMode = transparencySortMode;
        }

#if UNITY_EDITOR
        void LateUpdate() {
            if(!Application.isPlaying)
                DoUpdate();
        }

        static Vector3[] viewportBoxPoints = new Vector3[] {
		new Vector3(-1, -1, -1), new Vector3( 1, -1, -1), new Vector3( 1,  1, -1), new Vector3(-1,  1, -1), new Vector3(-1, -1,  1), new Vector3( 1, -1,  1), new Vector3( 1,  1,  1), new Vector3(-1,  1,  1),
	};
        static int[] viewportBoxIndices = new int[] {
		0, 1, 1, 2, 2, 3, 3, 0, 4, 5, 5, 6, 6, 7, 7, 4, 0, 4, 1, 5, 3, 7, 2, 6,
	};
        static Vector3[] transformedViewportBoxPoints = new Vector3[8];

        void DrawCameraBounds(Matrix4x4 worldToCamera, Matrix4x4 projectionMatrix) {
            Matrix4x4 m = worldToCamera.inverse * projectionMatrix.inverse;
            for(int i = 0; i < viewportBoxPoints.Length; ++i) {
                transformedViewportBoxPoints[i] =  m.MultiplyPoint(viewportBoxPoints[i]);
            }
            for(int i = 0; i < viewportBoxIndices.Length; i += 2) {
                Gizmos.DrawLine(transformedViewportBoxPoints[viewportBoxIndices[i]], transformedViewportBoxPoints[viewportBoxIndices[i + 1]]);
            }
        }

        Matrix4x4 getPerspectiveMatrix() {
            float aspect = (float)fixedResolutionWidth / (float)fixedResolutionHeight;
            return Matrix4x4.Perspective(fov, aspect, mCamera.nearClipPlane, mCamera.farClipPlane);
        }

        Matrix4x4 getFixedProjectionMatrix() {
            if(!mCamera.orthographic) {
                return getPerspectiveMatrix();
            }
            Rect rect1 = new Rect(0, 0, 1, 1);
            Rect rect2 = new Rect(0, 0, 1, 1);
            return getProjectionMatrix(fixedResolutionWidth, fixedResolutionHeight, false, out rect1, out rect2);
        }

        Matrix4x4 getFinalProjectionMatrix() {
            if(!mCamera.orthographic) {
                return getPerspectiveMatrix();
            }
            Vector2 resolution = getScreenPixelDimensions();
            Rect rect1 = new Rect(0, 0, 1, 1);
            Rect rect2 = new Rect(0, 0, 1, 1);
            return getProjectionMatrix(resolution.x, resolution.y, false, out rect1, out rect2);
        }

        void OnDrawGizmos() {
            if(!useClipping) {
                Gizmos.color = new Color32(255, 255, 255, 255);
                DrawCameraBounds(mCamera.worldToCameraMatrix, getFinalProjectionMatrix());
            }

            Gizmos.color = new Color32(55, 203, 105, 102);
            DrawCameraBounds(mCamera.worldToCameraMatrix, getFixedProjectionMatrix());
        }
#endif

        //private Rect unitRect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);

        Matrix4x4 orthoOffCenter(Vector2 scale, float left, float right, float bottom, float top, float near, float far) {
            // Additional half texel offset
            // Takes care of texture unit offset, if necessary.

            float x =  (2.0f) / (right - left) * scale.x;
            float y = (2.0f) / (top - bottom) * scale.y;
            float z = -2.0f / (far - near);

            float a = -(right + left) / (right - left);
            float b = -(bottom + top) / (top - bottom);
            float c = -(far + near) / (far - near);

            Matrix4x4 m = new Matrix4x4();
            m[0, 0] = x; m[0, 1] = 0; m[0, 2] = 0; m[0, 3] = a;
            m[1, 0] = 0; m[1, 1] = y; m[1, 2] = 0; m[1, 3] = b;
            m[2, 0] = 0; m[2, 1] = 0; m[2, 2] = z; m[2, 3] = c;
            m[3, 0] = 0; m[3, 1] = 0; m[3, 2] = 0; m[3, 3] = 1;

            return m;
        }

        Vector2 getScale(float width, float height) {
            Vector2 scale = Vector2.one;
            float s = 1.0f;

            switch(scaleMode) {
                case ScaleMode.None:
                    scale.Set(pixelScale, pixelScale);
                    break;
                case ScaleMode.Width:
                    s = width / fixedResolutionWidth;
                    scale.Set(s, s);
                    break;
                case ScaleMode.Height:
                    s = height / fixedResolutionHeight;
                    scale.Set(s, s);
                    break;
                case ScaleMode.Visible:
                case ScaleMode.PowerOfTwo:
                    float originAspect = (float)fixedResolutionWidth/fixedResolutionHeight;
                    float aspect = width/height;

                    s = originAspect < aspect ? width/fixedResolutionWidth : height/fixedResolutionHeight;

                    if(scaleMode == ScaleMode.PowerOfTwo) {
                        s = s > 1.0f ? Mathf.Floor(s) : Mathf.Pow(2, Mathf.Floor(Mathf.Log(s, 2)));
                    }

                    scale.Set(s, s);
                    break;
                case ScaleMode.Stretch:
                    scale.Set(width/fixedResolutionWidth, height/fixedResolutionHeight);
                    break;
                case ScaleMode.PixelPerfect:
                    break;
                case ScaleMode.Fill:
                    s = Mathf.Max(width/fixedResolutionWidth, height/fixedResolutionHeight);
                    scale.Set(s, s);
                    break;
            }

            return scale;
        }

        Vector2 getOffset(Vector2 scale, float width, float height) {
            Vector2 offset = Vector2.zero;

            if(usePixelOffset) {
                offset = -pixelOffset;
            }
            else {
                if(origin == Origin.BottomLeft) {
                    offset = new Vector2(
                        Mathf.Round((fixedResolutionWidth*scale.x - width) / 2.0f),
                        Mathf.Round((fixedResolutionHeight*scale.y - height) / 2.0f));
                }
            }
            return offset;
        }

        Matrix4x4 getProjectionMatrix(float pixelWidth, float pixelHeight, bool halfTexelOffset, out Rect screenExtents, out Rect unscaledScreenExtents) {
            Vector2 scale = getScale(pixelWidth, pixelHeight);
            Vector2 offset = getOffset(scale, pixelWidth, pixelHeight);

            float left = offset.x, bottom = offset.y;
            float right = pixelWidth + offset.x, top = pixelHeight + offset.y;
            Vector2 fixedResolutionOffset = Vector2.zero;

            bool usingLegacyViewportClipping = false;

            if(useClipping) {
                float vw = (right - left) / scale.x;
                float vh = (top - bottom) / scale.y;
                Vector4 sr = new Vector4((int)clipRegion.x, (int)clipRegion.y, (int)clipRegion.width, (int)clipRegion.height);

                usingLegacyViewportClipping = true;

                float viewportLeft = -offset.x / pixelWidth + sr.x / vw;
                float viewportBottom = -offset.y / pixelHeight + sr.y / vh;
                float viewportWidth = sr.z / vw;
                float viewportHeight = sr.w / vh;
                if(origin == Origin.Center) {
                    viewportLeft += (pixelWidth - fixedResolutionWidth*scale.x) / pixelWidth / 2.0f;
                    viewportBottom += (pixelHeight - fixedResolutionHeight*scale.y) / pixelHeight / 2.0f;
                }

                Rect r = new Rect(viewportLeft, viewportBottom, viewportWidth, viewportHeight);
                if(mCamera.rect.x != viewportLeft ||
			    mCamera.rect.y != viewportBottom ||
			    mCamera.rect.width != viewportWidth ||
			    mCamera.rect.height != viewportHeight) {
                    mCamera.rect = r;
                }

                float maxWidth = Mathf.Min(1.0f - r.x, r.width);
                float maxHeight = Mathf.Min(1.0f - r.y, r.height);

                float rectOffsetX = sr.x * scale.x - offset.x;
                float rectOffsetY = sr.y * scale.y - offset.y;

                if(origin == Origin.Center) {
                    rectOffsetX -= fixedResolutionWidth*0.5f*scale.x;
                    rectOffsetY -= fixedResolutionHeight*0.5f*scale.y;
                }

                if(r.x < 0.0f) {
                    rectOffsetX += -r.x*pixelWidth;
                    maxWidth = (r.x + r.width);
                }
                if(r.y < 0.0f) {
                    rectOffsetY += -r.y*pixelHeight;
                    maxHeight = (r.y + r.height);
                }

                left += rectOffsetX;
                bottom += rectOffsetY;
                right = pixelWidth*maxWidth + offset.x + rectOffsetX;
                top = pixelHeight*maxHeight + offset.y +  rectOffsetY;
            }
            else {
                if(mCamera.rect != viewRect)
                    mCamera.rect = viewRect;

                // By default the camera is orthographic, bottom left, 1 pixel per meter
                if(origin == Origin.Center) {
                    float w = (right - left)*0.5f;
                    left -= w; right -= w;
                    float h = (top - bottom)*0.5f;
                    top -= h; bottom -= h;
                    fixedResolutionOffset.Set(-fixedResolutionWidth*0.5f, -fixedResolutionHeight*0.5f);
                }
            }

            float zoomScale = 1.0f / zoom;

            // Only need the half texel offset on PC/D3D
            bool needHalfTexelOffset = (Application.platform == RuntimePlatform.WindowsPlayer ||
		                            Application.platform == RuntimePlatform.WindowsEditor);
            float halfTexel = (halfTexelOffset && needHalfTexelOffset) ? 0.5f : 0.0f;

            float orthoSize = orthographicSize;
            if(usePixelPerMeter) {
                orthoSize = 1.0f/pixelPerMeter;
            }
            else {
                orthoSize = 2.0f*orthographicSize / fixedResolutionHeight;
            }

            // Fixup for clipping
            if(!usingLegacyViewportClipping) {
                float clipWidth = Mathf.Min(mCamera.rect.width, 1.0f - mCamera.rect.x);
                float clipHeight = Mathf.Min(mCamera.rect.height, 1.0f - mCamera.rect.y);
                if(clipWidth > 0 && clipHeight > 0) {
                    scale.x /= clipWidth;
                    scale.y /= clipHeight;
                }
            }

            float s = orthoSize * zoomScale;
            screenExtents = new Rect(left * s / scale.x, bottom * s / scale.y,
                                     (right - left) * s / scale.x, (top - bottom) * s / scale.y);

            unscaledScreenExtents = new Rect(fixedResolutionOffset.x * s, fixedResolutionOffset.y * s,
                                             fixedResolutionWidth * s, fixedResolutionHeight * s);

            // Near and far clip planes are tweakable per camera, so we pull from current camera instance regardless of inherited values
            return orthoOffCenter(scale, orthoSize * (left + halfTexel) * zoomScale, orthoSize * (right + halfTexel) * zoomScale,
                                  orthoSize * (bottom - halfTexel) * zoomScale, orthoSize * (top - halfTexel) * zoomScale,
                                  mCamera.nearClipPlane, mCamera.farClipPlane);
        }

        Vector2 getScreenPixelDimensions() {
            Camera cam = useClipping ? Camera.main : mCamera;

            return new Vector2(cam.pixelWidth, cam.pixelHeight);
        }

        void DoUpdate() {
            if(mCamera.rect != viewRect)
                mCamera.rect = viewRect;

            mResolution = getScreenPixelDimensions();

            if(mCamera.orthographic) {
                Matrix4x4 m = getProjectionMatrix(mResolution.x, mResolution.y, true, out mScreenExtent, out mFixedScreenExtent);

#if false //Certain devices may require reorientation
                if(Application.platform == RuntimePlatform.WP8Player &&
			    (Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.LandscapeRight)) {
                    float angle = (Screen.orientation == ScreenOrientation.LandscapeRight) ? 90.0f : -90.0f;
                    Matrix4x4 m2 = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, angle), Vector3.one);
                    m = m2 * m;
                }
#endif
                if(mCamera.projectionMatrix != m)
                    mCamera.projectionMatrix = m;
            }
            else {
                float _fov = Mathf.Min(179.9f, fov / Mathf.Max(0.001f, zoom));
                if(mCamera.fieldOfView != _fov) mCamera.fieldOfView = _fov;
                mScreenExtent.Set(-mCamera.aspect, -1, mCamera.aspect*2.0f, 2.0f);
                mFixedScreenExtent = mScreenExtent;
                mCamera.ResetProjectionMatrix();
            }
        }
    }
}