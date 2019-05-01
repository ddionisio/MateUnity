using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    /// <summary>
    /// Copy palette range from source to destination.
    /// </summary>
    [AddComponentMenu("M8/Color/Palette Copy")]
    public class ColorPaletteCopy : MonoBehaviour {
        public ColorPalette source;
        public int sourceIndex;

        public ColorPalette destination;
        public int destinationIndex;

        [Tooltip("Set to 0 to copy up to the end.")]
        public int length;

        public bool applyOnEnable = true;
        public bool revertOnDisable = true;

        public void Apply() {
            int len;

            if(length == 0)
                len = destination.count - destinationIndex;
            else
                len = length;
                        
            for(int i = 0; i < length; i++)
                destination.SetColor(i + destinationIndex, source.GetColor(i + sourceIndex));
        }

        public void Revert() {
            for(int i = 0; i < length; i++)
                destination.RevertColor(i + destinationIndex);
        }

        void OnEnable() {
            if(applyOnEnable)
                Apply();
        }

        void OnDisable() {
            if(revertOnDisable)
                Revert();
        }
    }
}