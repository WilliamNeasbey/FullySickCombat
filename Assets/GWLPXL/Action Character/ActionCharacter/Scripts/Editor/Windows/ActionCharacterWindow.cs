using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using GWLPXL.ActionCharacter;
using System.Text;

namespace GWLPXL.ActionCharacter.Editor
{
    public static class CharacterActionWindow
    {
        public static void Show(ActionCharacter character)
        {
            ActionCharacter prefab = PrefabUtility.GetCorrespondingObjectFromSource(character);
            // Get existing open window or if none, make a new one:
            ActionCharacterWindow window = (ActionCharacterWindow)EditorWindow.GetWindow(typeof(ActionCharacterWindow));
            if (prefab != null)
            {
                window.Character = prefab;
            }
            else
            {
                window.Character = character;
            }
            window.Database = character.ActionsDatabase;
            window.CharacterLoadout = character.Loadout as CharacterActionLoadoutSO;
            window.InputMap = character.InputRequirements;
            window.TranslatedInputs = window.InputMap != null;
            window.Show();
        }
    }

    public class LoadoutEditorV2 : ScriptableObject
    {
        [System.Serializable]
        public class EditorOptions
        {
            public ActionSO Starter;
            public InputActionSlotSO Input;
            public bool Fold;
            public List<ActionSO> Nexts = new List<ActionSO>();
            public List<ActionSO> Cancels = new List<ActionSO>();
        }
        bool fold;
        string status = "Test";

        private void OnEnable()
        {

        }

        private void OnDisable()
        {

        }



        Dictionary<ActionSet, bool> dic = new Dictionary<ActionSet, bool>();
        public void Draw(ActionCharacterWindow window)
        {
            SerializedObject obj = new SerializedObject(window.CharacterLoadout);
            obj.Update();
            SerializedProperty actionset = obj.FindProperty("ActionSets");
            ActionEditorHelper.AddToDispose(actionset);
            SerializedProperty sets = actionset.FindPropertyRelative("ActionSets");
            ActionEditorHelper.AddToDispose(sets);
            int setscount = sets.arraySize;
            for (int i = 0; i < sets.arraySize; i++)
            {
                SerializedProperty flow = sets.GetArrayElementAtIndex(i).FindPropertyRelative("Flow");
                ActionEditorHelper.AddToDispose(flow);

                SerializedProperty flowref = flow.FindPropertyRelative("References");
                ActionEditorHelper.AddToDispose(flowref);

                SerializedProperty flownexts = flowref.FindPropertyRelative("Nexts");
                ActionEditorHelper.AddToDispose(flownexts);

                SerializedProperty starting = flowref.FindPropertyRelative("Starting");
                ActionEditorHelper.AddToDispose(starting);

                ActionEditorHelper.DrawProperty(starting);
                ActionEditorHelper.DrawProperty(flownexts);
            }

            //for (int i = 0; i < window.PlayerActionSet.ActionSets.ActionSets.Count; i++)
            //{
            //    PlayerActionSet actionset = window.PlayerActionSet.ActionSets.ActionSets[i];

            //    if (dic.ContainsKey(actionset) == false)
            //    {
            //        dic[actionset] = false;

            //    }



            //    dic[actionset] = EditorGUILayout.BeginFoldoutHeaderGroup(dic[actionset], actionset.Action);
            //    //fold = EditorGUILayout.BeginFoldoutHeaderGroup(fold, "Action");
            //    if (dic[actionset])
            //    {
            //        EditorGUI.indentLevel++;
            //        Flow flow = actionset.Flow;
            //        FlowReferences References = flow.References;

            //        References.Starting = EditorGUILayout.ObjectField(References.Starting, typeof(ActionSO), false) as ActionSO;
            //        for (int j = 0; j < References.Nexts.Count; j++)
            //        {
            //            References.Nexts[j] = EditorGUILayout.ObjectField(References.Nexts[j], typeof(ActionSO), false) as ActionSO;
            //        }


            //        EditorGUI.indentLevel--;
            //    }

            //    EditorGUILayout.EndFoldoutHeaderGroup();
            //}

            obj.ApplyModifiedProperties();

            ActionEditorHelper.Dispose();
        }
    }

    public class FlowInputSO : ScriptableObject
    {
        public List<FlowInput> InputMap = new List<FlowInput>();
    }
    [System.Serializable]
    public class FlowInput
    {
        public string Action;
        public int InputSlot;
        public FlowInput(string actioname, int inslot)
        {
            Action = actioname;
            InputSlot = inslot;
        }
    }

    /// <summary>
    /// clean up
    /// </summary>
    public class ActionCharacterWindow : EditorWindow
    {
        public FlowInputSO FlowInputSO;
        public ActionsDatabaseSO Database;
        public CharacterActionLoadoutSO CharacterLoadout;
        public ActionCharacter Character;
        public InputActionMapSO InputMap;
        protected CharacterActionLoadoutSO cache;
        protected ActionsDatabaseSO selectDb;
        public bool TranslatedInputs = false;
        protected string[] header = new string[2] { "Action Creator", "Action Loadout Creator" };

        public SimpleComboSO simplePieces;
        protected LoadoutEditorV2 v2;
        protected ValidSequenceType rtypetemp = ValidSequenceType.None;
        protected ValidSequenceType ntypetemp = ValidSequenceType.None;
        protected List<string> requiredTemp = new List<string>();
        protected List<string> nextTemp = new List<string>();
        protected List<string> earlyexittemp = new List<string>();
        protected bool earlyexits;
        protected string[] inputselections = new string[0];
        protected int inputselected = 0;
        protected int headerselection = 0;
        protected Vector2 mainscroll;
        protected UnityEditor.Editor seteditor;
        protected UnityEditor.Editor actioneditor;
        protected string[] selections = new string[0];
        protected string[] allactions = new string[0];
        protected int creationSelection = 0;
        protected int creationSelectioncache = -1;
        protected int mainselected = 0;
        protected int selected = 0;
        protected int cacheselected = -1;
        protected int maincacheselected = -1;
        protected bool redraw;
        protected bool isPlayer;
        protected string newname;
        protected bool redrawactioncreator;

