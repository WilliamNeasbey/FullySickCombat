#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace GWLPXL.ActionCharacter.Editor
{

    [CustomEditor(typeof(CharacterActionLoadoutSO))]
    public class CharacterActionLoadoutSOEditor : UnityEditor.Editor
    {
        string[] selections = new string[0];
        int[] previousactionselection = new int[0];
        int selected;
        public override VisualElement CreateInspectorGUI()
        {


            return base.CreateInspectorGUI();
        }
        public override void OnInspectorGUI()
        {
            CharacterActionLoadoutSO so = (CharacterActionLoadoutSO)target;
            //List<string> names = new List<string>();
            //so.AllinSet = EditorGUILayout.ObjectField(so.AllinSet, typeof(ActionsDatabaseSO), false) as ActionsDatabaseSO;
            //if (so.AllinSet != null)
            //{
            //    for (int i = 0; i < so.AllinSet.Database.Actions.Count; i++)
            //    {
            //        if (so.AllinSet.Database.Actions[i] == null) continue;
            //        names.Add(so.AllinSet.Database.Actions[i].Movement.Movements.ScriptedMovement.Name);
            //    }
            //    ActionEditorHelper.DrawStringHelpers(names);
            //}
            //selections = names.ToArray();
            //selected = GUILayout.SelectionGrid(selected, selections, 4);

            base.OnInspectorGUI();
            serializedObject.Update();

        
           
            for (int i = 0; i < so.ActionSets.Count; i++)
            {
     
                //PlayerActionSet set = so.ActionSets.ActionSets[i];
                //set.Flow.StartingAction = set.Action;


            }

            //previousactionselection = actionselection;
            serializedObject.ApplyModifiedProperties();
        }
    }

}

#endif