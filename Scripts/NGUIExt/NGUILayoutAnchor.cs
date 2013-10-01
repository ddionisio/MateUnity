using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[AddComponentMenu("M8/NGUI/LayoutAnchor")]
public class NGUILayoutAnchor : NGUILayoutBase {

    public enum DirectionHorizontal {
        Left,
        Right,
        Center,
        Stretch,
        None
    }

    public enum DirectionVertical {
        Top,
        Bottom,
        Center,
        Stretch,
        None
    }

    public Transform target;

    public Vector2 ofs;
    public Vector2 padding;

    public DirectionHorizontal dirHorz;
    public DirectionVertical dirVert;

    private UIWidget mWidget = null;

    public override void Reposition() {
        if(target != null) {
            Transform trans = transform;

            Vector3 pos = trans.localPosition;

            Bounds targetBound = NGUIMath.CalculateRelativeWidgetBounds(trans.parent, target);

            if(mWidget == null) {
                mWidget = GetComponent<UIWidget>();
            }

            switch(dirHorz) {
                case DirectionHorizontal.Left:
                    pos.x = targetBound.min.x + padding.x;
                    break;
                case DirectionHorizontal.Center:
                    pos.x = targetBound.center.x + padding.x;
                    break;
                case DirectionHorizontal.Right:
                    pos.x = targetBound.max.x + padding.x;
                    break;
                case DirectionHorizontal.Stretch:
                    pos.x = targetBound.min.x + padding.x;

                    if(mWidget) {
                        int w = Mathf.RoundToInt(targetBound.size.x - padding.x * 2.0f);
                        if(w > Mathf.Epsilon)
                            mWidget.width = Mathf.RoundToInt(targetBound.size.x - padding.x * 2.0f);
                    }
                    break;
            }

            switch(dirVert) {
                case DirectionVertical.Top:
                    pos.y = targetBound.max.y + padding.y;
                    break;
                case DirectionVertical.Center:
                    pos.y = targetBound.center.y + padding.y;
                    break;
                case DirectionVertical.Bottom:
                    pos.y = targetBound.min.y + padding.y;
                    break;
                case DirectionVertical.Stretch:
                    pos.y = targetBound.max.y - padding.y;

                    if(mWidget) {
                        int h = Mathf.RoundToInt(targetBound.size.y - padding.y * 2.0f);
                        if(h > Mathf.Epsilon)
                            mWidget.height = h;
                    }
                    break;
            }

            pos.x += ofs.x;
            pos.y += ofs.y;

            if(rounding) {
                pos.x = Mathf.Round(pos.x);
                pos.y = Mathf.Round(pos.y);
            }

            trans.localPosition = pos;
        }
    }
}
