using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

namespace M8.UI {
    /// <summary>
    /// Use this for controlling the UI with InputManager
    /// </summary>
    [AddComponentMenu("M8/UI/InputModule")]
    public class InputModule : PointerInputModule {
        private float m_PrevActionTime;
        private Vector2 m_LastMoveVector;
        private int m_ConsecutiveMoveCount = 0;

        private Vector2 m_LastMousePosition;
        private Vector2 m_MousePosition;

        protected InputModule() {
        }

        [SerializeField]        
        private int m_playerIndex;
        
        [SerializeField]
        [InputAction]
        private int m_HorizontalAxis = InputManager.ActionInvalid;

        /// <summary>
        /// Name of the vertical axis for movement (if axis events are used).
        /// </summary>
        [SerializeField]
        [InputAction]
        private int m_VerticalAxis = InputManager.ActionInvalid;

        /// <summary>
        /// Name of the submit button.
        /// </summary>
        [SerializeField]
        [InputAction]
        private int m_SubmitButton = InputManager.ActionInvalid;

        /// <summary>
        /// Name of the submit button.
        /// </summary>
        [SerializeField]
        [InputAction]
        private int m_CancelButton = InputManager.ActionInvalid;

        [SerializeField]
        private float m_InputActionsPerSecond = 10;

        [SerializeField]
        private float m_RepeatDelay = 0.5f;

        [SerializeField]
        private bool m_ForceModuleActive;
        
        public bool forceModuleActive {
            get { return m_ForceModuleActive; }
            set { m_ForceModuleActive = value; }
        }

        public float inputActionsPerSecond {
            get { return m_InputActionsPerSecond; }
            set { m_InputActionsPerSecond = value; }
        }

        public float repeatDelay {
            get { return m_RepeatDelay; }
            set { m_RepeatDelay = value; }
        }

        public int playerIndex {
            get { return m_playerIndex; }
            set { m_playerIndex = value; }
        }

        /// <summary>
        /// Name of the horizontal axis for movement (if axis events are used).
        /// </summary>
        public int horizontalAxis {
            get { return m_HorizontalAxis; }
            set { m_HorizontalAxis = value; }
        }

        /// <summary>
        /// Name of the vertical axis for movement (if axis events are used).
        /// </summary>
        public int verticalAxis {
            get { return m_VerticalAxis; }
            set { m_VerticalAxis = value; }
        }

        public int submitButton {
            get { return m_SubmitButton; }
            set { m_SubmitButton = value; }
        }

        public int cancelButton {
            get { return m_CancelButton; }
            set { m_CancelButton = value; }
        }
        
        ///////////////
        // M8 expanded
        public static InputModule instance { get; set; }

        private int mLockInputCounter;

        public bool lockInput {
            get { return mLockInputCounter > 0; }
            set {
                if(value)
                    mLockInputCounter++;
                else
                    mLockInputCounter--;
            }
        }
        ///////////////

        public override void UpdateModule() {
            m_LastMousePosition = m_MousePosition;
            m_MousePosition = input.mousePosition;
        }

        public override bool IsModuleSupported() {
            return m_ForceModuleActive || input.mousePresent || input.touchSupported;
        }

        public override bool ShouldActivateModule() {
            if(!base.ShouldActivateModule())
                return false;

            InputManager inputMgr = InputManager.instance;

            var shouldActivate = m_ForceModuleActive;
            shouldActivate |= inputMgr.IsPressed(m_playerIndex, m_SubmitButton);
            shouldActivate |= inputMgr.IsPressed(m_playerIndex, m_CancelButton);
            shouldActivate |= !Mathf.Approximately(inputMgr.GetAxis(m_playerIndex, m_HorizontalAxis), 0.0f);
            shouldActivate |= !Mathf.Approximately(inputMgr.GetAxis(m_playerIndex, m_VerticalAxis), 0.0f);
            shouldActivate |= (m_MousePosition - m_LastMousePosition).sqrMagnitude > 0.0f;
            shouldActivate |= input.GetMouseButtonDown(0);
            
            if(input.touchCount > 0)
                shouldActivate = true;

            return shouldActivate;
        }

        public override void ActivateModule() {
            base.ActivateModule();
            m_MousePosition = input.mousePosition;
            m_LastMousePosition = input.mousePosition;

            var toSelect = eventSystem.currentSelectedGameObject;
            if(toSelect == null)
                toSelect = eventSystem.firstSelectedGameObject;

            eventSystem.SetSelectedGameObject(toSelect, GetBaseEventData());
        }

