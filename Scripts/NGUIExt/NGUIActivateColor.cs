using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/NGUI/ActivateColor")]
public class NGUIActivateColor : MonoBehaviour {

    public struct Item {
        public UIWidget widget;
        public Color inactiveColor;
        public Color defaultColor;
    }

    public Color inactiveColor;
    public bool hover;
    public bool includeInactive;

    private Item[] mItems;
    private bool mStarted = false;

    void OnEnable() {
        if(mStarted) {
            if(UICamera.selectedObject == gameObject || (hover && UICamera.IsHighlighted(gameObject)))
                RevertColor();
            else
                ApplyColor();
        }
    }

    void OnSelect(bool yes) {
        if(yes)
            RevertColor();
        else
            ApplyColor();
    }

    void OnHover(bool yes) {
        if(UICamera.selectedObject == gameObject || yes)
            RevertColor();
        else
            ApplyColor();
    }

    void Awake() {
        UIWidget[] widgets = GetComponentsInChildren<UIWidget>(includeInactive);
        mItems = new Item[widgets.Length];
        for(int i = 0, max = mItems.Length; i < max; i++) {
            mItems[i] = new Item() { widget = widgets[i], inactiveColor = widgets[i].color * inactiveColor, defaultColor = widgets[i].color };
            widgets[i].color = mItems[i].inactiveColor;
        }
    }

    void Start() {
        mStarted = true;
    }

    void ApplyColor() {
        for(int i = 0, max = mItems.Length; i < max; i++) {
            Item itm = mItems[i];
            itm.widget.color = itm.inactiveColor;
        }
    }

    void RevertColor() {
        for(int i = 0, max = mItems.Length; i < max; i++) {
            Item itm = mItems[i];
            itm.widget.color = itm.defaultColor;
        }
    }
}
