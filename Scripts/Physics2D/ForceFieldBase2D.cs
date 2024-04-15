using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
#if !M8_PHYSICS2D_DISABLED
    [AddComponentMenu("")]
    public abstract class ForceFieldBase2D : MonoBehaviour {
        public enum FieldValueType {
            Accel, //value is multiplied by body's mass to acquire force
            Force //direct force value
        }

        public FieldValueType fieldType = FieldValueType.Accel;
        public float fieldValue = 9.81f;
        
        public bool isGlobal = false; //check as the default force field, there should only be one of these. This will always be applied to all ForceControllers

        public static ForceFieldBase2D global { get; private set; }

        public abstract Vector2 GetDir(ForceController2D entity);

        public float GetForce(ForceController2D entity) {
            switch(fieldType) {
                case FieldValueType.Accel:
                    return entity.body.mass * fieldValue;
                case FieldValueType.Force:
                    return fieldValue;
                default:
                    return 0f;
            }
        }

        public virtual void ItemRemoved(ForceController2D ctrl) {
        }

        protected virtual void OnDisable() {
            if(global == this)
                global = null;
        }

        protected virtual void OnEnable() {
            if(isGlobal)
                global = this;
        }
    }
#endif
}