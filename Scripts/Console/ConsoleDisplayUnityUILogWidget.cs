using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleDisplayUnityUILogWidget : MonoBehaviour {
    public Image icon;
    public Text label;

    public void Init(Sprite iconImage, string text, Color color) {
        if(icon) {
            if(iconImage) {
                icon.gameObject.SetActive(true);

                icon.sprite = iconImage;
                icon.SetNativeSize();
            }
            else
                icon.gameObject.SetActive(false);
        }

        if(label) {
            label.text = text;
            label.color = color;
        }
    }
}
