using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace M8 {
    public abstract class ResourceLoader {
        public enum Status {
            NotExist = -1,
            Unloaded,
            Loading,
            Loaded,
            Error
        }

        public abstract class Request : IEnumerator {
            public string path { get; private set; }
            public abstract bool isDone { get; } //keep getting called until it returns true
            public abstract object data { get; }
            public abstract string error { get; }

            public Request(string aPath) { path = aPath; }

            object IEnumerator.Current { get { return data; } }

            bool IEnumerator.MoveNext() { return isDone; }

            void IEnumerator.Reset() { }

        }

        //used for resources and bundles
        private StringBuilder mSBuff;
        private int mSBuffRootCount;
                
        public Status status { get; protected set; }
        public string error { get; protected set; }

        public string rootPath { get; private set; }

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

        public abstract IEnumerator Load();

        public abstract void Unload();

        public abstract bool ResourceExists(string path);

        public abstract Request RequestResource(string path, System.Type type);

        public abstract void UnloadResource(object obj);

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