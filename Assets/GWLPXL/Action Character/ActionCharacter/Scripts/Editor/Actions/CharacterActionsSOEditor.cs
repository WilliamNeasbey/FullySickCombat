#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using UnityEditorInternal;

using UnityEngine.UIElements;

namespace GWLPXL.ActionCharacter.Editor
{
    /// <summary>
    /// 
    /// </summary>
  
    [CustomEditor(typeof(CharacterActionsSO))]
    public class CharacterActionsSOEditor : UnityEditor.Editor
    {
        readonly string[] tabs = new string[12] { "Original", "Requirements", "Blocking", "Multipliers", "Movement", "Animator", "InputBuffers", "Extend Options", "Early Exit Options", "Timing Options", "Targeting Options", "HitBox Options" };//, "Custom Code" };
        int tab = 0;
        SerializedProperty custom;
        SerializedProperty movements;
        SerializedProperty requirements;
        SerializedProperty scripted;
        SerializedProperty movementsfield;
        SerializedProperty movementbehavior;
        SerializedProperty movementSequence;
        List<string> readonlynamelist = new List<string>();

        protected Vector2[] scrollv = new Vector2[12];
        bool[] rootmotionbools = new bool[0];
        AcquireTargetType[] targetingbools = new AcquireTargetType[0];