        protected int actionsetmakerflowselectionsize = 4;
        protected int actionsetmakerselectionsize = 4;
        protected bool changedb = false;
        protected bool useV2 = false;
        protected bool advanced = false;
        protected ValidSequenceType firstsequencetype = 0;
        protected string namedel = "=>";
        [MenuItem("GWLPXL/ActionCharacter/Windows/Action Character Window")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            ActionCharacterWindow window = (ActionCharacterWindow)EditorWindow.GetWindow(typeof(ActionCharacterWindow));
            window.Show();
        }


        bool showcolors = false;
        List<LoadoutPair> missing = new List<LoadoutPair>();


        private void OnDestroy()
        {
            if (v2 != null) DestroyImmediate(v2);
            if (simplePieces != null) DestroyImmediate(simplePieces);
            if (FlowInputSO != null) DestroyImmediate(FlowInputSO);
        }

        SerializedObject simple;
        SerializedProperty simplep;
        EditorInspectorDraw simpleed;
        List<string> messages = new List<string>();
        int select = 0;
        //public FlowInputSO FlowInputSO;
        //public ActionsDatabaseSO Database;
        //public CharacterActionLoadoutSO CharacterLoadout;
        //public ActionCharacter Character;
        //public InputActionMapSO InputMap;
        public override void SaveChanges()
        {
            if (CharacterLoadout != null) EditorUtility.SetDirty(CharacterLoadout);
            if (Database != null) EditorUtility.SetDirty(Database);
            AssetDatabase.SaveAssets();
            base.SaveChanges();
        }
        protected virtual void OnGUI()
        {
            if (FlowInputSO == null)
            {
                FlowInputSO = ScriptableObject.CreateInstance<FlowInputSO>();
            }
            if (v2 == null)
            {
                v2 = ScriptableObject.CreateInstance<LoadoutEditorV2>();
            }
            if (simplePieces == null)
            {
                simplePieces = ScriptableObject.CreateInstance<SimpleComboSO>();


            }

            if (simpleed == null)
            {
                simpleed = ActionEditorHelper.CreateEditor();
            }



            //check preconditions
            Character = EditorGUILayout.ObjectField("Character: ", Character, typeof(ActionCharacter), true) as ActionCharacter;
            if (Character == null)
            {
                creationSelectioncache = -1;
                return;
            }

            Database = EditorGUILayout.ObjectField("Database: ", Database, typeof(ActionsDatabaseSO), false) as ActionsDatabaseSO;


            GUILayout.Space(10);
            headerselection = GUILayout.SelectionGrid(headerselection, header, 2);
            GUILayout.Space(10);


            SerializedObject t = null;

            if (Character.Loadout != null) t = new SerializedObject(Character.Loadout);

            if (t != null)
            {
                t.Update();
            }

            mainscroll = EditorGUILayout.BeginScrollView(mainscroll);
            switch (headerselection)
            {
                case 0:
                    //action creator

                    GUILayout.Space(25);
                    ActionCreator();


                    break;
                case 1://action selection
                    CharacterActionLoadoutSO temp;
                    SerializedObject tempso;
                    bool hasactionset;
                    EditorGUILayout.BeginHorizontal();
          
                    if (GUILayout.Button("Create Entirely New and Empty Action Set"))
                    {
                        string path = EditorUtility.SaveFilePanelInProject("Create New Player Action Set Asset", "New Action Set", "asset", "Create a new Action Set Asset");
                        if (path.Length > 0)
                        {
                            CharacterActionLoadoutSO newset = ScriptableObject.CreateInstance<CharacterActionLoadoutSO>();
                            AssetDatabase.CreateAsset(newset, path);
                            CharacterLoadout = newset;
                            hasactionset = true;

                        }
                    }
                    InputMap = EditorGUILayout.ObjectField("Input Map: ", InputMap, typeof(InputActionMapSO), false) as InputActionMapSO;

                    ActionSetPreconditions(out temp, out hasactionset);
                    if (hasactionset == false)
                    {
                        break;
                    }
                    tempso = new SerializedObject(temp);
                    tempso.Update();
                    for (int i = 0; i < temp.GetActionSets().Count; i++)
                    {
                        string c = temp.GetActionSets()[i].Flow.StartingAction;
                        bool has = false;
                        for (int j = 0; j < FlowInputSO.InputMap.Count; j++)
                        {
                            if (CommonFunctions.WordEquals(c, FlowInputSO.InputMap[j].Action))
                            {
                                has = true;
                                break;
                            }
                        }

                        if (has == false)
                        {
                            FlowInputSO.InputMap.Add(new FlowInput(c, temp.GetActionSets()[i].InputIndex));
                        }
                    }

                    EditorGUILayout.EndHorizontal();
                    //useBuilder = EditorGUILayout.Toggle("Advanced: " , useBuilder);
                    if (advanced == false)
                    {
                        SerializedObject newcombo = new SerializedObject(simplePieces);
                        //okay showing the flow list is an issue, rethink
                        //maybe we have a button 'show corresponding flow data' or smth
                        //also need to verify if action is in database, if not do you want to add it.
                        //  ActionEditorHelper.DrawProperty(simplep);

                        string[] combonames = temp.GetComboNames();
                        System.Array.Resize(ref combonames, combonames.Length + 1);
                        combonames[combonames.Length - 1] = "NEW";
                        select = Mathf.Clamp(select, 0, combonames.Length - 1);
                        select = ActionEditorHelper.PopupSelection(select, combonames);

                        if (CommonFunctions.WordEquals("NEW", combonames[select]))
                        {

                            newcombo.Update();

                            SerializedProperty newcombosp = newcombo.FindProperty(nameof(simplePieces.Pieces));
                            SerializedProperty comboname = newcombo.FindProperty(nameof(simplePieces.Name));

                            ActionEditorHelper.DrawProperty(comboname);
                            ActionEditorHelper.DrawProperty(newcombosp);


                            if (simplePieces.Pieces.Count <= 0)
                            {
                                ActionEditorHelper.DrawLabel("Add a piece to continue");
                            }

                            StringBuilder sb = new StringBuilder();
                            for (int i = 0; i < simplePieces.Pieces.Count; i++)
                            {
                                if (simplePieces.Pieces[i].Action == null)
                                {
                                    sb.Append("NULL");
                                }
                                else
                                {
                                    sb.Append(simplePieces.Pieces[i].Action.GetActionName());
                                }

                                if (i < simplePieces.Pieces.Count - 1)
                                {
                                    sb.Append(namedel);
                                }

                            }
                            simplePieces.Name = sb.ToString();
                            bool confirm = ActionEditorHelper.Button("Save Combo");
                            if (confirm && simplePieces.Pieces.Count > 0)
                            {
                                ComboManager.TryBuildCombo(temp, simplePieces.Name, simplePieces.Pieces);
                                ComboManager.VerifyCombos(temp);
                                EditorUtility.SetDirty(temp);
                                AssetDatabase.SaveAssets();
                                //TryBuildCombo(temp, simplePieces);

                            }

                            newcombo.ApplyModifiedProperties();
                            newcombo.Dispose();
                        }
                        else
                        {
                            List<int> inputs = temp.GetInputSequence(select);
                            ActionEditorHelper.DrawLabel("Input Sequence: ");

                            EditorGUILayout.BeginVertical();
                            EditorGUI.indentLevel++;
                            TranslatedInputs = EditorGUILayout.Toggle("Translate Inputs", TranslatedInputs);
                            EditorGUI.BeginDisabledGroup(true);
                            for (int i = 0; i < inputs.Count; i++)
                            {

                                if (TranslatedInputs)
                                {
                                    if (InputMap != null)
                                    {
                                        if (inputs[i] <= InputMap.InputSlots.Count - 1)
                                        {
                                            InputMap.InputSlots[inputs[i]].Requirements =
                                            EditorGUILayout.ObjectField("Input Slot: ",
                                            InputMap.InputSlots[inputs[i]].Requirements,
                                            typeof(InputActionSlotSO), false) as InputActionSlotSO;
                                        }
                                        else
                                        {
                                            EditorGUILayout.IntField("ERROR. Input Map doesnt have input at ", inputs[i]);
                                        }

                                    }
                                    else
                                    {
                                        EditorGUILayout.IntField(inputs[i]);
                                    }
                                }
                                else
                                {
                                    EditorGUILayout.IntField(inputs[i]);
                                }




                            }
                            EditorGUI.EndDisabledGroup();
                            EditorGUI.indentLevel--;
                            EditorGUILayout.EndVertical();
                            tempso.Update();
                            SerializedProperty tp = tempso.FindProperty("Combos");
                            SerializedProperty tpindex = tp.GetArrayElementAtIndex(select);
                            ActionEditorHelper.DrawProperty(tpindex);

                            bool removeselectedcombo = ActionEditorHelper.Button("Delete " + combonames[select]);
                            if (removeselectedcombo)
                            {
                                temp.Combos.RemoveAt(select);
                                EditorUtility.SetDirty(temp);
                                AssetDatabase.SaveAssets();
                            }


                        }



                        bool clearall = ActionEditorHelper.Button("Clear all");
                        if (clearall)
                        {
                            bool confirm = EditorUtility.DisplayDialog("Confirm?", "This will erase all of your combo data completely.", "Okay", "No way!");
                            if (confirm)
                            {
                                FlowInputSO.InputMap.Clear();
                                simplePieces.Pieces.Clear();
                                temp.Combos.Clear();
                                temp.GetActionSets().Clear();
                            }

                        }

                        tempso.ApplyModifiedProperties();

                        bool reverify = ActionEditorHelper.Button("Verify all");
                        if (reverify)
                        {
                            VerifyTheLoadout(temp);
                            EditorUtility.SetDirty(temp);
                            AssetDatabase.SaveAssets();
                        }

                        EditorGUILayout.BeginHorizontal();
                        if (messages.Count > 0)
                        {
                            ActionEditorHelper.DrawLabel("ERROR");
                        }
                        EditorGUILayout.BeginVertical();
                        for (int i = 0; i < messages.Count; i++)
                        {
                            EditorGUILayout.TextArea(messages[i]);
                        }
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndHorizontal();






                    }
                    else
                    {

                        ActionEditorHelper.DrawHelpBox(
                       "Use Verify to ensure your connections are accurate. It will display a list of actions and any missing actions associated with them. " +
                       "To fix, add the missing actions to the loadout OR remove the missing actions from the next/prerequities of their associated actions.");
                        EditorGUILayout.BeginHorizontal();
                        bool confirm = ActionEditorHelper.Button("Verify");
                        if (confirm)
                        {
                            missing.Clear();
                            missing = VerifyLoadout(CharacterLoadout);

                        }
                        if (missing.Count > 0)
                        {
                            GUI.color = ActionEditorHelper.GetConfirmColor();
                        }
                        confirm = ActionEditorHelper.Button("Clear");
                        GUI.color = Color.white;

                        if (confirm)
                        {
                            missing.Clear();
                        }
                        EditorGUILayout.EndHorizontal();

                        for (int i = 0; i < missing.Count; i++)
                        {
                            EditorGUILayout.BeginHorizontal();
                            GUI.color = Color.green;
                            EditorGUILayout.LabelField("ACTION: " + missing[i].A);
                            GUI.color = Color.red;
                            EditorGUILayout.LabelField("MISSING ACTION: " + missing[i].B);
                            GUI.color = Color.white;
                            //EditorGUILayout.LabelField("NEXT/PREREQUISITE: " + missing[i].Type);
                            Debug.Log("Action: " + missing[i].A + " is missing " + missing[i].B + " of type " + missing[i].Type);
                            EditorGUILayout.EndHorizontal();
                        }

                        ActionSelection(temp, hasactionset);

                    }


                    tempso.ApplyModifiedProperties();
                    tempso.Dispose();
                    break;


            }



            EditorGUILayout.EndScrollView();

            if (t != null)
            {
                t.ApplyModifiedProperties();
            }



        }

