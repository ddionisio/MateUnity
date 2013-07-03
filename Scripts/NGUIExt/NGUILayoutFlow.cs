using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[AddComponentMenu("M8/NGUI/LayoutFlow")]
public class NGUILayoutFlow : NGUILayoutBase {

    public enum Arrangement {
        Horizontal,
        Vertical,
    }

    public enum LineUp {
        None,
        Center,
        End
    }

    public Arrangement arrangement = Arrangement.Horizontal;
    public float padding;
    public bool relativeLineup = false;

    public LineUp lineup = LineUp.None;
    public LineUp lineup2 = LineUp.None;

    public bool alphaSort = false;

    public Transform[] contents; //if empty, will iterate through children

    private Bounds[] mBounds;

    public override void Reposition() {
        Transform[] _contents;
        if(alphaSort) {
            _contents = new Transform[transform.childCount];
            for(int i = 0; i < _contents.Length; i++) {
                _contents[i] = transform.GetChild(i);
            }

            System.Array.Sort(_contents, (t1, t2) => t1.name.CompareTo(t2.name));
        }
        else {
            _contents = contents;
        }

        bool useContents = _contents != null && _contents.Length > 0;
        int count = useContents ? _contents.Length : transform.childCount;

        Bounds b = new Bounds();

        //calculate bounds

        if(mBounds == null || mBounds.Length != count)
            mBounds = new Bounds[count];

        float bMax = float.MinValue, bMin = float.MaxValue;

        for(int i = 0; i < count; ++i) {
            Transform t = useContents ? _contents[i] : transform.GetChild(i);

            b = NGUIMath.CalculateRelativeWidgetBounds(t);
            Vector3 scale = t.localScale;
            b.min = Vector3.Scale(b.min, scale);
            b.max = Vector3.Scale(b.max, scale);
            mBounds[i] = b;

            switch(arrangement) {
                case Arrangement.Horizontal:
                    if(bMax < b.max.y)
                        bMax = b.max.y;
                    if(bMin > b.min.y)
                        bMin = b.min.y;
                    break;

                case Arrangement.Vertical:
                    if(bMax < b.max.x)
                        bMax = b.max.x;
                    if(bMin > b.min.x)
                        bMin = b.min.x;
                    break;
            }
        }

        //arrange contents

        float offset = 0;

        for(int i = 0; i < count; ++i) {
            Transform t = useContents ? _contents[i] : transform.GetChild(i);

            if(!t.gameObject.activeSelf) continue;

            b = mBounds[i];

            Vector3 pos = t.localPosition;

            switch(arrangement) {
                case Arrangement.Horizontal:
                    pos.x = offset + b.extents.x - b.center.x;
                    pos.y = relativeLineup ? 0 : -(b.extents.y + b.center.y) + (b.max.y - b.min.y - bMax + bMin) * 0.5f;

                    offset += b.max.x - b.min.x + padding;
                    break;

                case Arrangement.Vertical:
                    pos.x = relativeLineup ? 0 : (b.extents.x - b.center.x) + (b.min.x - bMin);
                    pos.y = -(offset + b.extents.y + b.center.y);

                    offset += b.size.y + padding;
                    break;
            }

            if(rounding && lineup == LineUp.None && lineup2 == LineUp.None) {
                pos.x = Mathf.Round(pos.x);
                pos.y = Mathf.Round(pos.y);
            }

            t.localPosition = pos;
        }

        //Lineup

        if(lineup != LineUp.None || lineup2 != LineUp.None) {
            b = NGUIMath.CalculateRelativeWidgetBounds(transform);
        }

        switch(lineup) {
            case LineUp.None:
                if(lineup2 != LineUp.None) {
                    switch(arrangement) {
                        case Arrangement.Horizontal:
                            for(int i = 0; i < count; ++i) {
                                Transform t = useContents ? _contents[i] : transform.GetChild(i);

                                Vector3 pos = t.localPosition;

                                switch(lineup2) {
                                    case LineUp.Center:
                                        pos.y += rounding ? Mathf.Round(b.extents.y) : b.extents.y;
                                        break;

                                    case LineUp.End:
                                        pos.y += rounding ? Mathf.Round(b.size.y) : b.size.y;
                                        break;
                                }

                                t.localPosition = pos;
                            }
                            break;

                        case Arrangement.Vertical:
                            for(int i = 0; i < count; ++i) {
                                Transform t = useContents ? _contents[i] : transform.GetChild(i);

                                Vector3 pos = t.localPosition;

                                switch(lineup2) {
                                    case LineUp.Center:
                                        pos.x -= rounding ? Mathf.Round(b.extents.x) : b.extents.x;
                                        break;

                                    case LineUp.End:
                                        pos.x -= rounding ? Mathf.Round(b.size.x) : b.size.x;
                                        break;
                                }

                                t.localPosition = pos;
                            }
                            break;
                    }
                }
                break;

            case LineUp.Center:
                switch(arrangement) {
                    case Arrangement.Horizontal:
                        for(int i = 0; i < count; ++i) {
                            Transform t = useContents ? _contents[i] : transform.GetChild(i);

                            Vector3 pos = t.localPosition;

                            switch(lineup2) {
                                case LineUp.Center:
                                    pos.y += rounding ? Mathf.Round(b.extents.y) : b.extents.y;
                                    break;

                                case LineUp.End:
                                    pos.y += rounding ? Mathf.Round(b.size.y) : b.size.y;
                                    break;
                            }

                            pos.x -= b.extents.x;

                            if(rounding)
                                pos.x = Mathf.Round(pos.x);

                            t.localPosition = pos;
                        }
                        break;

                    case Arrangement.Vertical:
                        for(int i = 0; i < count; ++i) {
                            Transform t = useContents ? _contents[i] : transform.GetChild(i);

                            Vector3 pos = t.localPosition;

                            switch(lineup2) {
                                case LineUp.Center:
                                    pos.x -= rounding ? Mathf.Round(b.extents.x) : b.extents.x;
                                    break;

                                case LineUp.End:
                                    pos.x -= rounding ? Mathf.Round(b.size.x) : b.size.x;
                                    break;
                            }

                            pos.y += b.extents.y;

                            if(rounding)
                                pos.y = Mathf.Round(pos.y);

                            t.localPosition = pos;
                        }
                        break;
                }
                break;

            case LineUp.End:
                switch(arrangement) {
                    case Arrangement.Horizontal:
                        //TODO
                        foreach(Transform t in transform) {
                            Vector3 pos = t.localPosition;

                            switch(lineup2) {
                                case LineUp.Center:
                                    pos.y += rounding ? Mathf.Round(b.extents.y) : b.extents.y;
                                    break;

                                case LineUp.End:
                                    pos.y += rounding ? Mathf.Round(b.size.y) : b.size.y;
                                    break;
                            }

                            pos.x -= b.size.x;

                            if(rounding)
                                pos.x = Mathf.Round(pos.x);

                            t.localPosition = pos;
                        }
                        break;

                    case Arrangement.Vertical:
                        for(int i = 0; i < count; ++i) {
                            Transform t = useContents ? _contents[i] : transform.GetChild(i);

                            Vector3 pos = t.localPosition;

                            switch(lineup2) {
                                case LineUp.Center:
                                    pos.x -= rounding ? Mathf.Round(b.extents.x) : b.extents.x;
                                    break;

                                case LineUp.End:
                                    pos.x -= rounding ? Mathf.Round(b.size.x) : b.size.x;
                                    break;
                            }

                            pos.y += b.size.y;// .extents.y);

                            if(rounding)
                                pos.y = Mathf.Round(pos.y);

                            t.localPosition = pos;
                        }
                        break;
                }
                break;
        }
    }
}