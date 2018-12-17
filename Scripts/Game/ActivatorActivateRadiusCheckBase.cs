using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    public abstract class ActivatorActivateRadiusCheckBase<T> : MonoBehaviour where T:Component {
        public LayerMask layerMask;
        public float radius;
        public float delay = 0.3f;
        public int capacity = 8;

        private struct ActiveData {
            public T collider;
            public Activator activator;
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

                    int activeInd = -1;
                    for(int j = 0; j < activeCount; j++) {
                        var active = mActives[j];
                        if(active.collider == coll) {
                            activeInd = j;
                            break;
                        }
                    }

                    if(activeInd == -1) {
                        var activator = coll.GetComponent<Activator>();
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

        void OnDrawGizmos() {
            if(radius > 0f) {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(transform.position, radius);
            }
        }
    }
}