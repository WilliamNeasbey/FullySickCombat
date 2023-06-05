#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace GWLPXL.ActionCharacter.Editor
{

    [CustomEditor(typeof(ActionsDatabaseSO))]
    public class ActionsDatabaseSOEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();


        }
    }
}

#endif