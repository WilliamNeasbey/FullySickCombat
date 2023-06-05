
#if UNITY_EDITOR

using System.Collections.Generic;

using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GWLPXL.ActionCharacter.Editor
{

    [UnityEditor.CustomEditor(typeof(FlowControlSO))]
    public class FlowControlSOEditor : UnityEditor.Editor
    {
        SerializedProperty flows;

        public override VisualElement CreateInspectorGUI()
        {

  
            return base.CreateInspectorGUI();
        }
        public override void OnInspectorGUI()
        {
            flows = serializedObject.FindProperty("registered");
            serializedObject.Update();

            ActionEditorHelper.DrawLabel("Readonly Registered Flows");
            if (Application.isPlaying)
            {
                EditorGUI.BeginDisabledGroup(true);
                ActionEditorHelper.DrawProperty(flows);
                EditorGUI.EndDisabledGroup();
            }

            serializedObject.ApplyModifiedProperties();
            flows.Dispose();


        }


       
    }


}
#endif