        public override void DeactivateModule() {
            base.DeactivateModule();
            ClearSelection();
        }

        public override void Process() {
            if(lockInput)
                return;

            bool usedEvent = SendUpdateEventToSelectedObject();

            if(eventSystem.sendNavigationEvents) {
                if(!usedEvent)
                    usedEvent |= SendMoveEventToSelectedObject();

                if(!usedEvent)
                    SendSubmitEventToSelectedObject();
            }

            // touch needs to take precedence because of the mouse emulation layer
            if(!ProcessTouchEvents() && input.mousePresent)
                ProcessMouseEvent();
        }

        private bool ProcessTouchEvents() {
            for(int i = 0; i < input.touchCount; ++i) {
                Touch touch = input.GetTouch(i);

                if(touch.type == TouchType.Indirect)
                    continue;

                bool released;
                bool pressed;
                var pointer = GetTouchPointerEventData(touch, out pressed, out released);

                ProcessTouchPress(pointer, pressed, released);

                if(!released) {
                    ProcessMove(pointer);
                    ProcessDrag(pointer);
                }
                else
                    RemovePointerData(pointer);
            }
            return input.touchCount > 0;
        }

        protected void ProcessTouchPress(PointerEventData pointerEvent, bool pressed, bool released) {
            var currentOverGo = pointerEvent.pointerCurrentRaycast.gameObject;

            // PointerDown notification
            if(pressed) {
                pointerEvent.eligibleForClick = true;
                pointerEvent.delta = Vector2.zero;
                pointerEvent.dragging = false;
                pointerEvent.useDragThreshold = true;
                pointerEvent.pressPosition = pointerEvent.position;
                pointerEvent.pointerPressRaycast = pointerEvent.pointerCurrentRaycast;

                DeselectIfSelectionChanged(currentOverGo, pointerEvent);

                if(pointerEvent.pointerEnter != currentOverGo) {
                    // send a pointer enter to the touched element if it isn't the one to select...
                    HandlePointerExitAndEnter(pointerEvent, currentOverGo);
                    pointerEvent.pointerEnter = currentOverGo;
                }

                // search for the control that will receive the press
                // if we can't find a press handler set the press
                // handler to be what would receive a click.
                var newPressed = ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.pointerDownHandler);

                // didnt find a press handler... search for a click handler
                if(newPressed == null)
                    newPressed = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

                // Debug.Log("Pressed: " + newPressed);

                float time = Time.unscaledTime;

                if(newPressed == pointerEvent.lastPress) {
                    var diffTime = time - pointerEvent.clickTime;
                    if(diffTime < 0.3f)
                        ++pointerEvent.clickCount;
                    else
                        pointerEvent.clickCount = 1;

                    pointerEvent.clickTime = time;
                }
                else {
                    pointerEvent.clickCount = 1;
                }

                pointerEvent.pointerPress = newPressed;
                pointerEvent.rawPointerPress = currentOverGo;

                pointerEvent.clickTime = time;

                // Save the drag handler as well
                pointerEvent.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(currentOverGo);

                if(pointerEvent.pointerDrag != null)
                    ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.initializePotentialDrag);
            }

            // PointerUp notification
            if(released) {
                // Debug.Log("Executing pressup on: " + pointer.pointerPress);
                ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);

                // Debug.Log("KeyCode: " + pointer.eventData.keyCode);

                // see if we mouse up on the same element that we clicked on...
                var pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