        private void VerifyTheLoadout(CharacterActionLoadoutSO temp)
        {
            temp.GetActionSets().Clear();
            List<SimpleComboSO> tempcombos = new List<SimpleComboSO>();
            for (int i = 0; i < temp.Combos.Count; i++)
            {
                SimpleComboSO newinstance = ScriptableObject.CreateInstance<SimpleComboSO>();
                newinstance.Name = temp.Combos[i].Name;
                newinstance.Pieces = new List<SimpleComboPiece>();
                for (int j = 0; j < temp.Combos[i].Pieces.Count; j++)
                {
                    SimpleComboPiece c = temp.Combos[i].Pieces[j];
                    SimpleComboPiece newcopy = new SimpleComboPiece(c.Action, c.InputSlotIndex, c.Cancels, c.Forgiveness);
                    newinstance.Pieces.Add(newcopy);
                }

                tempcombos.Add(newinstance);
            }

            for (int i = 0; i < tempcombos.Count; i++)
            {
                ComboManager.TryBuildCombo(temp, tempcombos[i].Name, tempcombos[i].Pieces);
            }

            ComboManager.VerifyCombos(temp);
            ComboManagerEditor.TryVerifyDatabase(Database, temp);
            var sortedList = temp.Combos.OrderBy(go => go.Name).ToList();
            temp.Combos = sortedList;
            EditorUtility.SetDirty(temp);
            EditorUtility.SetDirty(Database);
            for (int i = 0; i < tempcombos.Count; i++)
            {
                DestroyImmediate(tempcombos[i]);
            }
        }




