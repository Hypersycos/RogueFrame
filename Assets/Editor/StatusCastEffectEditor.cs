using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Hypersycos.RogueFrame
{
    [CustomEditor(typeof(StatusCastEffect))]
    public class StatusCastEffectEditor : Editor
    {
        SerializedProperty StatusInstance;

        void OnEnable()
        {
            //StatusInstance = serializedObject.FindProperty("statusEffect");
            StatusInstance = this.FindPropertyByAutoPropertyName("statusEffect");
        }

        public override void OnInspectorGUI()
        {
            void handleItemClicked(object parameter)
            {
                Type type = (Type)parameter;
                StatusInstance instance = (StatusInstance)Activator.CreateInstance(type);
                StatusInstance.managedReferenceValue = instance;
                serializedObject.ApplyModifiedProperties();
            }

            serializedObject.Update();
            EditorGUILayout.PropertyField(StatusInstance, true);
            this.DrawDropdown<StatusInstance>(new GUIContent("Status Instance"), StatusInstance, handleItemClicked);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
