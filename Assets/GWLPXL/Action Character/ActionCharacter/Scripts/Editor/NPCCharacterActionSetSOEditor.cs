#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GWLPXL.ActionCharacter.Editor
{

    //[CustomEditor(typeof(NPCCharacterActionSetsSO))]
    //public class NPCCharacterActionSetSOEditor : UnityEditor.Editor
    //{

    //    public override void OnInspectorGUI()
    //    {
    //        base.OnInspectorGUI();
    //        serializedObject.Update();

    //        NPCCharacterActionSetsSO so = (NPCCharacterActionSetsSO)target;
    //        for (int i = 0; i < so.ActionSets.ActionSets.Count; i++)
    //        {
    //            ActionSet set = so.ActionSets.ActionSets[i];
    //            set.Flow.StartingAction = set.Action;
    //        }

    //        serializedObject.ApplyModifiedProperties();
    //    }
    //}
}

#endif