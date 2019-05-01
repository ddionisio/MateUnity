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
            var len = GetLength();

            for(int i = 0; i < len; i++)
                destination.SetColor(i + destinationIndex, source.GetColor(i + sourceIndex));
        }

        public void Revert() {
            var len = GetLength();

            for(int i = 0; i < len; i++)
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

        private int GetLength() {
            int len;

            if(length == 0)
                len = destination.count - destinationIndex;
            else
                len = length;

            return len;
        }
    }
}