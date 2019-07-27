using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    [AddComponentMenu("M8/Transform/ChildrenShuffle")]
    public class TransChildrenShuffle : MonoBehaviour {
        public enum Mode {
            None,
            Awake,
            Start,
            Enable
        }

        public Mode mode = Mode.Enable;
        public Transform target; //if null, use self

        public void Shuffle() {
            var t = target ? target : transform;

            var count = t.childCount;

            for(int i = 0; i < count; i++) {
                int r = Random.Range(i, count);

                var child1 = t.GetChild(i);
                var child2 = t.GetChild(r);

                child1.SetSiblingIndex(r);
                child2.SetSiblingIndex(i);
            }
        }

        void OnEnable() {
            if(mode == Mode.Enable)
                Shuffle();
        }

        void Awake() {
            if(mode == Mode.Awake)
                Shuffle();
        }

        void Start() {
            if(mode == Mode.Start)
                Shuffle();
        }
    }
}