        CharacterActionLoadoutSO setcache;
        bool redrawset;
        private void ActionSetPreconditions(out CharacterActionLoadoutSO temp, out bool hasactionset)
        {
            List<string> names = new List<string>();
            //Database = Character.ActionsDatabase;

            if (Database != null)
            {
                for (int i = 0; i < Database.Database.Actions.Count; i++)
                {
                    if (Database.Database.Actions[i] == null) continue;
                    names.Add(Database.Database.Actions[i].GetActionName());
                }


            }
            selections = names.ToArray();
            temp = null;
            CharacterLoadout = EditorGUILayout.ObjectField("Character Loadout: ", CharacterLoadout, typeof(CharacterActionLoadoutSO), false) as CharacterActionLoadoutSO;
            temp = CharacterLoadout;

            //if (Character is PlayerCharacter)
            //{
            //    PlayerCharacter player = Character as PlayerCharacter;
            //    isPlayer = true;

            //}
            //else
            //{
            //    Character.Loadout = EditorGUILayout.ObjectField("Character Action Set: ", Character.Loadout, typeof(CharacterActionLoadoutSO), false) as CharacterActionLoadoutSO;
            //    temp = Character.Loadout;
            //}

            if (temp != setcache)
            {
                setcache = temp;
                redrawset = true;
            }

            hasactionset = temp != null;
        }


        private void ActionSelection(CharacterActionLoadoutSO temp, bool hasactionset)
        {

            if (mainselected != maincacheselected)
            {

                if (unsavedchanges)
                {
                    bool save = EditorUtility.DisplayDialog("Changes not saved", "Apply unsaved changes to " + selections[maincacheselected] + "?", "Yes, apply unsaved changes!", "No thanks.");
                    if (save)
                    {
                        TryAddOrUpdatePlayerAction();
                    }
                }
                maincacheselected = mainselected;
                unsavedchanges = false;
                redraw = true;
                GUI.FocusControl(null);
            }

            if (temp == null)
            {
                return;
            }


            GUILayout.Space(25);
            if (hasactionset)
            {

                mainselected = Mathf.Clamp(mainselected, 0, selections.Length - 1);

                string name = temp.name;
                if (useV2)
                {
                    v2.Draw(this);
                }


                ActionEditorHelper.DrawLabel("Select Action: ", ActionEditorHelper.GetActionSelectionActionMaker(22, TextAnchor.MiddleCenter));
                ActionEditorHelper.DrawHelpBox("Select the action for " + name + " action set." +
                    '\n' + " Click UPDATE to save changes.");
                ActionEditorHelper.TinyGUISpace();

                EditorGUILayout.BeginHorizontal();
                Texture2D action = ActionEditorHelper.LoadImage("action.png");
                GUILayout.Label(action);

                if (selections.Length <= 0)
                {
                    ActionEditorHelper.DrawHelpBox("Loadout doesnt have any corresponding actions to the Database.");
                    return;
                }
                mainselected = EditorGUILayout.Popup(mainselected, selections);
                string message = "Add " + selections[mainselected];
                bool alreadyhas = false;
                //check if already in the loadout
                for (int i = 0; i < temp.GetActionSets().Count; i++)
                {
                    string current = temp.GetActionSets()[i].Action;
                    if (CommonFunctions.WordEquals(current, selections[mainselected]))
                    {
                        alreadyhas = true;
                    }
                }

                if (alreadyhas)
                {
                    message = "Update " + selections[mainselected];
                }

                if (unsavedchanges)
                {
                    ActionEditorHelper.MediumGUISpace();
                    Texture2D update = ActionEditorHelper.LoadImage("update.png");
                    GUILayout.Label(update);
                    GUI.color = ActionEditorHelper.GetConfirmColor();
                }
                bool confirm = ActionEditorHelper.Button(message);
                GUI.color = Color.white;
                if (confirm)
                {
                    TryAddOrUpdatePlayerAction();//add or update

                }
                message = "Reset";
                if (unsavedchanges)
                {
                    ActionEditorHelper.MediumGUISpace();
                    Texture2D undo = ActionEditorHelper.LoadImage("undo.png");
                    GUILayout.Label(undo);
                    GUI.color = ActionEditorHelper.GetConfirmColor();
                }
                confirm = ActionEditorHelper.Button(message);
                GUI.color = Color.white;
                if (confirm)
                {
                    RedrawActionCombo();
                }



                if (alreadyhas)
                {
                    ActionEditorHelper.MediumGUISpace();
                    Texture2D remove = ActionEditorHelper.LoadImage("remove.png");
                    GUILayout.Label(remove);
                    message = "Remove " + selections[mainselected] + " from Action Set";
                    GUI.color = ActionEditorHelper.GetRemoveColor();
                    confirm = ActionEditorHelper.Button(message);
                    GUI.color = Color.white;
                    if (confirm)
                    {
                        TryRemovePlayerAction();
                    }
                }

                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();

                if (alreadyhas)
                {
                    DrawFflowOptions();
                }
                else
                {
                    ActionEditorHelper.DrawHelpBox("Add " + selections[mainselected] + " in order to modify it's combo properties.");
                }


                redrawset = false;
            }



        }


