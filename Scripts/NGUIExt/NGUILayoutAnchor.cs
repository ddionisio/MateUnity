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
        
    public override void Reposition() {
        if(target != null) {
            Transform trans = transform;

            Vector3 pos = trans.localPosition;
            Vector3 s = trans.localScale;

            Bounds targetBound = NGUIMath.CalculateRelativeWidgetBounds(trans.parent, target);

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
                    s.x = targetBound.size.x - padding.x * 2.0f;
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
                    s.y = targetBound.size.y - padding.y * 2.0f;
                    break;
            }

            pos.x += ofs.x;
            pos.y += ofs.y;

            if(rounding) {
                pos.x = Mathf.Round(pos.x);
                pos.y = Mathf.Round(pos.y);

                s.x = Mathf.Round(s.x);
                s.y = Mathf.Round(s.y);
            }

            //too small
            if(Mathf.Abs(s.x) <= float.Epsilon || Mathf.Abs(s.y) <= float.Epsilon)
                return;

            trans.localPosition = pos;
            trans.localScale = s;
        }
    }
}
