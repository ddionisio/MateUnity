using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace M8 {
    public abstract class ResourceLoader {
        public enum ErrorCode {
            None,
            FileNotExist,
            InvalidPackage
        }

        public enum Status {
            NotExist = -1,
            Unloaded,
            Loading,
            Loaded,
            Error
        }

        public abstract class Request {
            public System.Type type { get; private set; }

            /// <summary>
            /// Relative to group
            /// </summary>
            public string path { get; private set; }
            public ErrorCode errorCode { get; protected set; }
            public abstract bool isDone { get; } //keep getting called until it returns true
            public abstract object data { get; }
            
            public Request(string aPath, System.Type aType) { path = aPath; type = aType; errorCode = ErrorCode.None; }

            public string error {
                get {
                    switch(errorCode) {
                        case ErrorCode.FileNotExist:
                            return "File does not exist: "+path+" type: "+type;

                        case ErrorCode.InvalidPackage:
                            return "Invalid package for: "+path+" type: "+type;
                    }

                    return "";
                }
            }
        }

        protected interface IRequestProcess {
            bool isDone { get; }
            object data { get; }
        }

        protected class RequestProcessResource : IRequestProcess {
            private ResourceRequest mReq;

            public bool isDone { get { return mReq.isDone; } }
            public object data { get { return mReq.asset; } }

            public RequestProcessResource(ResourceRequest req) {
                mReq = req;
            }
        }

#if !M8_ASSET_BUNDLE_DISABLED
        protected class RequestProcessBundle : IRequestProcess {
            private AssetBundleRequest mReq;

            public bool isDone { get { return mReq.isDone; } }
            public object data { get { return mReq.asset; } }

            public RequestProcessBundle(AssetBundleRequest req) {
                mReq = req;
            }
        }
#endif

        protected class RequestInternal : Request {
            public override bool isDone { get { return errorCode == ErrorCode.None ? processor != null ? processor.isDone : false : true; } }

            public override object data { get { return processor != null ? processor.data : null; } }

            public IRequestProcess processor { //fill this up
                get { return mProcessor; }
                set {
                    mProcessor = value;

                    errorCode = ErrorCode.None; //reset error
                }
            }

            public void Error(ErrorCode errCode) {
                errorCode = errCode;
            }

            public RequestInternal(string path, System.Type type) : base(path, type) { }

            private IRequestProcess mProcessor;
        }

        public Status status { get; protected set; }
        public string error { get; protected set; }

        public string rootPath { get; private set; }

        //used for resources and bundles
        private StringBuilder mSBuff;
        private int mSBuffRootCount;

        public static Request CreateRequest(string path, System.Type type) {
            return new RequestInternal(path, type);
        }

        /// <summary>
        /// Called by ResourceManager when something else went wrong while processing request
        /// </summary>
        public static void RequestError(Request req, ErrorCode code) {
            ((RequestInternal)req).Error(code);
        }
        
        public abstract IEnumerator Load();

        public abstract void Unload();
        
        public abstract void UnloadResource(object obj);

        /// <summary>
        /// Process given request, creating task for loading data.
        /// Returns true if successful
        /// </summary>
        public abstract bool ProcessRequest(Request req);
                
        public ResourceLoader(string aRootPath) {
            rootPath = aRootPath;

            //for unity file pathing in Resources and AssetBundle
            mSBuff = new StringBuilder(512);
            for(int i = 0; i < rootPath.Length; i++) {
                char c = rootPath[i];
                mSBuff.Append(c == '\\' ? '/' : c);
            }

            if(mSBuff[mSBuff.Length-1] != '/')
                mSBuff.Append('/');

            mSBuffRootCount = mSBuff.Length;
            //

            status = Status.Unloaded;
        }

        /// <summary>
        /// Format the path for use with Resources and AssetBundle, set absolute=true to prepend root path
        /// </summary>
        protected string GetUnityPath(string path, bool absolute) {
            mSBuff.Length = mSBuffRootCount;

            int dotInd = path.LastIndexOf('.');
            int count = dotInd == -1 ? path.Length : dotInd;
            for(int i = 0; i < count; i++) {
                char c = path[i];
                mSBuff.Append(c == '\\' ? '/' : c);
            }

            return absolute ? mSBuff.ToString() : mSBuff.ToString(mSBuffRootCount, mSBuff.Length - mSBuffRootCount);
        }
    }
}