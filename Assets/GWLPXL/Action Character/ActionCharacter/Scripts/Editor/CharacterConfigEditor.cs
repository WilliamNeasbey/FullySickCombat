#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace GWLPXL.ActionCharacter.Editor
{

    [CustomEditor(typeof(CharacterConfig))]
    public class CharacterConfigEditor : UnityEditor.Editor
    {

        SerializedProperty defaults;

        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            CharacterConfig so = (CharacterConfig)target;
            EditorGUILayout.HelpBox("Defaults. If using external controllers for locomotion, put multiplier defaults to 0. Use state names for Locomotion and Airborne.", MessageType.Info);
            defaults = serializedObject.FindProperty("Defaults");
            ActionEditorHelper.DrawProperty(defaults);

            ActionEditorHelper.MediumGUISpace();
            EditorGUILayout.LabelField("Read Only Values", EditorStyles.boldLabel);
            ActionEditorHelper.MediumGUISpace();

            EditorGUI.BeginDisabledGroup(true);
            ActionEditorHelper.DrawStringHelpers(so, true, true, true, true);
            EditorGUI.EndDisabledGroup();

            serializedObject.ApplyModifiedProperties();
            defaults.Dispose();

          

        }
    }
}
#endif