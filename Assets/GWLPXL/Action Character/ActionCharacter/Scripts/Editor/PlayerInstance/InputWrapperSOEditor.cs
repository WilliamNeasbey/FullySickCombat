#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GWLPXL.ActionCharacter.Editor
{


    [CustomEditor(typeof(InputWrapperSO), true)]
    public class InputWrapperSOEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            //InputWrapperSO so = (InputWrapperSO)target;
            //ActionEditorHelper.ImportExportButtons(so);
        }
    }
}
#endif