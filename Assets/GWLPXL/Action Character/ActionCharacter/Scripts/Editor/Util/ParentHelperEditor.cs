using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEngine.Animations;

namespace GWLPXL.ActionCharacter.Editor
{
    [CustomEditor(typeof(ParentHelper))]
    public class ParentHelperEditor : UnityEditor.Editor
    {
        protected string find = string.Empty;
        protected int selection = 0;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            ParentHelper c = (ParentHelper)target;
            if (c.RigRoot == null || c.ParentConstraint == null)
            {
                ActionEditorHelper.DrawHelpBox("Assign a RigRoot and Parent Constraint to use");
                return;
            }
            ActionEditorHelper.DrawHelpBox("Type the name of the transform and click Find to auto locate. Name must match but doesnt need to be case sensitive");
            find = EditorGUILayout.TextField(find);
            bool confirm = ActionEditorHelper.Button("Find");
            if (confirm)
            {
                find = CommonFunctions.StringKey(find);
                Transform[] trans = c.RigRoot.GetComponentsInChildren<Transform>();
                for (int i = 0; i < trans.Length; ++i)
                {
                    if (CommonFunctions.WordEquals(find, CommonFunctions.StringKey(trans[i].name)))
                    {
                        List<ConstraintSource> s = new List<ConstraintSource>();
                        c.ParentConstraint.GetSources(s);
                        bool add = true;
                        for (int j = 0; j < s.Count; j++)
                        {
                            if (s[j].sourceTransform == trans[i])
                            {
                                Debug.LogWarning("Already added trnasform " + trans[i].name);
                                add = false;
                                break;
                            }
                        }

                        if (add)
                        {
                            ConstraintSource source = new ConstraintSource();
                            source.sourceTransform = trans[i];
                            source.weight = 1;
                            c.ParentConstraint.AddSource(source);
                        }
            
                        break;
                    }
                }
               
            }
        }
    }
}