        bool hasactions = false;

        private void ActionCreator()
        {
            List<string> actions = new List<string>();
            List<ActionSO> actionSOList = Database.Database.Actions;
            hasactions = actionSOList.Count > 0;
            bool createnew = false;

            if (hasactions)
            {

                for (int i = 0; i < actionSOList.Count; i++)
                {
                    actions.Add(actionSOList[i].GetActionName());
                }
                allactions = actions.ToArray();
                Texture2D actionimage = ActionEditorHelper.LoadImage("action.png");
                Texture2D editimage = ActionEditorHelper.LoadImage("edit.png");


                EditorGUILayout.LabelField("Action Selection", ActionEditorHelper.GetActionsInDatabase(22, TextAnchor.MiddleLeft));
                ActionEditorHelper.DrawHelpBox("Select the Action you wish to modify. Click Edit to modify the selected action.");
                ActionEditorHelper.SmallGUISpace();
                EditorGUILayout.BeginHorizontal();

                GUILayout.Label(actionimage);

                creationSelection = EditorGUILayout.Popup(creationSelection, allactions);//, ActionEditorHelper.GetActionSelectionActionMaker(12, TextAnchor.MiddleLeft));

                //creationSelection = GUILayout.SelectionGrid(creationSelection, allactions, 4);
                GUILayout.Label(editimage);
                GUI.color = ActionEditorHelper.GetConfirmColor();
                bool edit = ActionEditorHelper.Button("Edit " + allactions[creationSelection]);
                GUI.color = Color.white;
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.ObjectField("Selected Asset: ", actionSOList[creationSelection], typeof(CharacterActionsSO), false);
                if (edit)
                {
                    EditorUtility.SetDirty(actionSOList[creationSelection]);
                    PopUpWindowBasic.ShowBasicPopUp(actionSOList[creationSelection]);
                }
                ActionEditorHelper.LargeGUISpace();
                createnew = DrawCreateAction();
                ActionEditorHelper.MediumGUISpace();
                ActionEditorHelper.DrawLabel("Other", ActionEditorHelper.GetActionSelectionActionMaker(16, TextAnchor.MiddleLeft));
                ActionEditorHelper.DrawHelpBox("Add an existing action asset to this database. Remove an action from the database. Removal will not delete the asset unless that option is chosen.");

                DrawAddExistingAction();

                ActionSO current = actionSOList[creationSelection];
                bool removeSelection = ActionEditorHelper.Button("Remove Selection " + current.GetActionName() + " from database.");
                if (removeSelection)
                {

                    Database.Database.Actions.Remove(current);
                    EditorUtility.SetDirty(Database);
                    creationSelection = 0;

                    bool alsodelete = EditorUtility.DisplayDialog("Delete asset?", "Also delete the asset entirely?", "Yes, delete", "No, just remove it from the database.");
                    if (alsodelete)
                    {
                        string path = AssetDatabase.GetAssetPath(current);
                        AssetDatabase.DeleteAsset(path);
                    }
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                }
                if (creationSelection != creationSelectioncache)
                {
                    creationSelectioncache = creationSelection;
                    redrawactioncreator = true;
                }

                createnew = DrawCreateNewActionDatabase();

                EditorGUILayout.BeginHorizontal();
                changedb = EditorGUILayout.Toggle("Change Action Database: ", changedb);
                ActionEditorHelper.DrawHelpBox("Toggle to choose a different action database object.");
                EditorGUILayout.EndHorizontal();
                if (changedb)
                {
                    ActionEditorHelper.DrawHelpBox("Insert a new database object. This will switch databases. It does not delete the previous database.");
                    selectDb = EditorGUILayout.ObjectField("New Database: ", selectDb, typeof(ActionsDatabaseSO), false) as ActionsDatabaseSO;
                    if (selectDb != null)
                    {
                        Character.ActionsDatabase = selectDb;
                        selectDb = null;
                        changedb = false;
                    }
                }
                ActionEditorHelper.MediumGUISpace();

            }
            else
            {
                //no actions
                createnew = DrawCreateAction();
                DrawAddExistingAction();
                createnew = DrawCreateNewActionDatabase();
            }
            EditorGUI.indentLevel++;


            if (createnew)
            {
                if (string.IsNullOrEmpty(newname))
                {
                    EditorUtility.DisplayDialog("New Action Error", "New Action Needs a name to Create", "Fine, I'll give it a name");
                    return;
                }

                for (int i = 0; i < actions.Count; i++)
                {
                    if (CommonFunctions.WordEquals(actions[i], newname))
                    {
                        EditorUtility.DisplayDialog("New Action Error", "New Action must have a UNIQUE name. An action with " + newname + " already exists.", "Fine, I'll give it a UNIQUE name");
                        return;
                    }
                }

                string path = EditorUtility.SaveFilePanelInProject("New Action", newname, "asset", "Where to save the new action?");
                if (path.Length > 0)
                {
                    CharacterActionsSO newtemp = ScriptableObject.CreateInstance<CharacterActionsSO>();
                    newtemp.AutoName = true;

                    newtemp.Movement.Movements.ScriptedMovement.Name = newname;
                    Database.Database.Actions.Add(newtemp);
                    AssetDatabase.CreateAsset(newtemp, path);
                    newname = string.Empty;
                    creationSelection = Database.Database.Actions.Count - 1;
                    EditorUtility.SetDirty(newtemp);
                    EditorUtility.SetDirty(Database);
                }
            }

            EditorGUI.indentLevel--;







        }

        private bool DrawCreateNewActionDatabase()
        {
            bool createnew = GUILayout.Button("Create new action database");
            if (createnew)
            {
                string path = EditorUtility.SaveFilePanelInProject("New Action Database", "NewDatabase", "asset", "Where and what to name the new action database");
                if (path.Length > 0)
                {
                    ActionsDatabaseSO newdb = ScriptableObject.CreateInstance<ActionsDatabaseSO>();
                    AssetDatabase.CreateAsset(newdb, path);
                    Character.ActionsDatabase = newdb;
                    mainselected = 0;
                    EditorUtility.SetDirty(Character);
                    EditorUtility.SetDirty(newdb);
                }

            }

            return createnew;
        }