                // PointerClick and Drop events
                if(pointerEvent.pointerPress == pointerUpHandler && pointerEvent.eligibleForClick) {
                    ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerClickHandler);
                }
                else if(pointerEvent.pointerDrag != null && pointerEvent.dragging) {
                    ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.dropHandler);
                }

                pointerEvent.eligibleForClick = false;
                pointerEvent.pointerPress = null;
                pointerEvent.rawPointerPress = null;

                if(pointerEvent.pointerDrag != null && pointerEvent.dragging)
                    ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);

                pointerEvent.dragging = false;
                pointerEvent.pointerDrag = null;

                if(pointerEvent.pointerDrag != null)
                    ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);

                pointerEvent.pointerDrag = null;

                // send exit events as we need to simulate this on touch up on touch device
                ExecuteEvents.ExecuteHierarchy(pointerEvent.pointerEnter, pointerEvent, ExecuteEvents.pointerExitHandler);
                pointerEvent.pointerEnter = null;
            }
        }

        /// <summary>
        /// Process submit keys.
        /// </summary>
        protected bool SendSubmitEventToSelectedObject() {
            if(eventSystem.currentSelectedGameObject == null)
                return false;

            InputManager inputMgr = InputManager.instance;

            var data = GetBaseEventData();
            if(inputMgr.IsPressed(m_playerIndex, m_SubmitButton))
                ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.submitHandler);

            if(inputMgr.IsPressed(m_playerIndex, m_CancelButton))
                ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.cancelHandler);
            return data.used;
        }

        private Vector2 GetRawMoveVector() {
            InputManager inputMgr = InputManager.instance;

            Vector2 move = Vector2.zero;
            move.x = inputMgr.GetAxis(m_playerIndex, m_HorizontalAxis);
            move.y = inputMgr.GetAxis(m_playerIndex, m_VerticalAxis);

            if(inputMgr.IsDown(m_playerIndex, m_HorizontalAxis)) {
                if(move.x < 0)
                    move.x = -1f;
                if(move.x > 0)
                    move.x = 1f;
            }
            if(inputMgr.IsDown(m_playerIndex, m_VerticalAxis)) {
                if(move.y < 0)
                    move.y = -1f;
                if(move.y > 0)
                    move.y = 1f;
            }
            return move;
        }

        /// <summary>
        /// Process keyboard events.
        /// </summary>
        protected bool SendMoveEventToSelectedObject() {
            float time = Time.unscaledTime;

            Vector2 movement = GetRawMoveVector();
            if(Mathf.Approximately(movement.x, 0f) && Mathf.Approximately(movement.y, 0f)) {
                m_ConsecutiveMoveCount = 0;
                return false;
            }

            InputManager inputMgr = InputManager.instance;

            // If user pressed key again, always allow event
            bool allow = inputMgr.IsDown(m_playerIndex, m_HorizontalAxis) || inputMgr.IsDown(m_playerIndex, m_VerticalAxis);
            bool similarDir = (Vector2.Dot(movement, m_LastMoveVector) > 0);
            if(!allow) {
                // Otherwise, user held down key or axis.
                // If direction didn't change at least 90 degrees, wait for delay before allowing consequtive event.
                if(similarDir && m_ConsecutiveMoveCount == 1)
                    allow = (time > m_PrevActionTime + m_RepeatDelay);
                // If direction changed at least 90 degree, or we already had the delay, repeat at repeat rate.
                else
                    allow = (time > m_PrevActionTime + 1f / m_InputActionsPerSecond);
            }
            if(!allow)
                return false;

            // Debug.Log(m_ProcessingEvent.rawType + " axis:" + m_AllowAxisEvents + " value:" + "(" + x + "," + y + ")");
            var axisEventData = GetAxisEventData(movement.x, movement.y, 0.6f);

            if(axisEventData.moveDir != MoveDirection.None) {
                ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, axisEventData, ExecuteEvents.moveHandler);
                if(!similarDir)
                    m_ConsecutiveMoveCount = 0;
                m_ConsecutiveMoveCount++;
                m_PrevActionTime = time;
                m_LastMoveVector = movement;
            }
            else {
                m_ConsecutiveMoveCount = 0;
            }

            return axisEventData.used;
        }

        protected void ProcessMouseEvent() {
            ProcessMouseEvent(0);
        }

        protected virtual bool ForceAutoSelect() {
            return false;
        }

        /// <summary>
        /// Process all mouse events.
        /// </summary>
        protected void ProcessMouseEvent(int id) {
            var mouseData = GetMousePointerEventData(id);
            var leftButtonData = mouseData.GetButtonState(PointerEventData.InputButton.Left).eventData;

            if(ForceAutoSelect())
                eventSystem.SetSelectedGameObject(leftButtonData.buttonData.pointerCurrentRaycast.gameObject, leftButtonData.buttonData);

            // Process the first mouse button fully
            ProcessMousePress(leftButtonData);
            ProcessMove(leftButtonData.buttonData);
            ProcessDrag(leftButtonData.buttonData);

            // Now process right / middle clicks
            ProcessMousePress(mouseData.GetButtonState(PointerEventData.InputButton.Right).eventData);
            ProcessDrag(mouseData.GetButtonState(PointerEventData.InputButton.Right).eventData.buttonData);
            ProcessMousePress(mouseData.GetButtonState(PointerEventData.InputButton.Middle).eventData);
            ProcessDrag(mouseData.GetButtonState(PointerEventData.InputButton.Middle).eventData.buttonData);

            if(!Mathf.Approximately(leftButtonData.buttonData.scrollDelta.sqrMagnitude, 0.0f)) {
                var scrollHandler = ExecuteEvents.GetEventHandler<IScrollHandler>(leftButtonData.buttonData.pointerCurrentRaycast.gameObject);
                ExecuteEvents.ExecuteHierarchy(scrollHandler, leftButtonData.buttonData, ExecuteEvents.scrollHandler);
            }
        }

        protected bool SendUpdateEventToSelectedObject() {
            if(eventSystem.currentSelectedGameObject == null)
                return false;

            var data = GetBaseEventData();
            ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.updateSelectedHandler);
            return data.used;
        }

        /// <summary>
        /// Process the current mouse press.
        /// </summary>
        protected void ProcessMousePress(MouseButtonEventData data) {
            var pointerEvent = data.buttonData;
            var currentOverGo = pointerEvent.pointerCurrentRaycast.gameObject;

            // PointerDown notification
            if(data.PressedThisFrame()) {
                pointerEvent.eligibleForClick = true;
                pointerEvent.delta = Vector2.zero;
                pointerEvent.dragging = false;
                pointerEvent.useDragThreshold = true;
                pointerEvent.pressPosition = pointerEvent.position;
                pointerEvent.pointerPressRaycast = pointerEvent.pointerCurrentRaycast;

                DeselectIfSelectionChanged(currentOverGo, pointerEvent);

                // search for the control that will receive the press
                // if we can't find a press handler set the press
                // handler to be what would receive a click.
                var newPressed = ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.pointerDownHandler);

                // didnt find a press handler... search for a click handler
                if(newPressed == null)
                    newPressed = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

                // Debug.Log("Pressed: " + newPressed);

                float time = Time.unscaledTime;

                if(newPressed == pointerEvent.lastPress) {
                    var diffTime = time - pointerEvent.clickTime;
                    if(diffTime < 0.3f)
                        ++pointerEvent.clickCount;
                    else
                        pointerEvent.clickCount = 1;

                    pointerEvent.clickTime = time;
                }
                else {
                    pointerEvent.clickCount = 1;
                }

                pointerEvent.pointerPress = newPressed;
                pointerEvent.rawPointerPress = currentOverGo;

                pointerEvent.clickTime = time;

                // Save the drag handler as well
                pointerEvent.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(currentOverGo);

                if(pointerEvent.pointerDrag != null)
                    ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.initializePotentialDrag);
            }

            // PointerUp notification
            if(data.ReleasedThisFrame()) {
                // Debug.Log("Executing pressup on: " + pointer.pointerPress);
                ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);

                // Debug.Log("KeyCode: " + pointer.eventData.keyCode);

                // see if we mouse up on the same element that we clicked on...
                var pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

                // PointerClick and Drop events
                if(pointerEvent.pointerPress == pointerUpHandler && pointerEvent.eligibleForClick) {
                    ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerClickHandler);
                }
                else if(pointerEvent.pointerDrag != null && pointerEvent.dragging) {
                    ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.dropHandler);
                }

                pointerEvent.eligibleForClick = false;
                pointerEvent.pointerPress = null;
                pointerEvent.rawPointerPress = null;

                if(pointerEvent.pointerDrag != null && pointerEvent.dragging)
                    ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);

                pointerEvent.dragging = false;
                pointerEvent.pointerDrag = null;

                // redo pointer enter / exit to refresh state
                // so that if we moused over somethign that ignored it before
                // due to having pressed on something else
                // it now gets it.
                if(currentOverGo != pointerEvent.pointerEnter) {
                    HandlePointerExitAndEnter(pointerEvent, null);
                    HandlePointerExitAndEnter(pointerEvent, currentOverGo);
                }
            }
        }

        ///////////////
        // M8 Expanded
        protected override void OnDestroy() {
            base.OnDestroy();

            if(instance == this)
                instance = null;
        }

        protected override void Awake() {
            base.Awake();

            if(instance == null)
                instance = this;
        }
        ///////////////
    }
}