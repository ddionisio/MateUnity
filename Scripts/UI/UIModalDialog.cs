using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/UI/ModalDialog")]
[ExecuteInEditMode()]
public class UIModalDialog : UIController {
    public delegate void OnChoice(int index);

    public bool editorRefresh = false; //only use in editor

    public UISprite frame; //optional
    public Vector2 framePadding;

    public UISprite background; //optional
    public Vector2 backgroundPadding;
        
    public UISprite portrait; //optional
    public UILabel nameLabel; //optional
    public UILabel textLabel; //required
            
    public Transform bodyContainer; //optional: this is the transform that encapsulates both the portrait and content
    public Transform contentContainer; //required: contains name, text, and the choices
    public Transform choiceContainer; //optional: each child must have a UIEventListener

    public event OnChoice choiceCallback;

    private NGUILayoutFlow mChoiceFlow = null;
    private NGUILayoutFlow mContentLayoutFlow = null; //this is to arrange all elements inside the body vertically
    private NGUILayoutFlow mBodyLayoutFlow = null; //this is to arrange the portrait and the content horizontally

    private UIEventListener[] mChoiceEvents = null;

    public void Reposition() {
        //refresh choice flow
        if(mChoiceFlow != null)
            mChoiceFlow.Reposition();

        Bounds bounds;

        //refresh content layout flow
        if(mContentLayoutFlow != null) {
            mContentLayoutFlow.Reposition();
        }

        //finally, the body, which will arrange content and portrait
        if(mBodyLayoutFlow != null) {
            mBodyLayoutFlow.Reposition();

            bounds = NGUIMath.CalculateRelativeWidgetBounds(bodyContainer);
        }
        else {
            bounds = NGUIMath.CalculateRelativeWidgetBounds(contentContainer);
        }

        //stretch frame and background based on bounds
        if(background != null) {
            M8.NGUIExtUtil.WidgetEncapsulateBoundsLocal(background, backgroundPadding, bounds);
        }

        if(frame != null) {
            M8.NGUIExtUtil.WidgetEncapsulateBoundsLocal(frame, framePadding, bounds);
        }
    }

    void OnDestroy() {
        foreach(UIEventListener listener in mChoiceEvents) {
            if(listener != null)
                listener.onClick = null;
        }

        choiceCallback = null;
    }

    void Awake() {
        InitData();
        
    }

    // Use this for initialization
    void Start() {

    }

#if UNITY_EDITOR
    // Update is called once per frame
    void Update() {
        if(editorRefresh) {
            InitData();
            Reposition();
            editorRefresh = false;
        }
    }
#endif

    protected override void OnActive(bool active) {
        if(active) {
        }
        else {
        }
    }

    protected override void OnOpen() {
    }

    protected override void OnClose() {
    }

    private void InitData() {
        if(portrait != null) {
            portrait.pivot = UIWidget.Pivot.TopLeft;
        }

        if(nameLabel != null) {
            nameLabel.pivot = UIWidget.Pivot.TopLeft;
        }

        if(textLabel != null) {
            textLabel.pivot = UIWidget.Pivot.TopLeft;
        }

        if(contentContainer != null) {
            mContentLayoutFlow = contentContainer.GetComponentInChildren<NGUILayoutFlow>();
            mContentLayoutFlow.arrangement = NGUILayoutFlow.Arrangement.Vertical;
            mContentLayoutFlow.rounding = true;
            mContentLayoutFlow.relativeLineup = false;

            mContentLayoutFlow.lineup = NGUILayoutFlow.LineUp.End;
            mContentLayoutFlow.lineup2 = NGUILayoutFlow.LineUp.Center;
        }

        if(bodyContainer != null) {
            if(mContentLayoutFlow != null) {
                mContentLayoutFlow.lineup = NGUILayoutFlow.LineUp.None;
                mContentLayoutFlow.lineup2 = NGUILayoutFlow.LineUp.None;
            }

            mBodyLayoutFlow = bodyContainer.GetComponentInChildren<NGUILayoutFlow>();
            mBodyLayoutFlow.arrangement = NGUILayoutFlow.Arrangement.Horizontal;
            mBodyLayoutFlow.rounding = true;
            mBodyLayoutFlow.relativeLineup = false;
            mBodyLayoutFlow.lineup = NGUILayoutFlow.LineUp.Center;
            mBodyLayoutFlow.lineup2 = NGUILayoutFlow.LineUp.End;
        }

        if(choiceContainer != null) {
            mChoiceFlow = choiceContainer.GetComponentInChildren<NGUILayoutFlow>();
            mChoiceFlow.arrangement = NGUILayoutFlow.Arrangement.Vertical;
            mChoiceFlow.rounding = true;
            mChoiceFlow.relativeLineup = false;
            mChoiceFlow.lineup = NGUILayoutFlow.LineUp.None;
            mChoiceFlow.lineup2 = NGUILayoutFlow.LineUp.None;

            mChoiceEvents = new UIEventListener[choiceContainer.GetChildCount()];

            //setup callback for click
            //disable all choices
            for(int i = 0; i < mChoiceEvents.Length; i++) {
                Transform choice = choiceContainer.GetChild(i);
                mChoiceEvents[i] = choice.GetComponent<UIEventListener>();

                mChoiceEvents[i].onClick = delegate(GameObject go) {
                    if(choiceCallback != null)
                        choiceCallback(i);
                };
                //

                choice.gameObject.SetActive(false);
            }
        }
    }
}