        private void DrawAddExistingAction()
        {
            bool addexisting = ActionEditorHelper.Button("Add Existing Action Asset to Database");
            if (addexisting)
            {
                string path = EditorUtility.OpenFilePanel("Existing Action", "Assets", "asset");
                if (path.Length > 0)
                {
                    path = path.Replace(Application.dataPath, "Assets");
                    CharacterActionsSO asset = AssetDatabase.LoadAssetAtPath(path, typeof(CharacterActionsSO)) as CharacterActionsSO;
                    if (asset != null)
                    {
                        if (Database.Database.Actions.Contains(asset) == false)
                        {
                            Database.Database.Actions.Add(asset);
                            creationSelection = Database.Database.Actions.Count - 1;
                            EditorUtility.SetDirty(Database);
                            AssetDatabase.SaveAssets();
                            AssetDatabase.Refresh();
                        }

                    }

                }
            }
        }

        private bool DrawCreateAction()
        {
            Texture2D createimage = ActionEditorHelper.LoadImage("create.png");
            bool createnew;
            EditorGUILayout.LabelField("Action Creation", ActionEditorHelper.GetActionsInDatabase(22, TextAnchor.MiddleLeft));
            ActionEditorHelper.DrawHelpBox("Enter a name for the new action and create new. The action will be saved and added to the current database.");
            EditorGUILayout.BeginHorizontal();

            newname = EditorGUILayout.TextField(newname);

            GUILayout.Label(createimage);
            GUI.color = ActionEditorHelper.GetConfirmColor();
            createnew = ActionEditorHelper.Button("Create New " + newname);
            GUI.color = Color.white;
            GUILayout.FlexibleSpace();
            if (createnew)
            {
                string path = EditorUtility.SaveFilePanelInProject("New Action ", newname, "asset", "Where?");
                if (path.Length > 0)
                {
                    CharacterActionsSO newdb = ScriptableObject.CreateInstance<CharacterActionsSO>();
                    newdb.SetActionName(newname);
                    Database.Database.Actions.Add(newdb);
                    AssetDatabase.CreateAsset(newdb, path);
                    mainselected = 0;
                    EditorUtility.SetDirty(newdb);
                    EditorUtility.SetDirty(Character);
                    EditorUtility.SetDirty(Database);
                }
            }
            EditorGUILayout.EndHorizontal();
            return createnew;
        }

        bool unsavedchanges = false;
        private void DrawFflowOptions()
        {
            //resetting
            if (redraw)
            {

                redraw = false;
                GUI.FocusControl(null);

                RedrawActionCombo();

            }

            ActionEditorHelper.MediumGUISpace();
            ActionEditorHelper.DrawLabel("Flow Options", ActionEditorHelper.GetActionSelectionActionMaker(ActionEditorHelper.LargeFont, TextAnchor.MiddleLeft));

            ActionEditorHelper.DrawHelpBox(
                "Which actions are prequisites to " + selections[mainselected]
                + "? \n Which actions can follow " + selections[mainselected]
                + "? \n Which actions can cancel " + selections[mainselected] + "?");


            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            Texture2D flow = ActionEditorHelper.LoadImage("flow.png");
            GUILayout.Label(flow);
            ActionEditorHelper.DrawLabel("Pre-Requisites", ActionEditorHelper.GetActionSelectionActionMaker(ActionEditorHelper.MediumFont, TextAnchor.MiddleLeft));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            int current = requiredTemp.Count;
            ValidSequenceType type = rtypetemp;
            EditorGUILayout.BeginHorizontal();

            GUI.color = ActionEditorHelper.GetSelectColor();
            EditorGUI.indentLevel++;
            rtypetemp = (ValidSequenceType)EditorGUILayout.EnumPopup("Required Type: ", rtypetemp);
            EditorGUI.indentLevel--;
            EditorGUILayout.EndHorizontal();
            ActionEditorHelper.ResetGUIColor();
            if (rtypetemp == ValidSequenceType.SelectedOnly || rtypetemp == ValidSequenceType.SelectedANDNone)
            {

                FormatSelection(requiredTemp);

            }
            else
            {
                requiredTemp.Clear();
            }
            if (current != requiredTemp.Count || type != rtypetemp)
            {
                //something changed
                unsavedchanges = true;
            }
            EditorGUI.indentLevel--;

            ActionEditorHelper.MediumGUISpace();
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(flow);
            ActionEditorHelper.DrawLabel("Possible Next Actions", ActionEditorHelper.GetActionSelectionActionMaker(ActionEditorHelper.MediumFont, TextAnchor.MiddleLeft));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            current = nextTemp.Count;
            type = ntypetemp;
            GUI.color = ActionEditorHelper.GetSelectColor();
            EditorGUI.indentLevel++;
            ntypetemp = (ValidSequenceType)EditorGUILayout.EnumPopup("Next Type: ", ntypetemp);
            EditorGUI.indentLevel--;
            ActionEditorHelper.ResetGUIColor();
            if (ntypetemp == ValidSequenceType.SelectedOnly)
            {
                FormatSelection(nextTemp);
            }
            else
            {
                nextTemp.Clear();
            }
            if (current != nextTemp.Count || type != ntypetemp)
            {
                unsavedchanges = true;
            }
            EditorGUI.indentLevel--;

            ActionEditorHelper.MediumGUISpace();

            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            Texture2D exitimage = ActionEditorHelper.LoadImage("exit.png");
            GUILayout.Label(exitimage);
            ActionEditorHelper.DrawLabel("Possible Cancels", ActionEditorHelper.GetActionSelectionActionMaker(ActionEditorHelper.MediumFont, TextAnchor.MiddleLeft));
            GUILayout.FlexibleSpace();
            EditorGUI.indentLevel--;
            EditorGUILayout.EndHorizontal();

            bool exit = earlyexits;

            GUI.color = ActionEditorHelper.GetSelectColor();
            earlyexits = EditorGUILayout.Toggle("Cancels: ", earlyexits);
            ActionEditorHelper.ResetGUIColor();
            EditorGUI.indentLevel--;
            current = earlyexittemp.Count;
            if (earlyexits)
            {
                FormatSelection(earlyexittemp);

            }
            else
            {
                earlyexittemp.Clear();
            }
            if (current != earlyexittemp.Count || exit != earlyexits)
            {
                unsavedchanges = true;
            }
            EditorGUI.indentLevel++;//hmmmmm, indent levels are funky somwehwere




            //input options
            if (isPlayer)
            {
                ActionEditorHelper.LargeGUISpace();
                PlayerCharacter player = Character as PlayerCharacter;


                ActionEditorHelper.DrawLabel("Required Input: ", ActionEditorHelper.GetActionSelectionActionMaker(ActionEditorHelper.LargeFont, TextAnchor.MiddleLeft));

                List<string> slotnames = new List<string>();
                List<InputSlot> slot = player.InputRequirements.InputSlots;
                for (int i = 0; i < slot.Count; i++)
                {
                    slotnames.Add(slot[i].Requirements.name);
                }
                inputselections = slotnames.ToArray();
                ActionEditorHelper.DrawHelpBox("Which Input is required for " + selections[mainselected] + "?");

                EditorGUILayout.BeginHorizontal();
                Texture2D input = ActionEditorHelper.LoadImage("input.png");
                GUILayout.Label(input);
                GUI.color = ActionEditorHelper.GetSelectColor();

                inputselected = EditorGUILayout.Popup(inputselected, inputselections);
                ActionEditorHelper.ResetGUIColor();
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();

                if (inputselected != inputselectionscache)
                {
                    unsavedchanges = true;
                    inputselectionscache = inputselected;
                }
                ActionEditorHelper.ResetGUIColor();
                ActionEditorHelper.LargeGUISpace();



            }
        }

