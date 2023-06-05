
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Linq;

namespace GWLPXL.ActionCharacter.Editor
{
    /// <summary>
    /// change config to use the starter set
    /// </summary>
    [CustomEditor(typeof(PlayerCharacter), true)]
    public class PlayerCharacterEditor : ActionCharacterEditor
    {
        protected string[] selections = new string[6] { "Original", "Movement", "Flow", "Config","Input Action Mapper", "Input Wrapper" };
        protected SerializedProperty movement;
        protected SerializedProperty movementcontroller;
        protected UnityEditor.Editor movemented;
        protected UnityEditor.Editor runtimeMovement;
        protected UnityEditor.Editor flowed;
        protected UnityEditor.Editor configed;
        protected UnityEditor.Editor actionmap;
        protected UnityEditor.Editor wrappered;
        protected int tab;
        protected bool redraw;

        protected MovementSO previous;
        protected FlowControlSO pflow;
        protected CharacterConfig cconfig;
        protected InputActionMapSO mapp;
        protected InputWrapperSO wrapper;
        protected bool redrawruntime;
        ActionCharacterWindow window;
        public override VisualElement CreateInspectorGUI()
        {

            return base.CreateInspectorGUI();
        }

       

        PlayerCharacter previousinstance;
        public override void OnInspectorGUI()
        {

            serializedObject.Update();
            PlayerCharacter instance = (PlayerCharacter)target;
            if (instance != previousinstance)
            {
                redraw = true;
                previousinstance = instance;
            }

            if (instance.Config == null)
            {
                ActionEditorHelper.DrawHelpBox("Config Required");
                instance.Config = EditorGUILayout.ObjectField(instance.Config, typeof(CharacterConfig), true) as CharacterConfig;
                return;
            }

            instance.Config.Actions.Clear();
            List<string> actions = instance.Actions.Keys.ToList();
            for (int i = 0; i < actions.Count; i++)
            {
                if (actions[i] == null) continue;
                instance.Config.Actions.Add(actions[i]);
            }
            instance.Config.ParentPoints.Clear();
            IParentPoint[] points = instance.GetComponentsInChildren<IParentPoint>();
            for (int i = 0; i < points.Length; i++)
            {
                instance.Config.ParentPoints.Add(points[i].Transform.name);
            }

            tab = GUILayout.SelectionGrid(tab, selections, 2);


            switch (tab)
            {
                case 0:
                    base.OnInspectorGUI();
                    break;
                case 1:
                    //movement = serializedObject.FindProperty("movementTemplate");
                    instance.MovementTemplate = EditorGUILayout.ObjectField(instance.MovementTemplate, typeof(MovementSO), true) as MovementSO;
                    ActionEditorHelper.DrawLabel("Movement");

                    if (instance.MovementTemplate != previous)
                    {
                        redraw = true;
                        previous = instance.MovementTemplate;
                    }

                    if (Application.isPlaying)
                    {
                        ActionEditorHelper.DrawLabel("Runtime Values");
                        ActionEditorHelper.DrawHelpBox("Runtime Values. Changes here will not persist once you exit play.");
                        if (runtimeMovement == null || redrawruntime)
                        {
                             runtimeMovement = UnityEditor.Editor.CreateEditor(instance.MovementRuntime);
                              redrawruntime = false;
                            runtimeMovement.CreateInspectorGUI();
                        }

                        runtimeMovement.OnInspectorGUI();
                    }
                    else
                    {
                        ActionEditorHelper.DrawHelpBox("Basic Locomotion");
                        redrawruntime = true;
                        if (movemented == null || redraw)
                        {
                            movemented = UnityEditor.Editor.CreateEditor(instance.MovementTemplate);

                        }
                        movemented.CreateInspectorGUI();
                        movemented.OnInspectorGUI();
                    }
                   
          
                    //ActionEditorHelper.DrawProperty(movement);
                    break;
                case 2:
                    
                    //EditorGUILayout.ObjectField(instance.Flow, typeof(FlowControlSO), false) as FlowControlSO;
   
                    ActionEditorHelper.DrawLabel("Flows");
                    ActionEditorHelper.DrawHelpBox("Combo Flows Registered during play. The view is good for debugging and see what is available.");
                    if (instance.Flow == null) break;
                    if (instance.Flow != pflow)
                    {
                        redraw = true;
                        pflow = instance.Flow;
                    }
                    if (flowed == null || redraw)
                    {
                        flowed = UnityEditor.Editor.CreateEditor(instance.Flow);

                    }
                    flowed.CreateInspectorGUI();
                    flowed.OnInspectorGUI();
                    break;
                case 3:
                    //this one is unique, readonly
                    instance.Config = EditorGUILayout.ObjectField(instance.Config, typeof(CharacterConfig), true) as CharacterConfig;
                    ActionEditorHelper.DrawLabel("Config");
                    ActionEditorHelper.DrawHelpBox("List of HitBoxes and Actions. Actions show during play.");


                    if (instance.Config != cconfig)
                    {
                        redraw = true;
                        cconfig = instance.Config;
                    }
                    if (configed == null || redraw)
                    {
                        configed = UnityEditor.Editor.CreateEditor(instance.Config);

                    }

                    configed.CreateInspectorGUI();
                    configed.OnInspectorGUI();

                    break;
                case 4:
                    //instance.InputRequirements = EditorGUILayout.ObjectField(instance.InputRequirements, typeof(InputActionMapSO), true) as InputActionMapSO;
                    ActionEditorHelper.DrawLabel("Input Action Map");
                    ActionEditorHelper.DrawHelpBox("Input Requirements. The actions are loaded at runtime and keyed to each slot based on the key in the ActionSet.");

                    if (instance.InputRequirements != mapp)
                    {
                        redraw = true;
                        mapp = instance.InputRequirements;
                    }
                    if (actionmap == null || redraw)
                    {
                        actionmap = UnityEditor.Editor.CreateEditor(instance.InputRequirements);
                        actionmap.CreateInspectorGUI();
                    }

             
                    actionmap.OnInspectorGUI();


                    break;
                case 5:
                    instance.InputWrapper = EditorGUILayout.ObjectField(instance.InputWrapper, typeof(InputWrapperSO), true) as InputWrapperSO;
                    ActionEditorHelper.DrawLabel("Input Wrapper");
                    SerializedProperty strafe = serializedObject.FindProperty("strafe");
                    SerializedProperty lockon = serializedObject.FindProperty("lockOn");
                    ActionEditorHelper.DrawProperty(strafe);
                    ActionEditorHelper.DrawProperty(lockon);
                    if (instance.InputWrapper != wrapper)
                    {
                        redraw = true;
                        wrapper = instance.InputWrapper;
                    }
                    if (actionmap == null || redraw)
                    {
                        wrappered = UnityEditor.Editor.CreateEditor(instance.InputWrapper);

                    }
                    wrappered.CreateInspectorGUI();
                    wrappered.OnInspectorGUI();
                    break;
            }


            redraw = false;

            serializedObject.ApplyModifiedProperties();

            bool editorwindow = ActionEditorHelper.Button("Open Action Character Editor");
            if (editorwindow)
            {
                CharacterActionWindow.Show(instance);
             
            }

            bool preview = ActionEditorHelper.Button("Open Action Preview");
            if (preview)
            {
                WindowActionPreview.ShowWindow(instance);
            }

            ActionEditorHelper.MediumGUISpace();

            DrawAnimatorHelperButton(instance.Animator);
            DrawCreateNewConfig(instance);
            DrawCreateNewMovementTemplate(instance);
        }
    }
}

#endif