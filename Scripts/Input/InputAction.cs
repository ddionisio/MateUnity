using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    [CreateAssetMenu(fileName = "inputAction", menuName = "M8/Input Action")]
    public class InputAction : ScriptableObject, ISerializationCallbackReceiver {
        public const int keyCodeNone = 0;

        public enum ButtonAxis {
            None,
            Plus,
            Minus
        }

        public enum ButtonState {
            None,
            Pressed,
            Released,
            Down
        }

        [System.Serializable]
        public class Key {
            public string input; //for use with unity's input or external string
            public int code; //can be KeyCode or some other external id, 0 is considered None/Invalid

            public bool invert; //for axis, flip sign

            public ButtonAxis axis; //for buttons as axis

            public bool isValid {
                get { return !string.IsNullOrEmpty(input) || code != keyCodeNone; }
            }

            public void SetAsInput(string input) {
                Reset();
                this.input = input;
            }

            public void SetAsKey(int aCode) {
                Reset();
                code = aCode;
            }
            
            public void Reset() {
                input = "";
                code = keyCodeNone;
            }

            public float GetAxisValue() {
                float ret;

                switch(axis) {
                    case ButtonAxis.Plus:
                        ret = 1.0f;
                        break;
                    case ButtonAxis.Minus:
                        ret = -1.0f;
                        break;
                    default:
                        ret = 0f;
                        break;
                }

                return ret;
            }

            public void CopyTo(Key other) {
                other.input = input;
                other.code = code;
                other.invert = invert;
                other.axis = axis;
            }
        }

        //axis info
        public float deadZone = 0.1f;
        public bool forceRaw = false;

        public Key[] defaultBinds = new Key[1];
        
        public Key[] binds {
            get {
                if(mBinds == null || mBinds.Length != defaultBinds.Length)
                    InitBinds();

                return mBinds;
            }
        }
        
        private Key[] mBinds;
        
        public void ResetBinds() {
            if(mBinds == null || mBinds.Length != defaultBinds.Length) {
                InitBinds();
            }
            else {
                for(int i = 0; i < defaultBinds.Length; i++) {
                    if(mBinds[i] == null)
                        mBinds[i] = new Key();

                    defaultBinds[i].CopyTo(mBinds[i]);
                }
            }
        }

        public void ResetBind(int bindIndex) {
            defaultBinds[bindIndex].CopyTo(binds[bindIndex]);
        }
        
        public float GetAxis() {
            var _binds = binds;

            float axis = 0f;
            for(int i = 0; i < _binds.Length; i++) {
                axis = ProcessAxis(_binds[i]);
                if(axis != 0.0f)
                    break;
            }

            return axis;
        }

        public bool IsPressed() {
            var _binds = binds;
            for(int i = 0; i < _binds.Length; i++) {
                if(ProcessButtonPressed(_binds[i]))
                    return true;
            }

            return false;
        }

        public bool IsReleased() {
            var _binds = binds;
            for(int i = 0; i < _binds.Length; i++) {
                if(ProcessButtonReleased(_binds[i]))
                    return true;
            }

            return false;
        }

        public bool IsDown() {
            var _binds = binds;
            for(int i = 0; i < _binds.Length; i++) {
                if(ProcessButtonDown(_binds[i]))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Batch call from IsPressed, IsReleased, IsDown. Use this if you want to know more than one state within a frame
        /// </summary>
        public ButtonState GetButtonState() {
            var _binds = binds;
            for(int i = 0; i < _binds.Length; i++) {
                var key = _binds[i];

                if(ProcessButtonPressed(key))
                    return ButtonState.Pressed;
                                
                if(ProcessButtonReleased(key))
                    return ButtonState.Released;

                if(ProcessButtonDown(key))
                    return ButtonState.Down;
            }

            return ButtonState.None;
        }
        
        //implements

        public virtual string GetKeyString(Key key) {
            if(!string.IsNullOrEmpty(key.input))
                return key.input;

            if(key.code != keyCodeNone) {
                var keyCode = (KeyCode)key.code;

                if(keyCode == KeyCode.Escape)
                    return "ESC";
                else {
                    string s = keyCode.ToString();

                    int i = s.IndexOf("Joystick");
                    if(i != -1) {
                        int bInd = s.LastIndexOf('B');
                        if(bInd != -1) {
                            return s.Substring(bInd);
                        }
                    }

                    return s;
                }
            }

            return "";
        }
        
        protected virtual float ProcessAxis(Key key) {
            float val = 0f;

            if(!string.IsNullOrEmpty(key.input)) {
                if(Time.timeScale == 0.0f || forceRaw)
                    val = Input.GetAxisRaw(key.input);
                else
                    val = Input.GetAxis(key.input);
            }
            else if(key.code != keyCodeNone) {                
                if(Input.GetKey((KeyCode)key.code)) {
                    val = key.GetAxisValue();
                }
            }

            if(Mathf.Abs(val) > deadZone) {
                if(key.invert)
                    val *= -1.0f;

                return val;
            }

            return 0.0f;
        }

        protected virtual bool ProcessButtonDown(Key key) {
            if(!string.IsNullOrEmpty(key.input))
                return Input.GetButton(key.input);
            else
                return Input.GetKey((KeyCode)key.code);
        }

        /// <summary>
        /// Used by IsPressed
        /// </summary>
        protected virtual bool ProcessButtonPressed(Key key) {
            if(!string.IsNullOrEmpty(key.input))
                return Input.GetButtonDown(key.input);
            else
                return Input.GetKeyDown((KeyCode)key.code);
        }

        /// <summary>
        /// Used by IsReleased
        /// </summary>
        protected virtual bool ProcessButtonReleased(Key key) {
            if(!string.IsNullOrEmpty(key.input))
                return Input.GetButtonUp(key.input);
            else
                return Input.GetKeyUp((KeyCode)key.code);
        }

        private void InitBinds() {
            mBinds = new Key[defaultBinds.Length];

            for(int i = 0; i < defaultBinds.Length; i++) {
                if(mBinds[i] == null)
                    mBinds[i] = new Key();

                defaultBinds[i].CopyTo(mBinds[i]);
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() {

        }

        void ISerializationCallbackReceiver.OnAfterDeserialize() {
            //initialize mBinds to defaultBinds
            InitBinds();
        }
    }
}