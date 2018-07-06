using UnityEngine;
using System.Collections.Generic;

namespace M8 {
    [AddComponentMenu("M8/TransitionFX/Render")]
    [RequireComponent(typeof(Camera))]
    public class TransitionFXRender : MonoBehaviour {
        private List<TransitionFX> mRenderTransList;

        public void AddRender(TransitionFX trans) {
            if(!mRenderTransList.Contains(trans))
                mRenderTransList.Add(trans);

            enabled = true;
        }

        public void RemoveRender(TransitionFX trans) {
            mRenderTransList.Remove(trans);

            if(mRenderTransList.Count == 0)
                enabled = false;
        }
        
        void Awake() {
            mRenderTransList = new List<TransitionFX>();
        }
        
        void OnRenderImage(RenderTexture source, RenderTexture destination) {
            for(int i = 0; i < mRenderTransList.Count; i++) {
                if(mRenderTransList[i])
                    mRenderTransList[i].OnRenderImage(source, destination);
            }
        }
    }
}