        List<SerializedProperty> disposable = new List<SerializedProperty>();
        public override VisualElement CreateInspectorGUI()
        {
            scrollv = new Vector2[tabs.Length];
            return base.CreateInspectorGUI();

        }
        public override void OnInspectorGUI()
        {

        
            movements = serializedObject.FindProperty("Movement");
            disposable.Add(movements);
            movementsfield = movements.FindPropertyRelative("Movements");
            disposable.Add(movementsfield);
            scripted = movementsfield.FindPropertyRelative("ScriptedMovement");
            disposable.Add(scripted);

            requirements = scripted.FindPropertyRelative("CharacterRequirements");
            disposable.Add(requirements);
            movementbehavior = scripted.FindPropertyRelative("MovementBehavior");
            disposable.Add(movementbehavior);
            movementSequence = movementbehavior.FindPropertyRelative("MovementSequence");
            disposable.Add(movementSequence);

            movements = serializedObject.FindProperty("Movement");
            disposable.Add(movements);

            requirements = scripted.FindPropertyRelative("CharacterRequirements");
            disposable.Add(requirements);
            movementbehavior = scripted.FindPropertyRelative("MovementBehavior");
            disposable.Add(movementbehavior);
            movementSequence = movementbehavior.FindPropertyRelative("MovementSequence");
            disposable.Add(movementSequence);

            SerializedProperty d = null;
            disposable.Add(d);


            serializedObject.Update();
            CharacterActionsSO so = (CharacterActionsSO)target;



            EditorGUILayout.BeginVertical();
            GUILayout.Space(10);
            //EditorGUILayout.LabelField(so.Movement.Movements.ScriptedMovement.Name, EditorStyles.boldLabel);
            //EditorGUILayout.LabelField("Name Asset to Scripted Movement Name", EditorStyles.boldLabel);
            //so.AutoName = EditorGUILayout.Toggle(so.AutoName);
            EditorGUILayout.HelpBox("Hit Enter to save when name is complete", MessageType.Info);
            so.Movement.Movements.ScriptedMovement.Name = EditorGUILayout.TextField(so.Movement.Movements.ScriptedMovement.Name);
            if (so.AutoName && string.IsNullOrEmpty(so.Movement.Movements.ScriptedMovement.Name) == false)
            {
                
                if (Event.current.keyCode == KeyCode.Return)
                {
                    
                    string name = so.Movement.Movements.ScriptedMovement.Name;
                    string soname = so.name;
                    if (CommonFunctions.WordEquals(name, soname) == false)
                    {
                        string path = AssetDatabase.GetAssetPath(so);
                        AssetDatabase.RenameAsset(path, name);
                    }
                }
               // so.name = so.Movement.Movements.ScriptedMovement.Name;
            }
            GUILayout.Space(10);

            EditorGUILayout.EndVertical();

            serializedObject.Update();


            tab = ActionEditorHelper.PopupSelection(tab, tabs);
            //tab = GUILayout.SelectionGrid(tab, tabs, 4);

            readonlynamelist.Clear();
            List<ActionCCVars> varsarr = so.Movement.Movements.ScriptedMovement.MovementBehavior.MovementSequence;
            rootmotionbools = new bool[varsarr.Count];
            for (int i = 0; i < rootmotionbools.Length; i++)
            {
                rootmotionbools[i] = varsarr[i].AnimatorVars.ApplyRootMotion;
            }
            targetingbools = new AcquireTargetType[varsarr.Count];
            for (int i = 0; i < varsarr.Count; i++)
            {
                targetingbools[i] = varsarr[i].TargetOptions.AcquireTargetType;
            }

            scrollv[tab] = GUILayout.BeginScrollView(scrollv[tab]);
            switch (tab)
            {
                case 0:

                    base.OnInspectorGUI();
 
                    break;
                case 1:
                    //movement
                    bool airborne = false;
                    CharacterRequirements chrequire = so.Movement.Movements.ScriptedMovement.CharacterRequirements;
                    for (int i = 0; i < chrequire.RequiredStates.Count; i++)
                    {
                        if (chrequire.RequiredStates[i] == FreeFormState.Airborne)
                        {
                            airborne = true;
                        }
                        readonlynamelist.Add(chrequire.RequiredStates[i].ToString());
                    }

                    ReadonlyHelpBox("Current Requirements, READONLY", readonlynamelist);
                    if (airborne)
                    {
                        ActionEditorHelper.DrawLabel("Required Costs");
                        ActionEditorHelper.DrawHelpBox("How much does this cost to begin performing the action in the air? The max amount of actions in the air is defind on the movement scriptable under the falling category. For example, a max of 1 and a cost of 1 would mean I can perform the action once while airborne. A common example is using it to limit the amount of air jumps, i.e. double jumping.");
                        so.Movement.Movements.ScriptedMovement.AirborneCost = EditorGUILayout.IntField("Airborne Cost: ", so.Movement.Movements.ScriptedMovement.AirborneCost);
                    }


                    ActionEditorHelper.SmallGUISpace();
                    EditorGUILayout.PropertyField(requirements);
                    break;
      
                case 2:

                    for (int i = 0; i < varsarr.Count; i++)
                    {

                        BlockingOptions blocking = varsarr[i].BlockingOptions;
                        
                        EditorGUILayout.LabelField("Blocking Options for Sequence " + i);
                        EditorGUI.indentLevel++;
                        GUILayout.Space(10);
                        EditorGUILayout.LabelField("Initial Blocking");
                        EditorGUI.BeginChangeCheck();
                        blocking.InitialBlocking.EndActionOnBlocked = EditorGUILayout.Toggle("End Action On Blocked: ", blocking.InitialBlocking.EndActionOnBlocked);
                        var layersinitial = EditorGUILayout.MaskField("Layers", LayerMaskToField(blocking.InitialBlocking.BlockingLayers), InternalEditorUtility.layers);
                        for (int j = 0; j < blocking.InitialBlocking.DirectionChecks.Count; j++)
                        {
                            blocking.InitialBlocking.DirectionChecks[j] = (DirectionType)EditorGUILayout.EnumPopup("Direction Check: ", blocking.InitialBlocking.DirectionChecks[j]);
                        }

                        if (blocking.InitialBlocking.DirectionChecks.Count > 0)
                        {
                            blocking.InitialBlocking.DistanceCheck = EditorGUILayout.FloatField("Radius Check: ", blocking.InitialBlocking.DistanceCheck);
                        }

                        if (ActionEditorHelper.AddButton("Direction Check"))
                        {
                            blocking.InitialBlocking.DirectionChecks.Add(DirectionType.None);
                        }
                       

                        if (blocking.InitialBlocking.DirectionChecks.Count > 0)
                        {
                            if (ActionEditorHelper.RemoveButton("Last Direction Check"))
                            {
                                blocking.InitialBlocking.DirectionChecks.RemoveAt(blocking.InitialBlocking.DirectionChecks.Count - 1);
                            }
                        }
                       

                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(so, "Initial Layers Changed");
                            blocking.InitialBlocking.BlockingLayers = FieldToLayerMask(layersinitial);
                        }

                        GUILayout.Space(10);
                      
                    }


                    break;
                case 3:
                    //multies
                    for (int i = 0; i < varsarr.Count; i++)
                    {
                        EditorGUILayout.LabelField("Multiplier Options for Sequence " + i);
                        SerializedProperty prop = movementSequence.GetArrayElementAtIndex(i);
                        DrawProperty(so, prop, "Multipliers", disposable);
                        disposable.Add(prop);
                    }
                    break;
                case 4:
                    //movement
                    for (int i = 0; i < varsarr.Count; i++)
                    {
                        EditorGUILayout.LabelField("Movement Options for Sequence " + i);
                        SerializedProperty prop = movementSequence.GetArrayElementAtIndex(i);
                        disposable.Add(prop);
                        bool draw = true;
                        DrawProperty(so, prop, "TravelX", disposable, draw);
                        DrawProperty(so, prop, "TravelY", disposable, draw);
                        DrawProperty(so, prop, "TravelZ", disposable, draw);


                        if (targetingbools[i] != AcquireTargetType.None)//this is per index...
                        {
                            EditorGUILayout.HelpBox("If no targets are found, this movement will be the default.", MessageType.Info);
                        }

                        if (rootmotionbools[i] == true)
                        {
                            EditorGUILayout.HelpBox("Root Motion will override these values. Disable Root Motion to use.", MessageType.Warning);
                        }


                        
                    }


                    break;
                case 5:
                    //anim
                    for (int i = 0; i < varsarr.Count; i++)
                    {
                        EditorGUILayout.LabelField("Animator Options for Sequence " + i);
                        SerializedProperty prop = movementSequence.GetArrayElementAtIndex(i);

                        SerializedProperty p = DrawProperty(so, prop, "AnimatorVars", disposable, true);
                        disposable.Add(prop);
                        if (targetingbools[i] != AcquireTargetType.None)
                        {
                            EditorGUILayout.HelpBox("Root Motion can not be used with Targeting. Disable Root Motion or Targeting.", MessageType.Warning);
                        }
                    }
                    break;
                case 6:
                    //input buffers
                    for (int i = 0; i < varsarr.Count; i++)
                    {
                        EditorGUILayout.LabelField("Input Buffers for Sequence " + i);
                        SerializedProperty prop = movementSequence.GetArrayElementAtIndex(i);
                        disposable.Add(prop);
                        DrawProperty(so, prop, "BufferVars", disposable);
                    }

                    break;
                case 7:
                    //extend buffers
                    for (int i = 0; i < varsarr.Count; i++)
                    {
                        EditorGUILayout.LabelField("Extend Options for Sequence " + i);
                        SerializedProperty prop = movementSequence.GetArrayElementAtIndex(i);
                        disposable.Add(prop);
                        DrawProperty(so, prop, "ExtendOptions", disposable);
                    }

                    break;
                case 8:
                    //early exit
                    for (int i = 0; i < varsarr.Count; i++)
                    {
                        EditorGUILayout.LabelField("Early Exit Options for Sequence " + i);
                        SerializedProperty prop = movementSequence.GetArrayElementAtIndex(i);
                        disposable.Add(prop);
                        DrawProperty(so, prop, "EarlyExitOptions", disposable);
                    }

                    break;
                case 9:
                    //tming options
                    for (int i = 0; i < varsarr.Count; i++)
                    {
                        EditorGUILayout.LabelField("Timing Options for Sequence " + i);
                        SerializedProperty prop = movementSequence.GetArrayElementAtIndex(i);
                        disposable.Add(prop);
                        DrawProperty(so, prop, "TimingOptions", disposable);
                    }
                    break;
                case 10:
                    //auto target
                    for (int i = 0; i < varsarr.Count; i++)
                    {
                        EditorGUILayout.LabelField("Targeting Options for Sequence " + i);
                        SerializedProperty prop = movementSequence.GetArrayElementAtIndex(i);
                        disposable.Add(prop);

                        SerializedProperty p= DrawProperty(so, prop, "TargetOptions", disposable, true);
                        disposable.Add(p);
                        if (rootmotionbools[i])
                        {
                            EditorGUILayout.HelpBox("Targeting can not be used with Root Motion. Disable Root Motion to use.", MessageType.Warning);
                        }
                    }
                    break;
                case 11:
                    //hitboxes
                    for (int i = 0; i < varsarr.Count; i++)
                    {
                        EditorGUILayout.LabelField("Hit Box Options for Sequence " + i);
                        SerializedProperty prop = movementSequence.GetArrayElementAtIndex(i);
                        disposable.Add(prop);
                    DrawProperty(so, prop, "HitBoxOptions", disposable);
                    }
                    break;
                case 12:
                    //custom code
                    for (int i = 0; i < varsarr.Count; i++)
                    {
                        EditorGUILayout.LabelField("Custom Code Assets for Sequence " + i);
                        SerializedProperty customcode = movementSequence.GetArrayElementAtIndex(i);

                        disposable.Add(customcode);
                       DrawProperty(so, customcode, "Custom Code Assets", disposable, true);
                    }
   


                    break;


            }
            GUILayout.EndScrollView();


