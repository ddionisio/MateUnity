using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    public abstract class GOActivatorActivateCheckBase<T> : MonoBehaviour where T:Component {
        public LayerMask layerMask;
        [TagSelector]
        public string[] tagFilters; //if not empty, only select activators that match one of these tags
        public float delay = 0.3f;
        public int capacity = 8;

        private struct ActiveData {
            public T collider;
            public GOActivator activator;
        }

        private T[] mColls;
        private CacheList<ActiveData> mActives;
        private float mCurTime;

        protected abstract int PopulateCollisions(T[] cache);

        void OnEnable() {
            mCurTime = 0f;
        }

        void OnDisable() {
            //deactivate current actives
            for(int i = 0; i < mActives.Count; i++) {
                var active = mActives[i];
                if(active.activator)
                    active.activator.Deactivate();
            }

            mActives.Clear();
        }

        void Awake() {
            mColls = new T[capacity];
            mActives = new CacheList<ActiveData>(capacity);
        }

        void Update() {
            if(mCurTime >= delay) {
                mCurTime = 0f;

                int count = PopulateCollisions(mColls);

                //remove some actives
                for(int i = mActives.Count - 1; i >= 0; i--) {
                    var active = mActives[i];

                    //no longer valid?
                    if(active.collider == null || !ArrayUtil.Contains(mColls, 0, count, active.collider)) {
                        if(active.activator)
                            active.activator.Deactivate();

                        mActives.RemoveLast();
                    }
                }

                //add new actives
                int activeCount = mActives.Count;
                for(int i = 0; i < count; i++) {
                    var coll = mColls[i];

                    //ignore if tag not match
                    if(tagFilters.Length > 0) {
                        bool isTagMatch = false;
                        for(int t = 0; t < tagFilters.Length; t++) {
                            if(coll.CompareTag(tagFilters[t])) {
                                isTagMatch = true;
                                break;
                            }
                        }

                        if(!isTagMatch)
                            continue;
                    }

                    //check if it already exists
                    int activeInd = -1;
                    for(int j = 0; j < activeCount; j++) {
                        var active = mActives[j];
                        if(active.collider == coll) {
                            activeInd = j;
                            break;
                        }
                    }

                    //add
                    if(activeInd == -1) {
                        var activator = coll.GetComponent<GOActivator>();
                        if(activator) {
                            activator.Activate();

                            mActives.Add(new ActiveData { collider = coll, activator = activator });
                        }
                    }
                }
            }
            else
                mCurTime += Time.deltaTime;
        }
    }
}