using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace M8 {
    [AddComponentMenu("M8/Modal/Dialog Unity")]
    public class ModalDialogUnity : ModalDialog {
        [Header("Display")]
        public Image iconImage;
        public bool iconHideIfEmpty = false; //if false, do not hide icon image if given icon is null.
        public bool iconResize = true; //if true, resize icon image based on given icon's dimension.
        public Text titleLabel;
        public Text descLabel;

        protected override void Setup(string title, string description, object icon) {
            if(titleLabel)
                titleLabel.text = title;

            if(descLabel)
                descLabel.text = description;

            if(iconImage) {
                var iconSprite = icon as Sprite;
                if(iconSprite) {
                    iconImage.gameObject.SetActive(true);
                    iconImage.sprite = iconSprite;

                    if(iconResize)
                        iconImage.SetNativeSize();
                }
                else if(iconHideIfEmpty)
                    iconImage.gameObject.SetActive(false);
            }
        }
    }
}