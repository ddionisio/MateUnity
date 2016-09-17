using UnityEngine;
using System.Collections.Generic;

namespace M8 {
    [AddComponentMenu("M8/Screen Transition/Render")]
    [RequireComponent(typeof(Camera))]
    public class ScreenTransRender : MonoBehaviour {
        private List<ScreenTrans> mRenderTransList;

        public void AddRender(ScreenTrans trans) {
            if(!mRenderTransList.Contains(trans))
                mRenderTransList.Add(trans);

            enabled = true;
        }

        public void RemoveRender(ScreenTrans trans) {
            mRenderTransList.Remove(trans);

            if(mRenderTransList.Count == 0)
                enabled = false;
        }
        
        void Awake() {
            mRenderTransList = new List<ScreenTrans>();
        }
        
        void OnRenderImage(RenderTexture source, RenderTexture destination) {
            for(int i = 0; i < mRenderTransList.Count; i++) {
                if(mRenderTransList[i])
                    mRenderTransList[i].OnRenderImage(source, destination);
            }
        }
    }
}