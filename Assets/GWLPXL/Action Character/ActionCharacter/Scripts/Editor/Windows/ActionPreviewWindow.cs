using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace GWLPXL.ActionCharacter.Editor
{
    public static class WindowActionPreview
    {
        [MenuItem("GWLPXL/ActionCharacter/Windows/Action Preview Window")]
        public static void Init()
        {
            // Get existing open window or if none, make a new one:
            ActionPreviewWindow window = (ActionPreviewWindow)EditorWindow.GetWindow(typeof(ActionPreviewWindow));
            window.Show();
        }
        public static void ShowWindow(ActionCharacter character)
        {
            // Get existing open window or if none, make a new one:
            ActionPreviewWindow window = (ActionPreviewWindow)EditorWindow.GetWindow(typeof(ActionPreviewWindow));
            window.Character = character;
            window.Show();

        }
    }
    public class ActionPreviewWindow : UnityEditor.EditorWindow
    {
        string[] tabs = new string[2] { "Preview Window", "Combo Builder" };
        public ActionCharacter Character;
        public ActionCharacter characterCache;
        int tabselection;

        public ActionWindow Actionwindow;//resets on reload...
        ComboWindow comboWindow;//resets on reload...

        

        protected virtual void OnGUI()
        {
            Character = EditorGUILayout.ObjectField(Character, typeof(ActionCharacter), true) as ActionCharacter;
            if (Character == null)
            {
                return;
            }

            ActionEditorHelper.DrawLabel("Action Preview Window", ActionEditorHelper.GetActionSelectionActionMaker(24, TextAnchor.MiddleCenter));
            ActionEditorHelper.SmallGUISpace();
            ActionEditorHelper.GetHeaderColor();
            tabselection = ActionEditorHelper.TabSelection(tabselection, tabs, 3);
            ActionEditorHelper.ResetGUIColor();


            switch (tabselection)
            {
                case 0:
                    //Action window
                    ActionWindow(Character);
                    break;
                case 1:
                    //combo window
                    ComboWindow(Character);
                    break;
            }



        }

        void OnFocus()
        {
            // Remove delegate listener if it has previously
            // been assigned.
            SceneView.duringSceneGui -= this.OnSceneGUI;
            // Add (or re-add) the delegate.
            SceneView.duringSceneGui += this.OnSceneGUI;

        }
        void OnSceneGUI(SceneView sceneView)
        {
            // Do your drawing here using Handles.
            Handles.BeginGUI();
            //combo drawing
            switch (Actionwindow.Preview.EntireVars.Focus)
            {
                case PlaybackFocus.Combo:
                    ActionSequenceVisualizer.DrawCombo(Actionwindow.Preview.EntireVars.Combo.Actions, Actionwindow.Preview.EntireVars.FrameData, Actionwindow.Preview.EntireVars.FrameData);
                    ActionSequenceVisualizer.DrawHitBoxes(Character, Actionwindow.Preview.EntireVars.Combo);
                    break;
                case PlaybackFocus.Action:
                    if (Actionwindow.Preview.EntireVars.HitBoxData)
                    {
                        ActionSequenceVisualizer.DrawHitBoxes(Character, Actionwindow.Preview.EntireVars.Combo);
                    }
                    break;
            }
            // Do your drawing here using GUI.
            Handles.EndGUI();
        }

        void OnDestroy()
        {
            SceneView.duringSceneGui -= this.OnSceneGUI;
            DestroyImmediate(Actionwindow);
            DestroyImmediate(comboWindow);
        }

        void ComboWindow(ActionCharacter character)
        {
            if (comboWindow == null)
            {
                comboWindow = CreateInstance<ComboWindow>();
            }
           
            comboWindow.Draw(character);
        }

        void ActionWindow(ActionCharacter character)
        {
            if (Actionwindow == null)
            {
                Actionwindow = CreateInstance<ActionWindow>();
            }
           
            if (character != characterCache && Actionwindow != null)
            {
                DestroyImmediate(Actionwindow);
                characterCache = character;
            }
            Actionwindow.Draw(character);

           
        }




       
    }


    /// <summary>
    /// working on combo window for creating sequences
    /// </summary>
    public class ComboWindow : ScriptableObject
    {
 
        [System.Serializable]
        public class ComboPiece
        {
            [HideInInspector]
            public bool Remove;
            [HideInInspector]
            public bool Edit;
            [HideInInspector]
            public int Selected;
            public ActionSO Action;
            public InputActionSlotSO Input;
            public List<int> Cancels = new List<int>();
            public List<ActionSO> CancelActions = new List<ActionSO>();
            [Tooltip("Duration of combo forgiveness.")]
            public float Forgiveness = .25f;
        }
        ActionCharacter character;
        public ActionsDatabaseSO Database;
        public CharacterActionLoadoutSO Loadout;
        public PreviewVars Preview;
        EditorInspectorDraw[] tempdbed = new EditorInspectorDraw[2] {
            new EditorInspectorDraw(),
            new EditorInspectorDraw() };

        VanillaActionList tempdb;
        int selectedflow = 0;
        int selectedaction = 0;
        Vector2 sv;
        public List<ComboPiece> Pieces = new List<ComboPiece>();
        private void OnEnable()
        {
           if (tempdb == null)
            {
                tempdb = CreateInstance<VanillaActionList>();
            }
            if (Preview == null)
            {
                Preview = CreateInstance<PreviewVars>();
            }
        }

        private void OnDisable()
        {
            if (tempdb != null) DestroyImmediate(tempdb);
            if (Preview != null) DestroyImmediate(Preview);
        }
        int comboselection;
        int nextselection;
        bool inprogress = true;
        int comboLength = 0;
        public void Draw(ActionCharacter character)
        {
            SerializedObject ob = new SerializedObject(this);
            ob.Update();
            SerializedProperty p = ob.FindProperty("Pieces");
            if (inprogress)
            {
                ActionEditorHelper.DrawHelpBox("Currently a work in progress. Coming Soon.");
                return;
            }

            Database = EditorGUILayout.ObjectField(Database, typeof(ActionsDatabaseSO), false) as ActionsDatabaseSO;
            Loadout = EditorGUILayout.ObjectField(Loadout, typeof(CharacterActionLoadoutSO), false) as CharacterActionLoadoutSO;
            if (Loadout == null || Database == null)
            {
                return;
            }

            sv = EditorGUILayout.BeginScrollView(sv);
            //build the set
            ActionEditorHelper.DrawLabel("Combo Length");
            comboLength = EditorGUILayout.IntField(comboLength);
            ActionEditorHelper.DrawLabel("Combo Moves");

            p.arraySize = comboLength;
            for (int i = 0; i < p.arraySize; i++)
            {
                SerializedProperty piece = p.GetArrayElementAtIndex(i);
                SerializedProperty s = piece.FindPropertyRelative("Selected");
                SerializedProperty a = piece.FindPropertyRelative("Action");
                SerializedProperty e = piece.FindPropertyRelative("Edit");
                SerializedProperty r = piece.FindPropertyRelative("Remove");
                s.intValue = ActionEditorHelper.PopupSelection(s.intValue, Database.GetActionNames());
                a.objectReferenceValue = Database.Database.Actions[s.intValue];

                EditorGUILayout.BeginHorizontal();
                ActionEditorHelper.DrawProperty(a);
                e.boolValue = ActionEditorHelper.Button("Edit");
                r.boolValue = ActionEditorHelper.Button("Remove");
                EditorGUILayout.EndHorizontal();

                SerializedProperty f = piece.FindPropertyRelative("Forgiveness");
                ActionEditorHelper.DrawProperty(f);

                SerializedProperty c = piece.FindPropertyRelative("Cancels");
                for (int j = 0; j < c.arraySize; j++)
                {
                    c.GetArrayElementAtIndex(j).intValue = ActionEditorHelper.PopupSelection(c.GetArrayElementAtIndex(j).intValue, Database.GetActionNames());

                }
            }

            if (tempdb.List.Count > 0)
            {
                Preview.Preview(character, tempdb.List, character.FPS);
            }
        

            

            EditorGUILayout.EndScrollView();
            ob.ApplyModifiedProperties();
        }
    }

    
    public class PreviewVars : ScriptableObject
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        public EntireActionPlayback EntireVars = new EntireActionPlayback();

        public void Preview(ActionCharacter actionCharacter, List<ActionSO> actions, int fps)
        {
           
            EntireVars.Combo = ActionSequenceVisualizer.CreateComboSequencePlayback(actionCharacter, actions, fps, EntireVars.AnimatorOnRoot);
            EntireVars.ActionChoices = new string[EntireVars.Combo.Actions.Count];
            for (int i = 0; i < EntireVars.ActionChoices.Length; i++)
            {
                EntireVars.ActionChoices[i] = EntireVars.Combo.Actions[i].ActionName;
            }

            EntireVars.ActionSelection = Mathf.Clamp(EntireVars.ActionSelection, 0, EntireVars.Combo.Actions.Count - 1);

            sb.Clear();
            for (int i = 0; i < EntireVars.ActionChoices.Length; i++)
            {
                sb.Append(EntireVars.ActionChoices[i]);
                if (i < EntireVars.ActionChoices.Length - 1)
                {
                    sb.Append(" + ");
                }
            }
            EditorGUI.indentLevel++;
            GUI.color = Color.green;
            ActionEditorHelper.DrawLabel(sb.ToString(), ActionEditorHelper.GetActionSelectionActionMaker(12, TextAnchor.MiddleLeft));
            GUI.color = Color.white;
            EditorGUI.indentLevel--;

            ActionEditorHelper.DrawLabel("Selected Action: ");
            EditorGUILayout.BeginHorizontal();
            ActionEditorHelper.GetSelectColor();
            EntireVars.ActionSelection = ActionEditorHelper.PopupSelection(EntireVars.ActionSelection, EntireVars.ActionChoices);
            ActionEditorHelper.ResetGUIColor();
            ActionEditorHelper.GetConfirmColor();
            bool edit = ActionEditorHelper.Button("Edit");
            ActionEditorHelper.GetRemoveColor();
            bool remove = ActionEditorHelper.Button("Remove");
            EditorGUILayout.EndHorizontal();

            


            ActionPlayback action = EntireVars.Combo.Actions[EntireVars.ActionSelection];
            EntireVars.SequenceSelection = Mathf.Clamp(EntireVars.SequenceSelection, 0, action.Sequences.Count - 1);
            EntireVars.SequenceChoices = new string[action.Sequences.Count];
            for (int i = 0; i < EntireVars.SequenceChoices.Length; i++)
            {
                EntireVars.SequenceChoices[i] = i.ToString();
            }

            if (EntireVars.Focus == PlaybackFocus.Sequence)
            {
                ActionEditorHelper.DrawLabel("Sequences for " + action.ActionName);
                EntireVars.SequenceSelection = ActionEditorHelper.TabSelection(EntireVars.SequenceSelection, EntireVars.SequenceChoices, 4);
            }

            Sequence selected = action.Sequences[EntireVars.SequenceSelection];

            if (edit)
            {
                PopUpWindowBasic.ShowBasicPopUp(action.ActionRef);
            }

            switch (EntireVars.Focus)
            {
                case PlaybackFocus.Sequence:
                    ActionSequenceVisualizer.MoveSequence(actionCharacter, selected, EntireVars.NormalizedPlayback);
                    break;
                case PlaybackFocus.Action:
                    ActionSequenceVisualizer.MoveAction(actionCharacter, action.Sequences, EntireVars.NormalizedPlayback);
                    break;
                case PlaybackFocus.Combo:
                    ActionSequenceVisualizer.MoveCombo(actionCharacter, EntireVars.Combo.Actions, EntireVars.NormalizedPlayback);
                    break;
            }




            if (remove)
            {
                for (int i = 0; i < actions.Count; i++)
                {
                    string toremove = EntireVars.ActionChoices[EntireVars.ActionSelection];
                    if (CommonFunctions.WordEquals(toremove, actions[i].GetActionName()))
                    {
                        actions.RemoveAt(i);
                    }
                }
            }
        }
    }

    public class VanillaActionList : ScriptableObject
    {
        public List<ActionSO> List = new List<ActionSO>();
        public void Add(ActionSO action)
        {
            if (List.Contains(action) == false)
            {
                List.Add(action);
            }
        }
    }
    public class ActionWindow : ScriptableObject
    {
        Vector3 cachepos;
        int newactionselection;
        ActionCharacter current;
        VanillaActionList tempdb;
        EditorInspectorDraw[] tempdbed = new EditorInspectorDraw[2] { 
            new EditorInspectorDraw(), 
            new EditorInspectorDraw() };

        public PreviewVars Preview;

        private void OnEnable()
        {
            if (tempdb == null)
            {
                tempdb = CreateInstance<VanillaActionList>();
            }
            if (Preview == null)
            {
                Preview = CreateInstance<PreviewVars>();
            }
        }


        private void OnDestroy()
        {
            
            for (int i = 0; i < tempdb.List.Count; i++)
            {
                tempdb.List[i].OnValuesChanged -= DrawUpdate;
                tempdb.List.RemoveAt(i);
            }

            if (current != null)
            {
                
                for (int i = 0; i < current.ActionsDatabase.Database.Actions.Count; i++)
                {
                    current.ActionsDatabase.Database.Actions[i].OnValuesChanged -= DrawUpdate;
                }
                //reset it
                ActionSequenceVisualizer.SetAnimationFrame(this.current.GetComponentInChildren<Animator>(), this.current.Config.Defaults.Locomotion, 0, 0);
                current.GetComponentInChildren<Animator>().transform.localPosition = cachepos;
                current = null;
            }

            DestroyImmediate(tempdb);
            DestroyImmediate(Preview);
        }

        void DrawUpdate()
        {
            Draw(current);
           

        }
        public void Draw(ActionCharacter character)
        {

            if (character == null) return;

            if (this.current == null)
            {
                this.current = character;
                cachepos = this.current.GetComponentInChildren<Animator>().transform.localPosition;
              
            }
           
           

            ActionsDatabaseSO db = character.ActionsDatabase;
            ActionEditorHelper.MediumGUISpace();
            ActionEditorHelper.DrawLabel("Choose Actions to Preview", ActionEditorHelper.GetActionSelectionActionMaker(16, TextAnchor.MiddleLeft));
            string message = "Add ";
            EditorGUILayout.BeginHorizontal();
            ActionEditorHelper.GetSelectColor();
            newactionselection = ActionEditorHelper.PopupSelection(newactionselection, db.GetActionNames());
            ActionEditorHelper.ResetGUIColor();

            string[] current = new string[tempdb.List.Count];
            bool has = false;



                for (int j = 0; j < tempdb.List.Count; j++)
                {
                    if (CommonFunctions.WordEquals(db.GetActionNames()[newactionselection], tempdb.List[j].GetActionName()))
                    {
                        has = true;
                        break;
                    }
                }

            bool confirm = false;
            if (has)
            {
                message = "Remove ";
                ActionEditorHelper.GetRemoveColor();
                confirm = ActionEditorHelper.Button(message + db.GetActionNames()[newactionselection]);
            }
            else
            {
                ActionEditorHelper.GetConfirmColor();
                confirm = ActionEditorHelper.Button(message + db.GetActionNames()[newactionselection]);
            }



            EditorGUILayout.EndHorizontal();

            if (confirm)
            {
                if (has == false)
                {
                    db.Database.Actions[newactionselection].OnValuesChanged -= DrawUpdate;
                    db.Database.Actions[newactionselection].OnValuesChanged += DrawUpdate;
                    tempdb.List.Add(db.Database.Actions[newactionselection]);
                }
                else
                {
                    db.Database.Actions[newactionselection].OnValuesChanged -= DrawUpdate;
                    tempdb.List.Remove(db.Database.Actions[newactionselection]);
                }

            }


            ActionEditorHelper.DrawLabel("Current Actions Preview", ActionEditorHelper.GetActionSelectionActionMaker(16, TextAnchor.MiddleLeft));
            if (tempdb.List.Count > 0)
            {
                for (int i = 0; i < tempdb.List.Count; i++)
                {
                    if (tempdb.List[i] == null)
                    {
                        tempdb.List.RemoveAt(i);
                    }
                }

                if (tempdb.List.Count > 0)
                {
                    Preview.Preview(this.current, tempdb.List, character.FPS);
                    ActionEditorHelper.DrawLabel("Preview Control");
                    ActionEditorHelper.DrawHelpBox("Combo allows you to preview all the actions as if they were a combo. \n" +
                        "Action allows you to preview a single action. \n" +
                        "Sequence allows you to preview a particular sequence within the selected action.");



                }

            }
            else
            {

                this.current.GetComponentInChildren<Animator>().transform.localPosition = cachepos;
                ActionSequenceVisualizer.SetAnimationFrame(this.current.GetComponentInChildren<Animator>(), this.current.Config.Defaults.Locomotion, 0, 0);
                this.current = null;

                //reset character
            }

            tempdbed[1].Draw(Preview);

            //CharacterActionLoadoutSO loadout = null;
            //loadout = EditorGUILayout.ObjectField(loadout, typeof(CharacterActionLoadoutSO), true) as CharacterActionLoadoutSO;
            //  tempdbed[0].Draw(tempdb, true);


        }
    }
}
