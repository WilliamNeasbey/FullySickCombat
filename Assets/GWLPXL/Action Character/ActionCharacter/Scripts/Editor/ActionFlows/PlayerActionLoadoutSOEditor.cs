using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GWLPXL.ActionCharacter.Editor
{
    public class PlayerActionLoadoutSOEditor : UnityEditor.Editor
    {


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            //SerializedProperty sets = serializedObject.FindProperty("ActionSets");
            //ActionEditorHelper.DrawProperty(sets);


        }
    }
}
