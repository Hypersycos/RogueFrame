using Cinemachine.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Hypersycos.RogueFrame
{
    [CustomEditor(typeof(AbilitySO))]
    [CanEditMultipleObjects]
    public class AbilitySOEditor : Editor
    {
        SerializedProperty name;
        SerializedProperty description;
        SerializedProperty castTime;
        SerializedProperty animationTime;
        SerializedProperty requirements;
        SerializedProperty castEffects;
        SerializedProperty delayedCastEffects;
        SerializedProperty iconPainters;

        void OnEnable()
        {
            name = FindPropertyByAutoPropertyName("Name");
            description = FindPropertyByAutoPropertyName("Description");
            castTime = FindPropertyByAutoPropertyName("CastTime");
            animationTime = FindPropertyByAutoPropertyName("AnimationTime");
            requirements = serializedObject.FindProperty("Requirements");
            castEffects = serializedObject.FindProperty("CastEffects");
            delayedCastEffects = serializedObject.FindProperty("DelayedCastEffects");
            iconPainters = serializedObject.FindProperty("IconPainters");
        }

        private SerializedProperty FindPropertyByAutoPropertyName(string propName)
        {
            return serializedObject.FindProperty(string.Format("<{0}>k__BackingField", propName));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(name);
            EditorGUILayout.PropertyField(description);
            EditorGUILayout.PropertyField(castTime);
            EditorGUILayout.PropertyField(animationTime);
            ShowPolymorphicList<IAbilityRequirement>(requirements, new GUIContent("Add Requirement"));
            ShowPolymorphicList<ICastEffect>(castEffects, new GUIContent("Add Cast Effect"));
            ShowPolymorphicList<ICastEffect>(delayedCastEffects, new GUIContent("Add Delayed Cast Effect"));
            ShowPolymorphicList<IDrawIcon>(iconPainters, new GUIContent("Add Icon Painter"));
            serializedObject.ApplyModifiedProperties();
        }

        public void ShowPolymorphicList<T>(SerializedProperty list, GUIContent buttonLabel)
        {
            list.isExpanded = EditorGUILayout.Foldout(list.isExpanded, list.displayName, EditorStyles.foldoutHeader);
            EditorGUI.indentLevel += 1;
            if (list.isExpanded)
            {
                for (int i = 0; i < list.arraySize; i++)
                {
                    SerializedProperty element = list.GetArrayElementAtIndex(i);
                    string name = element.type;
                    int start = name.IndexOf("<") + 1;
                    name = name.Substring(start, name.Length-start-1);
                    EditorGUILayout.PropertyField(element, new GUIContent(name), true);
                }
            }
            EditorGUI.indentLevel -= 1;
            DrawDropdown<T>(buttonLabel, list);
        }

        public void DrawDropdown<T>(GUIContent label, SerializedProperty field)
        {
            if (!EditorGUILayout.DropdownButton(label, FocusType.Passive))
            {
                return;
            }

            void handleItemClicked(object parameter)
            {
                Type type = (Type)parameter;
                T instance = (T)Activator.CreateInstance(type);
                field.GetArrayElementAtIndex(field.arraySize++).managedReferenceValue = instance;
                serializedObject.ApplyModifiedProperties();
            }

            GenericMenu menu = new GenericMenu();
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes()).Where(t => t.GetInterfaces().Contains(typeof(T)));
            foreach (var type in types)
            {
                menu.AddItem(new GUIContent(type.Name), false, handleItemClicked, type);
            }
            menu.ShowAsContext();
        }

    }
}
