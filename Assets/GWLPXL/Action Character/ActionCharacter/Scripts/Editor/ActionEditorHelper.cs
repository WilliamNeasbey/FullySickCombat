
#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace GWLPXL.ActionCharacter.Editor
{
   
    public class EditorInspectorDraw
    {
        public UnityEditor.Editor Editor = null;
        public bool Redraw = true;
        public Object cachemap = null;
        Vector2 sv;
        public EditorInspectorDraw()
        {

        }
        public void Draw(Object target, bool disabled = false)
        {
            if (target == null)
            {
                cachemap = null;
                return;
            }
            if (cachemap != target)
            {
                Redraw = true;
                cachemap = target;
            }

            if (Redraw)
            {
                Editor = UnityEditor.Editor.CreateEditor(cachemap);
                Editor.CreateInspectorGUI();
            }

            EditorGUI.BeginDisabledGroup(disabled);
            sv = EditorGUILayout.BeginScrollView(sv);
            Editor.OnInspectorGUI();
            EditorGUILayout.EndScrollView();
            EditorGUI.EndDisabledGroup();
        }

    }

    //neutral editor helper
    public static class EditorHelper
    {
        public static int LargeFont = 22;
        public static int MediumFont = 16;
        public static int SmallFont = 12;
        public static int MediumSpace = 25;
        public static int SmallSpace = 10;
        public static int LargeSpace = 50;
        public static int tinySpace = 5;

        static Color remove = Color.red;
        static Color confirm = Color.green;
        static Color select = Color.blue;
        static Color header = Color.blue;
        const string selecthtml = "#FFB900";
        const string removehtml = "#B900FF";
        const string confirmhtml = "#00FFB9";
        const string headerhtml = "#77caff";
        public static void DrawProperty(SerializedProperty prop)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            EditorGUILayout.PropertyField(prop);
            GUILayout.Space(10);
            GUILayout.EndHorizontal();
        }

        public static void DrawLabel(string lable)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField(lable, EditorStyles.boldLabel);
        }
        public static void DrawLabel(string lable, GUIStyle style)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField(lable, style);
        }


        public static void DrawHelpBox(string message, int space = 5)
        {
            EditorGUILayout.Space(space);
            EditorGUILayout.HelpBox(message, MessageType.Info);
        }

        public static EditorInspectorDraw CreateEditor()
        {
            return new EditorInspectorDraw();
        }
        public static int PopupSelection(int selection, string[] options)
        {
            selection = EditorGUILayout.Popup(selection, options);
            return selection;
        }
        public static int TabSelection(int selection, string[] options, int perrow)
        {
            selection = GUILayout.SelectionGrid(selection, options, perrow);
            return selection;
        }
        public static void SmallGUISpace()
        {
            GUILayout.Space(SmallSpace);
        }
        public static void MediumGUISpace()
        {
            GUILayout.Space(MediumSpace);
        }

        public static void LargeGUISpace()
        {
            GUILayout.Space(LargeSpace);
        }

        public static void TinyGUISpace()
        {
            GUILayout.Space(tinySpace);
        }
        public static bool RemoveButton(string thingtoremove)
        {
            return GUILayout.Button("Remove " + thingtoremove);
        }
        public static bool AddButton(string thingtoadd)
        {
            return GUILayout.Button("Add " + thingtoadd);


        }

        public static Color GetSelectColor(string htmloverride = "")
        {
            if (string.IsNullOrWhiteSpace(htmloverride))
            {
                htmloverride = selecthtml;
            }
            ColorUtility.TryParseHtmlString(htmloverride, out select);
            GUI.color = select;
            return select;
        }
        public static Color GetRemoveColor(string htmloverride = "")
        {
            if (string.IsNullOrWhiteSpace(htmloverride))
            {
                htmloverride = removehtml;
            }
            ColorUtility.TryParseHtmlString(htmloverride, out remove);
            GUI.color = remove;
            return remove;
        }
        public static Color GetHeaderColor(string htmloverride = "")
        {
            if (string.IsNullOrWhiteSpace(htmloverride))
            {
                htmloverride = headerhtml;
            }
            ColorUtility.TryParseHtmlString(htmloverride, out header);
            GUI.color = header;
            return remove;
        }
        public static Color GetConfirmColor(string htmloverride = "")
        {
            if (string.IsNullOrWhiteSpace(htmloverride))
            {
                htmloverride = confirmhtml;
            }
            ColorUtility.TryParseHtmlString(htmloverride, out confirm);
            GUI.color = confirm;
            return confirm;
        }

        public static void ResetGUIColor()
        {
            GUI.color = Color.white;
        }
        static bool button = false;
        public static bool Button(string thingtoadd, GUILayoutOption[] options = null, bool removecolor = true)
        {
            button = GUILayout.Button(thingtoadd, options);
            if (removecolor)
            {
                ResetGUIColor();
            }
            return button;


        }


        public static Texture2D LoadImage(string projectpath)
        {
            Texture2D image = AssetDatabase.LoadAssetAtPath(projectpath, typeof(Texture2D)) as Texture2D;
            return image;
        }
    }

    /// <summary>
    /// action character specific
    /// </summary>
    public static class ActionEditorHelper
    {
        public static int LargeFont = 22;
        public static int MediumFont = 16;
        public static int SmallFont = 12;
        public static int MediumSpace = 25;
        public static int SmallSpace = 10;
        public static int LargeSpace = 50;
        public static int tinySpace = 5;
        static string[] actions = new string[0];
        static string editorImagesPath = "Assets/GWLPXL/Action Character/ActionCharacter/Common Data/Editor/Images/";

        static List<SerializedProperty> disposable = new List<SerializedProperty>();
        public static Texture2D LoadImage(string assetname)
        {
            return EditorHelper.LoadImage(editorImagesPath+assetname);
        }
        public static bool ContainsName(string[] array, string name)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (CommonFunctions.WordEquals(array[i], name))
                {
                    return true;
                }
            }
            return false;
        }

        public static void Dispose()
        {
            for (int i = 0; i < disposable.Count; i++)
            {
                if (disposable[i] == null) continue;
                disposable[i].Dispose();
            }
            disposable.Clear();
        }
      public static void AddToDispose(SerializedProperty p)
        {
            disposable.Add(p);
        }
        public static string[] HitTakers(CharacterConfig config)
        {
            return config.HitTakerBoxes.ToArray();
        }
        public static string[] HitGivers(CharacterConfig config)
        {
            return config.HitGiverBoxes.ToArray();
        }
        public static string[] ActionNames(ActionDatabase db)
        {
            actions = new string[db.Actions.Count];
            for (int i = 0; i < db.Actions.Count; i++)
            {
                actions[i] = db.Actions[i].GetActionName();
            }
            return actions;
        }
        public static EditorInspectorDraw CreateEditor()
        {
            return EditorHelper.CreateEditor();
        }

        public static int PopupSelection(int selection, string[] options)
        {
            return EditorHelper.PopupSelection(selection, options);
        }
        public static int TabSelection(int selection, string[] options, int perrow)
        {
            return EditorHelper.TabSelection(selection, options, perrow);

        }
        public static void SmallGUISpace()
        {
            EditorHelper.SmallGUISpace();
        }
        public static void MediumGUISpace()
        {
            EditorHelper.MediumGUISpace();

        }

        public static void LargeGUISpace()
        {
            EditorHelper.LargeGUISpace();

        }

        public static void TinyGUISpace()
        {
            EditorHelper.TinyGUISpace();
        }
        public static bool RemoveButton(string thingtoremove)
        {
            return EditorHelper.RemoveButton(thingtoremove);

        }
        public static bool AddButton(string thingtoadd)
        {
            return EditorHelper.AddButton(thingtoadd);

        }


        public static Color GetSelectColor(string htmloverride = "")
        {
            return EditorHelper.GetSelectColor(htmloverride);
            
        }
        public static Color GetRemoveColor(string htmloverride = "")
        {
            return EditorHelper.GetRemoveColor(htmloverride);
            
        }
        public static Color GetHeaderColor(string htmloverride = "")
        {
            return EditorHelper.GetHeaderColor(htmloverride);
            
        }
        public static Color GetConfirmColor(string htmloverride = "")
        {
            return EditorHelper.GetConfirmColor(htmloverride);
          
        }

        public static void ResetGUIColor()
        {
            EditorHelper.ResetGUIColor();

        }
  
        public static bool Button(string thingtoadd, GUILayoutOption[] options = null, bool removecolor = true)
        {
            return EditorHelper.Button(thingtoadd, options, removecolor);

        }
        public static GUIStyle GetActionsInDatabase(int fontsize, TextAnchor anchor)
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = fontsize;
            style.padding = EditorStyles.boldLabel.padding;
            style.alignment = anchor;
            style.normal = EditorStyles.boldLabel.normal;
            style.onNormal = EditorStyles.boldLabel.onNormal;
            return style;
        }
        public static GUIStyle GetNewActionStyle(int fontsize, TextAnchor anchor)
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = fontsize;
            style.padding = EditorStyles.boldLabel.padding;
            style.alignment = anchor;
            style.normal = EditorStyles.boldLabel.normal;
            style.onNormal = EditorStyles.boldLabel.onNormal;
            return style;
        }
        public static GUIStyle GetActionSelectionActionMaker(int fontsize, TextAnchor anchor)
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = fontsize;
            style.padding = EditorStyles.boldLabel.padding;
            style.alignment = anchor;
            style.normal = EditorStyles.boldLabel.normal;
            style.onNormal = EditorStyles.boldLabel.onNormal;
            return style;
        }
        public static GUIStyle GetActionComboString(int fontsize, TextAnchor anchor)
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = fontsize;
            style.padding = EditorStyles.boldLabel.padding;
            style.alignment = anchor;
            style.normal = EditorStyles.boldLabel.normal;
            style.onNormal = EditorStyles.boldLabel.onNormal;
            return style;
        }
        public static GUIStyle GetActionSelectionFlowMaker(int fontsize)
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = fontsize;
            style.padding = EditorStyles.boldLabel.padding;
            style.alignment = EditorStyles.boldLabel.alignment;
            style.normal = EditorStyles.boldLabel.normal;
            style.onNormal = EditorStyles.boldLabel.onNormal;
            return style;
        }
        public static void ImportExportButtons(UnityEngine.Object so)
        {
            GUILayout.Space(25);
            EditorGUILayout.LabelField("Export and Import Options", EditorStyles.boldLabel);
            if (GUILayout.Button("Export to Json Text File"))
            {
                string title = so.name;
                string path = EditorUtility.SaveFilePanelInProject("Export Json", title, "json", "Export to json");
                if (path.Length > 0)
                {
                    string json = JsonUtility.ToJson(so, true);
                    ResourceManager.SaveJson(json, path);
                    AssetDatabase.Refresh();
                }
            }


            if (GUILayout.Button("Import from Json Text File"))
            {
                string path = EditorUtility.OpenFilePanel("Import Json", "Assets", "json");
                if (path.Length > 0)
                {
                    string json = ResourceManager.ReadJson(path);
                    JsonUtility.FromJsonOverwrite(json, so);

                }
            }
        }

        /// <summary>
        /// just use the other for now
        /// </summary>
        /// <param name="so"></param>
        public static void ImportExportFlows(FlowControlSO so)
        {
            GUILayout.Space(25);
            EditorGUILayout.LabelField("Export and Import Options", EditorStyles.boldLabel);
            if (GUILayout.Button("Export to Json Text File"))
            {
                string title = so.name;
                string path = EditorUtility.SaveFilePanelInProject("Export Json", title, "json", "Export to json");
                if (path.Length > 0)
                {
                    string json = JsonUtility.ToJson(so, true);
                    ResourceManager.SaveJson(json, path);
                    AssetDatabase.Refresh();
                }
            }


            if (GUILayout.Button("Import from Json Text File"))
            {
                string path = EditorUtility.OpenFilePanel("Import Json", "Assets", "json");
                if (path.Length > 0)
                {
                    string json = ResourceManager.ReadJson(path);
                    string[] pieces = json.Split('\n');
                    bool start = false;
                    string custom = string.Empty;
                    for (int i = 0; i < pieces.Length; i++)
                    {
                        if (pieces[i].StartsWith("Flows") && start == false)
                        {
                            start = true;
                        }
                        if (pieces[i].StartsWith("NewFlow") && start == true)
                        {
                            start = false;
                        }

                        if (start)
                        {
                            custom += pieces[i];
                        }

                    }
                    Debug.Log(custom);
                    // JsonUtility.FromJsonOverwrite(json, so);

                }
            }
        }

       

        public static void DrawProperty(SerializedProperty prop)
        {
            EditorHelper.DrawProperty(prop);

        }

      
        public static void DrawLabel(string lable)
        {
            EditorHelper.DrawLabel(lable);
        }
        public static void DrawLabel(string lable, GUIStyle style)
        {
            EditorHelper.DrawLabel(lable, style);

        }

      
        public static void DrawHelpBox(string message, int space = 5)
        {
            EditorHelper.DrawHelpBox(message, space);
        }

        public static void DrawStringHelpers(List<string> stringvalues)
        {
            int rowlsize = 4;
            EditorGUILayout.LabelField("Available Actions");
            EditorGUI.indentLevel++;
            GUILayout.BeginHorizontal();

            for (int i = 0; i < stringvalues.Count; i++)
            {
                if (i % rowlsize == 0)
                {
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                }
                EditorGUILayout.TextField(stringvalues[i]);
            }
            EditorGUI.indentLevel--;
            GUILayout.EndHorizontal();
        }
        public static void DrawStringHelpers(CharacterConfig instance, bool hitgivers, bool hittakers, bool actions, bool parentpoints)
        {
            //show all options or just some
            int rowlsize = 4;
            EditorGUILayout.LabelField("Self Info", EditorStyles.boldLabel);
            if (hitgivers)
            {
                EditorGUILayout.LabelField("Available Hit Givers");
                EditorGUI.indentLevel++;
                GUILayout.BeginHorizontal();
                for (int i = 0; i < instance.HitGiverBoxes.Count; i++)
                {
                    if (i % rowlsize == 0)
                    {
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                    }
                    EditorGUILayout.TextField(instance.HitGiverBoxes[i]);
                }
                EditorGUI.indentLevel--;
                GUILayout.EndHorizontal();
            }

            if (hittakers)
            {


                EditorGUILayout.LabelField("Available Hit Takers");
                EditorGUI.indentLevel++;
                GUILayout.BeginHorizontal();

                for (int i = 0; i < instance.HitTakerBoxes.Count; i++)
                {
                    if (i % rowlsize == 0)
                    {
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                    }
                    EditorGUILayout.TextField(instance.HitTakerBoxes[i]);
                }
                EditorGUI.indentLevel--;
                GUILayout.EndHorizontal();
            }
            if (actions)
            {
                EditorGUILayout.LabelField("Available Actions");
                EditorGUI.indentLevel++;
                GUILayout.BeginHorizontal();

                for (int i = 0; i < instance.Actions.Count; i++)
                {
                    if (i % rowlsize == 0)
                    {
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                    }
                    EditorGUILayout.TextField(instance.Actions[i]);
                }
                EditorGUI.indentLevel--;
                GUILayout.EndHorizontal();
            }
            if (parentpoints)
            {
                EditorGUILayout.LabelField("Available Parent Points");
                EditorGUI.indentLevel++;
                GUILayout.BeginHorizontal();

                for (int i = 0; i < instance.ParentPoints.Count; i++)
                {
                    if (i % rowlsize == 0)
                    {
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                    }
                    EditorGUILayout.TextField(instance.ParentPoints[i]);
                }
                EditorGUI.indentLevel--;
                GUILayout.EndHorizontal();
            }
            EditorGUILayout.Space(25);
        }
        public static void DrawStringHelpers(ActionCharacter instance, bool hitgivers, bool hittakers, bool actions)
        {
            //show all options or just some
            int rowlsize = 4;
            EditorGUILayout.LabelField("Self Info", EditorStyles.boldLabel);
            if (hitgivers)
            {
                EditorGUILayout.LabelField("Available Hit Givers");



                if (instance != null)
                {
                    GUILayout.BeginHorizontal();
                    EditorGUI.indentLevel++;
                    for (int i = 0; i < instance.Config.HitGiverBoxes.Count; i++)
                    {
                        if (i % rowlsize == 0)
                        {
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                        }
                        EditorGUILayout.TextField(instance.Config.HitGiverBoxes[i]);
                    }
                    EditorGUI.indentLevel--;
                    GUILayout.EndHorizontal();
                }
              
            }

            if (hittakers)
            {


                EditorGUILayout.LabelField("Available Hit Takers");

           

                if (instance != null)
                {
                    GUILayout.BeginHorizontal();
                    EditorGUI.indentLevel++;
                    for (int i = 0; i < instance.Config.HitTakerBoxes.Count; i++)
                    {
                        if (i % rowlsize == 0)
                        {
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                        }
                        EditorGUILayout.TextField(instance.Config.HitTakerBoxes[i]);
                    }
                    EditorGUI.indentLevel--;
                    GUILayout.EndHorizontal();
                }

              
            }
            if (actions)
            {


                EditorGUILayout.LabelField("Available Actions");

                if (instance != null)
                {
                    GUILayout.BeginHorizontal();
                    EditorGUI.indentLevel++;
                    for (int i = 0; i < instance.Config.Actions.Count; i++)
                    {
                        if (i % rowlsize == 0)
                        {
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                        }
                        EditorGUILayout.TextField(instance.Config.Actions[i]);
                    }
                    EditorGUI.indentLevel--;
                    GUILayout.EndHorizontal();
                }
               
            }
            EditorGUILayout.Space(25);
        }
    }

}
#endif
