
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace GWLPXL.ActionCharacter.Editor
{
    /// <summary>
    /// draws custom editor for the actor hit boxes
    /// custom features are tied to action character config at the moment.
    /// </summary>
    [CustomEditor(typeof(ActorHitBoxes))]
    public class ActorHitBoxesEditor : UnityEditor.Editor
    {
        string[] boxtype = new string[2] { "Hit Givers", "Hit Takers" };
        int box;
        string[] tabsgivers = new string[6] { "Original", "Unity Physics Events", "Hit Boxes", "Group HitBox Enabler", "Hit Sender", "Default Hit Boxes" };
        string[] tabstakers = new string[6] { "Original", "Unity Physics Events", "Hit Boxes", "Group HitBox Enabler", "Hit Receiver", "Default Hit Boxes" };
        int tabgiver;
        int tabtaker;

        SerializedProperty hitboxes;
        SerializedProperty giversbase;
        SerializedProperty hitgivers;
        SerializedProperty giversPhysEvents;
        SerializedProperty giversHitBoxes;
        SerializedProperty giversdynobox;
        SerializedProperty giverssender;
        SerializedProperty giversDefaults;


        SerializedProperty hittakers;
        SerializedProperty takersbase;
        SerializedProperty takersPhysEvents;
        SerializedProperty takersHitBoxes;
        SerializedProperty takersdyno;
        SerializedProperty takersReceiver;
        SerializedProperty takersDefaults;

        bool usingactioncharacter = false;
        bool usinghitboxes = false;
        private void OnEnable()
        {


          
        }

        void DrawProperty(SerializedProperty prop)
        {
            ActionEditorHelper.DrawProperty(prop);

        }
        public override void OnInspectorGUI()
        {
            hitboxes = serializedObject.FindProperty("HitBoxes");
            ActionEditorHelper.AddToDispose(hitboxes);

            hittakers = hitboxes.FindPropertyRelative("HitTakers");
            ActionEditorHelper.AddToDispose(hittakers);

            takersReceiver = hittakers.FindPropertyRelative("HitReceiver");
            ActionEditorHelper.AddToDispose(takersReceiver);

            takersbase = hittakers.FindPropertyRelative("HitBoxesBase");
            ActionEditorHelper.AddToDispose(takersbase);

            takersPhysEvents = takersbase.FindPropertyRelative("UnityPhysicsEvents");
            ActionEditorHelper.AddToDispose(takersPhysEvents);

            takersHitBoxes = takersbase.FindPropertyRelative("HitBoxes");
            ActionEditorHelper.AddToDispose(takersHitBoxes);
            takersdyno = takersbase.FindPropertyRelative("GroupHitboxEnabler");
            ActionEditorHelper.AddToDispose(takersdyno);
            takersDefaults = takersbase.FindPropertyRelative("DefaultHitBoxes");
            ActionEditorHelper.AddToDispose(takersDefaults);

            hitgivers = hitboxes.FindPropertyRelative("HitGivers");
            ActionEditorHelper.AddToDispose(hitgivers);
            giverssender = hitgivers.FindPropertyRelative("HitSender");
            ActionEditorHelper.AddToDispose(giverssender);
            giversbase = hitgivers.FindPropertyRelative("HitBoxesBase");
            ActionEditorHelper.AddToDispose(giversbase);
            giversPhysEvents = giversbase.FindPropertyRelative("UnityPhysicsEvents");
            ActionEditorHelper.AddToDispose(giversPhysEvents);
            giversHitBoxes = giversbase.FindPropertyRelative("HitBoxes");
            ActionEditorHelper.AddToDispose(giversHitBoxes);
            giversdynobox = giversbase.FindPropertyRelative("GroupHitboxEnabler");
            ActionEditorHelper.AddToDispose(giversdynobox);

            giversDefaults = giversbase.FindPropertyRelative("DefaultHitBoxes");
            ActionEditorHelper.AddToDispose(giversDefaults);

            serializedObject.Update();

            ActorHitBoxes hit = (ActorHitBoxes)target;
            ActionCharacter actioncharacter = hit.GetComponent<ActionCharacter>();
            IHaveHitBoxes instance = hit.GetComponent<IHaveHitBoxes>();
            if (instance != null)
            {
                usinghitboxes = true;
                List<string> takers = instance.GetHitTakers();
                takers.Clear();
                for (int i = 0; i < hit.HitBoxes.HitTakers.HitBoxesBase.HitBoxes.Count; i++)
                {
                    string name = hit.HitBoxes.HitTakers.HitBoxesBase.HitBoxes[i].Name;
                    if (takers.Contains(name) == false)
                    {
                        takers.Add(name);
                    }
                }
                instance.SetHitTakers(takers);

                List<string> givers = instance.GetHitGivers();
                givers.Clear();
                for (int i = 0; i < hit.HitBoxes.HitGivers.HitBoxesBase.HitBoxes.Count; i++)
                {
                    string name = hit.HitBoxes.HitGivers.HitBoxesBase.HitBoxes[i].Name;
                    if (givers.Contains(name) == false)
                    {
                        givers.Add(name);
                    }
                }
                instance.SetHitGivers(givers);

            }
            else
            {
                usinghitboxes = false;
            }

            if (actioncharacter != null)
            {
                usingactioncharacter = true;
                actioncharacter.Config.Actions.Clear();
                List<ActionSO> actions = ActionManager.GetActionSOs(actioncharacter.Actions.Keys.ToList(), actioncharacter.ActionsDatabase.Database);
                for (int i = 0; i < actions.Count; i++)
                {
                    string name = actions[i].GetActionName();
                    if (actioncharacter.Config.Actions.Contains(name) == false)
                    {
                        actioncharacter.Config.Actions.Add(name);
                    }
                }
            }
            else
            {
                usingactioncharacter = false;
            }





            GUILayout.Label("Box Type", EditorStyles.boldLabel);

            box = GUILayout.SelectionGrid(box, boxtype, 2);
            GUILayout.Space(10);
            GUILayout.Label("Options", EditorStyles.boldLabel);
          
            switch (box)
            {
                case 0:
                    EditorGUILayout.HelpBox("Givers deal hits, like swords or fists", MessageType.Info);
                    tabgiver = GUILayout.SelectionGrid(tabgiver, tabsgivers, 4);
                    GUILayout.Space(10);
                    DrawGivers(instance, hit);
                    break;
                case 1:
                    EditorGUILayout.HelpBox("Takers receive hits, like bodies and heads", MessageType.Info);
                    tabtaker = GUILayout.SelectionGrid(tabtaker, tabstakers, 4);
                    GUILayout.Space(10);
                    DrawTakers(instance, hit);
                    break;
            }


       

            serializedObject.ApplyModifiedProperties();

            ActionEditorHelper.Dispose();
        }

        void HelpMessage(string message)
        {
            EditorGUILayout.HelpBox(message, MessageType.Info);
        }

        int takerdefaultsection = 0;
        string[] empty = new string[0];
        private void DrawTakers(IHaveHitBoxes instance, ActorHitBoxes target)
        {
            switch (tabtaker)
            {
                case 0:
                    HelpMessage("Original shows the non-custom editor class.");
                    base.OnInspectorGUI();
                    break;
                case 1:
                    HelpMessage("Unity Physics Events are raised on the normal physics events received from any active hit boxes (collision, trigger, enter, stay, etc)");
                    DrawProperty(takersPhysEvents);
                    break;
                case 2:
                    HelpMessage("Hit Boxes are the actual colliders that will register hits.");
                    DrawProperty(takersHitBoxes);
                    EditorHelper.DrawLabel("Convenience");
                    EditorHelper.DrawHelpBox("If the name matches the name of the gameobject, this button will automatically create and assign hitboxes.\n " +
                        "If colliders are already set in the hitboxes, they will not be overwritten.");
                    bool b = EditorHelper.Button("Try to Create Boxes");
                    if (b == true)
                    {
                        //find it
                        MonoBehaviour obj = target as MonoBehaviour;
                        GameObject t = obj.gameObject;
                        Transform[] children = t.GetComponentsInChildren<Transform>();
                        List<HitCollider> hitcolliders = target.HitBoxes.HitTakers.HitBoxesBase.HitBoxes;
                        TryCreateHitBoxes(target, children, hitcolliders);
                    }
                    break;
                case 3:
                    HelpMessage("Group HitBox Enablers can disable and enable hit boxes based on group names.");
                    SerializedProperty groups = takersdyno.FindPropertyRelative("Groups");
                    if (usinghitboxes)
                    {
                        NewGroupCreationStarted(instance.GetHitTakers().ToArray(), groups);
                    }
                    else
                    {
                        NewGroupCreationStarted(empty, groups);
                    }

                    DisplayGroups(groups);
                    break;
                case 4:
                    HelpMessage("Hit Receiver raises an event once a valid hit is registered");
                    //ActionEditorHelper.DrawStringHelpers(instance, false, true, true);
                    DrawProperty(takersReceiver);
                    break;
                case 5:
                    HelpMessage("Defaults are always enabled");
                    if (usinghitboxes)
                    {
                        takerdefaultsection = DrawDefaults(takerdefaultsection, instance.GetHitTakers().ToArray(), takersDefaults);
                    }
                    else
                    {
                        takerdefaultsection = DrawDefaults(takerdefaultsection, empty, takersDefaults);
                    }

                   
                    break;



            }
        }


        int giverdefaultsection = 0;
        int DrawDefaults(int selection, string[] options, SerializedProperty defaultprop)
        {
            if (options.Length < 1)
            {
                ActionEditorHelper.DrawHelpBox("Need a hitbox to make a default");
                return selection;
            }

            selection = GUILayout.SelectionGrid(selection, options, 4);

            int length = defaultprop.arraySize;
            bool canadd = true;

            ActionEditorHelper.DrawLabel("Defaults");
            EditorGUI.BeginDisabledGroup(true);
            for (int i = 0; i < length; i++)
            {
                SerializedProperty p = defaultprop.GetArrayElementAtIndex(i);
                EditorGUILayout.TextField(p.stringValue);
                if (CommonFunctions.WordEquals(p.stringValue, options[selection]))
                {
                    canadd = false;
                }
            }
            EditorGUI.EndDisabledGroup();

            string message = "Make " + options[selection] + " default";
            if (canadd == false)
            {
                message = "Remove " + options[selection] + " from defaults";
            }
            bool pressed = ActionEditorHelper.Button(message);
            if (pressed)
            {
                if (canadd)
                {
                    defaultprop.arraySize++;
                    serializedObject.ApplyModifiedProperties();
                    SerializedProperty p = defaultprop.GetArrayElementAtIndex(defaultprop.arraySize - 1);
                    p.stringValue = options[selection];
                    serializedObject.ApplyModifiedProperties();
                }
                else
                {
                    //remove it
                    List<int> removeentries = new List<int>();
                    for (int i = 0; i < length; i++)
                    {
                        SerializedProperty p = defaultprop.GetArrayElementAtIndex(i);
                        EditorGUILayout.TextField(p.stringValue);
                        if (CommonFunctions.WordEquals(p.stringValue, options[selection]))
                        {
                            removeentries.Add(i);
                        }
                    }

                    for (int i = 0; i < removeentries.Count; i++)
                    {
                        defaultprop.DeleteArrayElementAtIndex(removeentries[i]);
                    }
                    serializedObject.ApplyModifiedProperties();
                }

            }

            return selection;
        }

        string newgroup = string.Empty;
        GroupBoxes groupBox = new GroupBoxes();
        bool addgroup;
        bool shownew;
        bool addnewboxes;
        int newboxselection = 0;
        List<int> removeentries = new List<int>();
        private void DrawGivers(IHaveHitBoxes instance, ActorHitBoxes target)
        {
            switch (tabgiver)
            {
                case 0:
                    HelpMessage("Original shows the non-custom editor class.");
                    base.OnInspectorGUI();
                    break;
                case 1:
                    HelpMessage("Unity Physics Events are raised on the normal physics events from any active hit boxes (collision, trigger, enter, stay, etc)");
                    DrawProperty(giversPhysEvents);
                    break;
                case 2:
                    HelpMessage("Hit Boxes are the actual colliders that will register hits.");
                    DrawProperty(giversHitBoxes);
                    EditorHelper.DrawLabel("Convenience");
                    EditorHelper.DrawHelpBox("If the name matches the name of the gameobject, this button will automatically create and assign hitboxes.\n " +
                                  "If colliders are already set in the hitboxes, they will not be overwritten."); 
                    bool b = EditorHelper.Button("Try to Create Boxes");
                    if (b == true)
                    {
                        //find it
                        MonoBehaviour obj = target as MonoBehaviour;
                        GameObject t = obj.gameObject;
                        Transform[] children = t.GetComponentsInChildren<Transform>();
                        List<HitCollider> hitcolliders = target.HitBoxes.HitGivers.HitBoxesBase.HitBoxes;
                        TryCreateHitBoxes(target, children, hitcolliders);
                    }
                    break;
                case 3:
                    HelpMessage("Group HitBox Enablers can disable and enable hit boxes based on group names.");
                    SerializedProperty groups = giversdynobox.FindPropertyRelative("Groups");
                   // GroupCreationIni();
                   if (usinghitboxes)
                    {
                        NewGroupCreationStarted(instance.GetHitGivers().ToArray(), groups);
                    }
                    else
                    {
                        NewGroupCreationStarted(empty, groups);
                    }

                    DisplayGroups(groups);

                    break;
                case 4:
                    HelpMessage("Hit Sender raises an event for any valid hits.");

                    //ActionEditorHelper.DrawStringHelpers(instance, true, false, true);
                    DrawProperty(giverssender);


                    break;
                case 5:
                    HelpMessage("Defaults are always on");
                    if (usinghitboxes)
                    {
                        giverdefaultsection = DrawDefaults(giverdefaultsection, instance.GetHitGivers().ToArray(), giversDefaults);
                    }
                    else
                    {
                        giverdefaultsection = DrawDefaults(giverdefaultsection, empty, giversDefaults);
                    }

                    break;

            }
        }

        private static List<HitCollider> TryCreateHitBoxes(ActorHitBoxes target, Transform[] children, List<HitCollider> hitcolliders)
        {
            for (int i = 0; i < hitcolliders.Count; i++)
            {
                Collider coll = hitcolliders[i].Collider;
                if (coll == null)
                {
                    foreach (Transform eachChild in children)
                    {
                        if (eachChild.name == target.HitBoxes.HitGivers.HitBoxesBase.HitBoxes[i].Name)
                        {
                            GameObject newColliderHolder = new GameObject(eachChild.name + "_collider");
                            newColliderHolder.transform.SetParent(eachChild);
                            newColliderHolder.transform.SetPositionAndRotation(new Vector3(0, 0, 0), Quaternion.identity);
                            newColliderHolder.transform.localPosition = new Vector3(0, 0, 0);
                            newColliderHolder.transform.localRotation = Quaternion.identity;
                            CapsuleCollider capsule = newColliderHolder.AddComponent<CapsuleCollider>();
                            capsule.radius = .25f;
                            capsule.enabled = false;
                            hitcolliders[i].Collider = capsule;
                            Debug.Log("Added new collider to " + eachChild.name);
                        }
                    }
                }
            }

            return hitcolliders;
        }

        private void DisplayGroups(SerializedProperty groups)
        {
            int groupscount = groups.arraySize;
            removeentries.Clear();
            EditorGUI.indentLevel++;
            for (int i = 0; i < groupscount; i++)
            {
                EditorGUI.indentLevel++;
                EditorGUI.BeginDisabledGroup(true);
                SerializedProperty p = groups.GetArrayElementAtIndex(i);
                SerializedProperty name = p.FindPropertyRelative("GroupName");
                SerializedProperty boxes = p.FindPropertyRelative("HitBoxes");
                ActionEditorHelper.DrawLabel("Group");
                EditorGUILayout.TextField(name.stringValue);
                int boxcount = boxes.arraySize;
                ActionEditorHelper.DrawLabel("HitBoxes");
                EditorGUI.indentLevel++;
                for (int j = 0; j < boxcount; j++)
                {
                    EditorGUILayout.TextField(boxes.GetArrayElementAtIndex(j).stringValue);
                }
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
                EditorGUI.EndDisabledGroup();
                bool remove = ActionEditorHelper.Button("Remove + " + name.stringValue);
                if (remove)
                {
                    removeentries.Add(i);
                }
            }
            EditorGUI.indentLevel--;


            if (removeentries.Count > 0)
            {
                for (int i = 0; i < removeentries.Count; i++)
                {
                    groups.DeleteArrayElementAtIndex(i);
                }

            }
        }

        private void NewGroupCreationStarted(string[] options, SerializedProperty groups)
        {
            if (options.Length < 2)
            {
                ActionEditorHelper.DrawHelpBox("Can't create a group with less than 2 hitboxes");
                return;
            }

            GroupCreationIni();

            if (shownew)
            {
                EditorGUI.indentLevel++;
                newgroup = EditorGUILayout.TextField(newgroup);
                newboxselection = GUILayout.SelectionGrid(newboxselection, options, 4);

                bool add = true;
                for (int i = 0; i < groupBox.HitBoxes.Count; i++)
                {
                    if (CommonFunctions.WordEquals(groupBox.HitBoxes[i], options[newboxselection]))
                    {
                        add = false;
                        break;
                    }
                }

                string message = "Add to Group " + groupBox.GroupName;
                if (add)
                {

                }
                else
                {
                    message = "Remove from Group " + groupBox.GroupName;
                }

                if (GUILayout.Button(message))
                {
                    if (add)
                    {
                        groupBox.HitBoxes.Add(options[newboxselection]);
                    }
                    else
                    {
                        groupBox.HitBoxes.Remove(options[newboxselection]);
                    }

                }

                for (int i = 0; i < groupBox.HitBoxes.Count; i++)
                {
                    EditorGUILayout.TextField(groupBox.HitBoxes[i]);
                }

                bool save = ActionEditorHelper.Button("Save Group");
                if (save)
                {
                    groups.arraySize++;
                    serializedObject.ApplyModifiedProperties();
                    SerializedProperty p = groups.GetArrayElementAtIndex(groups.arraySize - 1);
                    SerializedProperty name = p.FindPropertyRelative("GroupName");
                    name.stringValue = groupBox.GroupName;
                    SerializedProperty list = p.FindPropertyRelative("HitBoxes");
                    list.ClearArray();
                    list.arraySize = groupBox.HitBoxes.Count;
                    for (int i = 0; i < list.arraySize; i++)
                    {
                        list.GetArrayElementAtIndex(i).stringValue = groupBox.HitBoxes[i];
                    }

                    groupBox.GroupName = string.Empty;
                    groupBox.HitBoxes.Clear();
                    shownew = false;
                    newgroup = string.Empty;
                    GUI.FocusControl(null);
                    serializedObject.ApplyModifiedProperties();

                }
                EditorGUI.indentLevel--;
            }
        }

        private void GroupCreationIni()
        {
            if (shownew == false)
            {
                EditorGUILayout.LabelField("Name");
                newgroup = EditorGUILayout.TextField(newgroup);
                string message = "Create Group " + newgroup;
                if (string.IsNullOrWhiteSpace(newgroup))
                {
                    message = "Add a name to create a group";
                }
                addgroup = GUILayout.Button(message);
                if (addgroup)
                {
                    if (string.IsNullOrWhiteSpace(newgroup))
                    {
                        EditorUtility.DisplayDialog("Name", "Need a name to make", "Okay, I'll make a name");
                        shownew = false;
                    }
                    else
                    {
                        shownew = true;
                        groupBox.GroupName = newgroup;
                    }
                }
            }
            else
            {
                if (GUILayout.Button("Clear"))
                {
                    groupBox.GroupName = string.Empty;
                    groupBox.HitBoxes.Clear();
                    newgroup = string.Empty;
                    shownew = false;
                }
            }
        }

    }
}

#endif