        int inputselectionscache = 0;

        private void RedrawActionCombo()
        {
            Flow flow = null;
            int input = 0;
            if (isPlayer)
            {
                for (int i = 0; i < CharacterLoadout.ActionSets.Count; i++)
                {
                    string c = CharacterLoadout.ActionSets[i].Action;
                    if (CommonFunctions.WordEquals(c, selections[mainselected]))
                    {
                        ///found it
                        flow = CharacterLoadout.ActionSets[i].Flow;
                        input = CharacterLoadout.ActionSets[i].InputIndex;
                        break;
                    }
                }
            }


            if (flow != null)
            {

                rtypetemp = flow.RequiredType;
                ntypetemp = flow.NextType;
                earlyexittemp = flow.EarlyExits.ToList();
                nextTemp = flow.Nexts.ToList();
                requiredTemp = flow.RequiredActionStates.ToList();
                earlyexits = flow.EarlyExits.Count > 0;
                PlayerCharacter player = Character as PlayerCharacter;
                List<InputSlot> slot = player.InputRequirements.InputSlots;
                //for (int i = 0; i < slot.Count; i++)
                //{
                //    if (input.InputRequirements == slot[i].Requirements.InputRequirements)
                //    {
                //        inputselected = i;
                //        inputselectionscache = i;
                //        break;
                //    }

                //}
            }
            unsavedchanges = false;
        }

        /// <summary>
        /// actions
        /// </summary>
        /// <param name="choices"></param>
        private void FormatSelection(List<string> choices)
        {

            EditorGUILayout.BeginHorizontal();
            ActionEditorHelper.SmallGUISpace();
            EditorGUI.indentLevel++;
            ActionEditorHelper.DrawLabel("To add/remove: ");
            Texture2D action = ActionEditorHelper.LoadImage("action.png");
            GUILayout.Label(action);
            selected = EditorGUILayout.Popup(selected, selections);

            RequiredStates(selections[selected], choices);

            EditorGUI.indentLevel--;
            EditorGUILayout.EndHorizontal();

        }

        private void TryRemovePlayerAction()
        {
            if (isPlayer)
            {
                PlayerCharacter player = Character as PlayerCharacter;
                List<InputSlot> slot = player.InputRequirements.InputSlots;
                string selectedname = selections[mainselected];
                for (int i = 0; i < CharacterLoadout.ActionSets.Count; i++)
                {
                    if (CommonFunctions.WordEquals(selectedname, CharacterLoadout.ActionSets[i].Action))
                    {
                        CharacterLoadout.ActionSets.RemoveAt(i);

                    }
                }
            }
        }
        private void TryAddOrUpdatePlayerAction()
        {
            if (isPlayer)
            {
                PlayerCharacter player = Character as PlayerCharacter;
                List<InputSlot> slot = player.InputRequirements.InputSlots;
                string name = selections[maincacheselected];
                bool create = true;
                int modifyset = -1;
                for (int i = 0; i < CharacterLoadout.ActionSets.Count; i++)
                {
                    if (CommonFunctions.WordEquals(name, CharacterLoadout.ActionSets[i].Action))
                    {
                        //right here, ask to overwrite.
                        bool modify = EditorUtility.DisplayDialog("Overwrite?", "Overwrite the existing " + name, "Yeah, overwrite", "No");
                        if (modify)
                        {
                            modifyset = i;
                            break;
                        }

                    }
                }


                if (create)
                {
                    // SerializedObject obj = new SerializedObject(PlayerActionSet);
                    if (modifyset < 0)
                    {
                        ntypetemp = ValidSequenceType.None;
                        rtypetemp = ValidSequenceType.None;
                        earlyexits = false;
                    }
                    if (ntypetemp != ValidSequenceType.SelectedOnly)
                    {
                        nextTemp.Clear();
                    }

                    if (rtypetemp != ValidSequenceType.SelectedOnly)
                    {
                        requiredTemp.Clear();
                    }
                    if (earlyexits == false)
                    {
                        earlyexittemp.Clear();
                    }
                    Flow flow = new Flow(name, nextTemp, requiredTemp, earlyexittemp);
                    InputActionSlotSO input = slot[inputselected].Requirements;
                    PlayerActionSet newset = new PlayerActionSet(name, flow, input.Key);

                    //  obj.Update();
                    if (modifyset < 0)
                    {
                        CharacterLoadout.ActionSets.Add(newset);
                    }
                    else
                    {

                        CharacterLoadout.ActionSets[modifyset] = new PlayerActionSet(name, flow, input.Key);


                    }


                    EditorUtility.SetDirty(CharacterLoadout);

                }
            }

            unsavedchanges = false;
        }

