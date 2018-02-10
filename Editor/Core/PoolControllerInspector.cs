using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace M8 {
    [CustomEditor(typeof(PoolController))]
    public class PoolControllerInspector : Editor {
        private SerializedProperty mGroupName;
        private SerializedProperty mPoolHolder;

        //private SerializedProperty mFactory;
        private UnityEditorInternal.ReorderableList mFactory;

        private bool mGroupNameOverride;

        void OnEnable() {
            mGroupName = serializedObject.FindProperty("group");
            mPoolHolder = serializedObject.FindProperty("poolHolder");

            //mFactory = serializedObject.FindProperty("factory");
            mFactory = new UnityEditorInternal.ReorderableList(serializedObject, serializedObject.FindProperty("factory"), true, false, true, true);
            mFactory.drawElementCallback = OnFactoryItemRender;
            mFactory.elementHeight = 140f;

            mGroupNameOverride = !string.IsNullOrEmpty(mGroupName.stringValue);
        }

        public override void OnInspectorGUI() {
            EditorGUILayout.BeginVertical(GUI.skin.box);

            if(mGroupNameOverride) {
                EditorGUILayout.PropertyField(mGroupName);
            }
            else {
                EditorGUILayout.LabelField("group", (target as MonoBehaviour).name);
            }

            mGroupNameOverride = EditorGUILayout.Toggle("override", mGroupNameOverride);
            if(!mGroupNameOverride)
                mGroupName.stringValue = "";

            EditorGUILayout.EndVertical();
            
            EditorGUILayout.PropertyField(mPoolHolder);

            M8.EditorExt.Utility.DrawSeparator();

            mFactory.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }

        void OnFactoryItemRender(Rect rect, int index, bool isActive, bool isFocused) {
            SerializedProperty item = mFactory.serializedProperty.GetArrayElementAtIndex(index);

            Rect itemPos = rect;
            itemPos.y += 4.0f;
            itemPos.height = 16.0f;

            const float padding = 4.0f;

            var nameProp = item.FindPropertyRelative("name");

            EditorGUI.PropertyField(itemPos, nameProp);
            itemPos.y += itemPos.height + padding;

            var templateProp = item.FindPropertyRelative("template");
            var lastTemplateObj = templateProp.objectReferenceValue;
                        
            EditorGUI.PropertyField(itemPos, templateProp);
            itemPos.y += itemPos.height + padding;

            //set name to template object if it's empty
            var curTemplateObj = templateProp.objectReferenceValue;
            if(curTemplateObj != lastTemplateObj && curTemplateObj && string.IsNullOrEmpty(nameProp.stringValue)) {
                nameProp.stringValue = templateProp.objectReferenceValue.name;
            }

            EditorGUI.PropertyField(itemPos, item.FindPropertyRelative("startCapacity"));
            itemPos.y += itemPos.height + padding;

            EditorGUI.PropertyField(itemPos, item.FindPropertyRelative("maxCapacity"));
            itemPos.y += itemPos.height + padding;

            EditorGUI.PropertyField(itemPos, item.FindPropertyRelative("defaultParent"));
            itemPos.y += itemPos.height + padding;

            itemPos.y += padding;

            /*Rect copyPos = itemPos; copyPos.height = 20.0f; copyPos.width = rect.width*0.3f; copyPos.x = rect.x + rect.width*0.5f - rect.width*0.3f - 4.0f;
            if(GUI.Button(copyPos, "Copy Name")) {
                Transform t = item.FindPropertyRelative("template").objectReferenceValue as Transform;
                if(t)
                    EditorGUIUtility.systemCopyBuffer = t.name;
            }*/

            //Rect dupPos = itemPos; dupPos.height = 20.0f; dupPos.width = rect.width*0.3f; dupPos.x = rect.x + rect.width*0.5f + 4.0f;
            Rect dupPos = itemPos; dupPos.height = 20.0f; dupPos.width = rect.width; dupPos.x = rect.x;
            if(GUI.Button(dupPos, "Duplicate")) {
                mFactory.serializedProperty.InsertArrayElementAtIndex(index);
                SerializedProperty newItem = mFactory.serializedProperty.GetArrayElementAtIndex(index);
                newItem.serializedObject.CopyFromSerializedProperty(item);
            }
        }
    }
}