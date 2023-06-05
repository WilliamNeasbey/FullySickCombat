#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace GWLPXL.ActionCharacter.Editor
{

    [CustomEditor(typeof(InputActionMapSO))]
    public class InputActionMapSOEditor : UnityEditor.Editor
    {


        public override VisualElement CreateInspectorGUI()
        {


            return base.CreateInspectorGUI();
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            return;
            InputActionMapSO so = (InputActionMapSO)target;
            ActionEditorHelper.DrawLabel("Inputs for Actions");

            for (int i = 0; i < so.InputSlots.Count; i++)
            {
                so.InputSlots[i].Requirements = EditorGUILayout.ObjectField(so.InputSlots[i].Requirements, typeof(InputActionSlotSO), true) as InputActionSlotSO;
               
               
            }


            GUILayout.Space(25);
            if (GUILayout.Button("Add New Slot"))
            {
                so.InputSlots.Add(new InputSlot());
            }

            if (GUILayout.Button("Remove Input Slot"))
            {
                if (so.InputSlots.Count > 0)
                {
                    so.InputSlots.RemoveAt(so.InputSlots.Count - 1);
                }
          
            }
        }

       
    }
}

#endif