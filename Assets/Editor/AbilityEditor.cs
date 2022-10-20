using Cinemachine.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Printing;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Hypersycos.RogueFrame
{
    [CustomEditor(typeof(Ability))]
    [CanEditMultipleObjects]
    public class AbilityEditor : Editor
    {
        SerializedProperty Name;
        SerializedProperty Description;
        SerializedProperty CastTime;
        SerializedProperty AnimationTime;
        SerializedProperty Requirements;
        SerializedProperty CastEffects;
        SerializedProperty DelayedCastEffects;
        SerializedProperty IconPainters;

        void OnEnable()
        {
            Name = this.FindPropertyByAutoPropertyName("Name");
            Description = this.FindPropertyByAutoPropertyName("Description");
            CastTime = this.FindPropertyByAutoPropertyName("CastTime");
            AnimationTime = this.FindPropertyByAutoPropertyName("AnimationTime");
            Requirements = serializedObject.FindProperty("Requirements");
            CastEffects = serializedObject.FindProperty("CastEffects");
            DelayedCastEffects = serializedObject.FindProperty("DelayedCastEffects");
            IconPainters = serializedObject.FindProperty("IconPainters");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(Name);
            EditorGUILayout.PropertyField(Description);
            EditorGUILayout.PropertyField(CastTime);
            EditorGUILayout.PropertyField(AnimationTime);
            this.ShowPolymorphicList<IAbilityRequirement>(Requirements, new GUIContent("Add Requirement"));
            this.ShowPolymorphicList<ICastType>(CastEffects, new GUIContent("Add Cast Effect"));
            this.ShowPolymorphicList<ICastType>(DelayedCastEffects, new GUIContent("Add Delayed Cast Effect"));
            this.ShowPolymorphicList<IDrawIcon>(IconPainters, new GUIContent("Add Icon Painter"));
            serializedObject.ApplyModifiedProperties();
        }

    }
}