        ActionSet FindSet(string name, CharacterActionLoadoutSO set)
        {
            for (int i = 0; i < set.ActionSets.Count; i++)
            {
                if (CommonFunctions.WordEquals(name, set.ActionSets[i].Action))
                {
                    return set.ActionSets[i];
                }
            }
            return null;
        }

        bool hasName(string name, string[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (CommonFunctions.WordEquals(name, array[i]))
                {
                    return true;
                }
            }
            return false;
        }

        void BuildChain(CharacterActionLoadoutSO loadout)
        {
            List<string> starts = new List<string>();
            for (int i = 0; i < loadout.ActionSets.Count; i++)
            {
                ActionSet set = loadout.ActionSets[i];
                if (set.Flow.RequiredType == ValidSequenceType.None)
                {
                    //build it
                    starts.Add(set.Action);

                }
            }

            for (int i = 0; i < starts.Count; i++)
            {
                List<string> nexts = FindAllWithNext(starts[i], loadout);
            }
        }

        List<string> FindAllWithNext(string next, CharacterActionLoadoutSO loadout)
        {
            List<string> allwithnext = new List<string>();
            for (int i = 0; i < loadout.ActionSets.Count; i++)
            {
                ActionSet set = loadout.ActionSets[i];
                bool has = hasName(next, set.Flow.Nexts.ToArray());
                if (has)
                {
                    allwithnext.Add(set.Action);
                }

            }
            return allwithnext;
        }

        class LoadoutPair
        {
            public string A;
            public string B;
            public string Type;
            public LoadoutPair(string type, string action, string second)
            {
                Type = type;
                A = action;
                B = second;
            }
        }

        /// <summary>
        /// to do, add in selectedANDNone
        /// </summary>
        /// <param name="loadout"></param>
        /// <returns></returns>
        List<LoadoutPair> VerifyLoadout(CharacterActionLoadoutSO loadout)
        {
            List<LoadoutPair> missing = new List<LoadoutPair>();
            for (int i = 0; i < loadout.ActionSets.Count; i++)
            {
                ActionSet set = loadout.ActionSets[i];

                //required -> next verification
                string[] required = set.Flow.RequiredActionStates.ToArray();
                for (int j = 0; j < required.Length; j++)
                {
                    string currentrequired = required[j];

                    ActionSet a = FindSet(currentrequired, loadout);
                    Flow aflow = a.Flow;
                    if (a != null || aflow.NextType == ValidSequenceType.SelectedOnly)
                    {

                        if (CommonFunctions.WordEquals(a.Action, currentrequired) == false && hasName(currentrequired, aflow.Nexts.ToArray()) == false)
                        {
                            aflow.AddNext(currentrequired);


                        }
                        else if (CommonFunctions.WordEquals(a.Action, currentrequired) == true && hasName(currentrequired, aflow.Nexts.ToArray()) == true)//if action and required and already on nexts, remove it
                        {
                            //remove it
                            List<string> nextlist = aflow.Nexts.ToList();
                            for (int k = 0; k < nextlist.Count; k++)
                            {
                                if (CommonFunctions.WordEquals(nextlist[k], currentrequired))
                                {
                                    nextlist.RemoveAt(k);
                                }
                            }
                            aflow.Nexts = nextlist;

                        }

                    }
                    else if (a == null)
                    {
                        //not valid
                        missing.Add(new LoadoutPair("Required", set.Action, currentrequired));
                    }
                }

                //next -> required verification
                string[] nexts = set.Flow.Nexts.ToArray();
                for (int j = 0; j < nexts.Length; j++)
                {
                    string currentnext = nexts[j];
                    ActionSet a = FindSet(currentnext, loadout);
                    if (a != null && a.Flow.NextType == ValidSequenceType.SelectedOnly)
                    {
                        Flow aflow = a.Flow;
                        if (CommonFunctions.WordEquals(a.Action, currentnext) == false && hasName(currentnext, aflow.Nexts.ToArray()) == false)
                        {
                            aflow.AddNext(currentnext);

                        }
                        else if (CommonFunctions.WordEquals(a.Action, currentnext) == true && hasName(currentnext, aflow.Nexts.ToArray()) == true)//if action and required and already on nexts, remove it
                        {
                            //remove it
                            List<string> nextlist = aflow.Nexts.ToList();
                            for (int k = 0; k < nextlist.Count; k++)
                            {
                                if (CommonFunctions.WordEquals(nextlist[k], currentnext))
                                {
                                    nextlist.RemoveAt(k);
                                }
                            }
                            aflow.Nexts = nextlist;

                        }
                    }

                    else if (a == null)
                    {
                        //not valid
                        missing.Add(new LoadoutPair("Next", set.Action, currentnext));
                    }
                }

            }

            return missing;
        }


        bool canremove = false;
        bool canadd = false;
        private void RequiredStates(string text, List<string> temp)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.BeginVertical();
            for (int i = 0; i < temp.Count; i++)
            {
                EditorGUILayout.TextArea(temp[i]);
            }
            EditorGUILayout.EndVertical();
            EditorGUI.EndDisabledGroup();

            if (temp.Contains(text))
            {
                canremove = true;
                canadd = false;
            }
            else
            {
                canremove = false;
                canadd = true;
            }

            bool confirm = false;
            string msg = string.Empty;
            if (canadd)
            {
                msg = "Add ";
                ActionEditorHelper.MediumGUISpace();
                Texture2D add = ActionEditorHelper.LoadImage("add.png");
                GUILayout.Label(add);

                GUI.color = ActionEditorHelper.GetConfirmColor();
                confirm = ActionEditorHelper.Button(msg + text);
                if (confirm)
                {
                    temp.Add(text);
                }

            }
            if (canremove)
            {
                msg = "Remove ";
                ActionEditorHelper.MediumGUISpace();
                Texture2D remove = ActionEditorHelper.LoadImage("remove.png");
                GUILayout.Label(remove);

                GUI.color = ActionEditorHelper.GetRemoveColor();
                confirm = ActionEditorHelper.Button(msg + text);
                if (confirm)
                {
                    temp.Remove(text);
                }

            }


            if (temp.Count > 0)
            {
                GUILayout.Space(ActionEditorHelper.tinySpace);
                if (GUILayout.Button("Clear"))
                {
                    temp.Clear();
                }
            }


            GUILayout.Space(25);
        }



    }
}