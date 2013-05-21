using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/UI/ModalCharacterDialog")]
[ExecuteInEditMode()]
public class UIModalCharacterDialog : UIController {
    //index = -1 if there are no choices and we clicked next
    public delegate void OnAction(int choiceIndex);

    public const string defaultModalRef = "CharacterDialog";

#if UNITY_EDITOR
    public bool editorRefresh = false; //only use in editor
#endif

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

    public event OnAction actionCallback;

    private struct ChoiceData {
        public UIEventListener listener;
        public UIButtonKeys keys;
    }

    private NGUILayoutFlow mChoiceFlow = null;
    private NGUILayoutFlow mContentLayoutFlow = null; //this is to arrange all elements inside the body vertically
    private NGUILayoutFlow mBodyLayoutFlow = null; //this is to arrange the portrait and the content horizontally

    private ChoiceData[] mChoiceEvents = null;

    public static UIModalCharacterDialog Open(string modalRef, string text, string name = null, string portraitSpriteRef = null, string[] choices = null) {
        UIModalManager ui = UIModalManager.instance;
        UIModalCharacterDialog dlg = ui.ModalGetController<UIModalCharacterDialog>(modalRef);

        if(dlg != null) {
            dlg.Apply(text, name, portraitSpriteRef, choices);

            if(!ui.ModalIsInStack(modalRef)) {
                ui.ModalOpen(modalRef); //will show on the next update
            }
        }
        else {
            Debug.LogWarning("Failed to open dialog: " + modalRef);
        }

        return dlg;
    }

    /// <summary>
    /// Set the text and such for the dialog, call this before opening the dialog
    /// </summary>
    public void Apply(string text, string name = null, string portraitSpriteRef = null, string[] choices = null) {
        textLabel.text = text;

        if(nameLabel != null)
            nameLabel.text = name;

        if(portrait != null) {
            if(portraitSpriteRef != null) {
                portrait.spriteName = portraitSpriteRef;
                portrait.gameObject.SetActive(true);
            }
            else {
                portrait.gameObject.SetActive(false);
            }
        }

        //apply choices
        if(mChoiceEvents != null) {
            ResetChoices();

            if(choices != null && choices.Length > 0) {
                int max = Mathf.Min(choices.Length, mChoiceEvents.Length);

                mChoiceEvents[0].keys.selectOnUp = mChoiceEvents[max - 1].keys;
                mChoiceEvents[0].keys.gameObject.SetActive(true);

                mChoiceEvents[max - 1].keys.selectOnDown = mChoiceEvents[0].keys;
                mChoiceEvents[max - 1].keys.gameObject.SetActive(true);

                for(int i = 1; i < max - 1; i++) {
                    mChoiceEvents[i].keys.gameObject.SetActive(true);
                    mChoiceEvents[i].keys.selectOnDown = mChoiceEvents[i + 1].keys;
                }
            }
        }

        //if dialog is already open, apply positioning
        if(gameObject.activeSelf)
            Reposition();
    }

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
        foreach(ChoiceData dat in mChoiceEvents) {
            if(dat.listener != null)
                dat.listener.onClick = null;
        }

        actionCallback = null;
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
        //for selector, activate first choice
        if(active) {
            if(mChoiceEvents != null && mChoiceEvents[0].keys.gameObject.activeInHierarchy) {
                UICamera.selectedObject = mChoiceEvents[0].keys.gameObject;
            }
            else {
                UICamera.selectedObject = gameObject;
            }
        }
    }

    protected override void OnOpen() {
        Reposition();
    }

    protected override void OnClose() {
        //reset choices
        ResetChoices();
    }

    //this would be called if we have no choice events, or a mouse click. If there are choices, ignore
    void OnClick() {
        if(mChoiceEvents == null && actionCallback != null) {
            actionCallback(-1);

        }
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

            mChoiceEvents = new ChoiceData[choiceContainer.GetChildCount()];

            //setup callback for click
            //disable all choices
            for(int i = 0; i < mChoiceEvents.Length; i++) {
                Transform choice = choiceContainer.GetChild(i);
                mChoiceEvents[i].listener = choice.GetComponent<UIEventListener>();
                mChoiceEvents[i].keys = choice.GetComponent<UIButtonKeys>();

                mChoiceEvents[i].listener.onClick = delegate(GameObject go) {
                    if(actionCallback != null)
                        actionCallback(i);
                };
                //

                choice.gameObject.SetActive(false);
            }
        }
    }

    private void ResetChoices() {
        if(mChoiceEvents != null) {
            foreach(ChoiceData dat in mChoiceEvents) {
                dat.listener.gameObject.SetActive(false);
                dat.keys.selectOnUp = null;
                dat.keys.selectOnDown = null;
            }
        }
    }
}