            ActionEditorHelper.ImportExportButtons(so);

           
            serializedObject.ApplyModifiedProperties();

            DisposeSe(disposable);
            EditorUtility.SetDirty(so);

        }

        void DisposeSe(List<SerializedProperty> props)
        {
            for (int i = 0; i < props.Count; i++)
            {
                if (props[i] == null) continue;
                props[i].Dispose();
            }
            props.Clear();
        }
        // Converts a LayerMask to a field value
        private int LayerMaskToField(LayerMask mask)
        {
            int field = 0;
            var layers = InternalEditorUtility.layers;
            for (int c = 0; c < layers.Length; c++)
            {
                if ((mask & (1 << LayerMask.NameToLayer(layers[c]))) != 0)
                {
                    field |= 1 << c;
                }
            }
            return field;
        }

        private LayerMask FieldToLayerMask(int field)
        {
            LayerMask mask = 0;
            var layers = InternalEditorUtility.layers;
            for (int c = 0; c < layers.Length; c++)
            {
                if ((field & (1 << c)) != 0)
                {
                    mask |= 1 << LayerMask.NameToLayer(layers[c]);
                }
            }
            return mask;
        }

        private static SerializedProperty DrawProperty(ScriptableObject target, SerializedProperty prop, string propertyname, List<SerializedProperty> clean, bool draw = true)
        {
            SerializedProperty multiinside = prop.FindPropertyRelative(propertyname);
            EditorGUI.indentLevel++;
            EditorGUI.BeginChangeCheck();
            if (draw)
            {
                EditorGUILayout.PropertyField(multiinside);
            }

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, propertyname + "Changed");
            }
            EditorGUI.indentLevel--;
            clean.Add(multiinside);
            return multiinside;
        }

        private void ReadonlyHelpBox(string helpLabel, List<string> names)
        {
            EditorGUILayout.BeginVertical();
            GUILayout.Space(10);
            EditorGUILayout.LabelField(helpLabel, EditorStyles.helpBox);
            EditorGUI.indentLevel++;
            for (int i = 0; i < names.Count; i++)
            {
                EditorGUILayout.LabelField(names[i], EditorStyles.boldLabel);

            }
           
            EditorGUI.indentLevel--;
            GUILayout.Space(10);
            EditorGUILayout.EndVertical();
        }
    }
}
#endif