using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter.Editor
{
    public class PlayerCreatorWindow : EditorWindow
    {
        public GameObject GameObject;
        public CharacterConfig Config;
        public MovementSO MovementTemplate;
        public CharacterActionLoadoutSO StarterSet;
        public ActionsDatabaseSO ActionsDatabase;
        public InputActionMapSO InputActionMap;
        public InputWrapperSO InputWrapper;

        [MenuItem("GWLPXL/ActionCharacter/Windows/Player Creator Window")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            PlayerCreatorWindow window = (PlayerCreatorWindow)EditorWindow.GetWindow(typeof(PlayerCreatorWindow));
            window.Show();
        }

        
        protected virtual void OnGUI()
        {
            bool valid = true;
            GameObject = EditorGUILayout.ObjectField(GameObject, typeof(GameObject), true) as GameObject;
            if (GameObject == null)
            {
                //error
                ActionEditorHelper.DrawHelpBox("Insert the GameObject you want to use as the Player");
                valid = false;
            }

            Config = EditorGUILayout.ObjectField(Config, typeof(CharacterConfig), true) as CharacterConfig;
            if (Config == null)
            {
                ActionEditorHelper.DrawHelpBox("Create or Insert a config to use for the Player");
                bool create = ActionEditorHelper.Button("Create New Config");
                if (create)
                {

                }
                valid = false;
            }

            MovementTemplate = EditorGUILayout.ObjectField(MovementTemplate, typeof(MovementSO), true) as MovementSO;
            if (MovementTemplate == null)
            {
                ActionEditorHelper.DrawHelpBox("Create or Insert Movement Template for the Player");
                bool create = ActionEditorHelper.Button("Create New Movement Template");
                if (create)
                {

                }
                valid = false;
            }

            StarterSet = EditorGUILayout.ObjectField(StarterSet, typeof(CharacterActionLoadoutSO), true) as CharacterActionLoadoutSO;
            if (StarterSet == null)
            {
                ActionEditorHelper.DrawHelpBox("Insert Starter for the Player");
                bool create = ActionEditorHelper.Button("Open Action Creator Window");
                if (create)
                {

                }
                valid = false;
            }

            ActionsDatabase = EditorGUILayout.ObjectField(ActionsDatabase, typeof(ActionsDatabaseSO), true) as ActionsDatabaseSO;
            if (ActionsDatabase == null)
            {
                ActionEditorHelper.DrawHelpBox("Insert Actions Database for the Player");
                bool create = ActionEditorHelper.Button("Open Action Creator Window");
                if (create)
                {

                }
                valid = false;
            }

            InputActionMap = EditorGUILayout.ObjectField(InputActionMap, typeof(InputActionMapSO), true) as InputActionMapSO;
            if (InputActionMap == null)
            {
                ActionEditorHelper.DrawHelpBox("Insert Input Action Map for the Player");
                bool create = ActionEditorHelper.Button("Open Action Creator Window");
                if (create)
                {

                }
                valid = false;
            }

            InputWrapper = EditorGUILayout.ObjectField(InputWrapper, typeof(InputWrapperSO), true) as InputWrapperSO;
            if (InputWrapper == null)
            {
                ActionEditorHelper.DrawHelpBox("Insert Input Wrapper for the Player");
                //bool create = ActionEditorHelper.Button("Open Action Creator Window");
                //if (create)
                //{

                //}
                valid = false;
            }

            if (valid == false || GameObject == null)
            {
                ActionEditorHelper.DrawHelpBox("Missing Required fields, can't modify.");
                return;
            }
           


            Animator animator = GameObject.GetComponentInChildren<Animator>();
            if (animator == null)
            {
                ActionEditorHelper.DrawHelpBox("GameObject requires an Animator at the root or on a child.");
                return;
            }



          


            bool modify = ActionEditorHelper.Button("Modify " + GameObject.name);

            if (modify)
            {
                PlayerCharacterCC cc = GameObject.GetComponent<PlayerCharacterCC>();
                if (cc == null)
                {
                    cc = GameObject.AddComponent<PlayerCharacterCC>();
                }
                cc.Config = Config;
                cc.MovementTemplate = MovementTemplate;
                cc.Loadout = StarterSet;
                cc.ActionsDatabase = ActionsDatabase;
                cc.InputRequirements = InputActionMap;
                cc.InputWrapper = InputWrapper;

                ActorHitBoxes boxes = GameObject.GetComponent<ActorHitBoxes>();
                if (boxes == null)
                {
                    boxes = GameObject.AddComponent<ActorHitBoxes>();
                }

                IAnimatorController ac = animator.gameObject.GetComponent<IAnimatorController>();
                if (ac == null)
                {
                    ac = animator.gameObject.AddComponent<AnimatorControllerScript>();
                }
                IAnimationEvents aes = animator.gameObject.GetComponent<IAnimationEvents>();
                if (aes == null)
                {
                    aes = animator.gameObject.AddComponent<AnimationEventScript>();
                }
                IRootMotion rm = animator.gameObject.GetComponent<IRootMotion>();
                if (rm == null)
                {
                    rm = animator.gameObject.AddComponent<RootMotionScript>();
                }


            }
        }
    }
    
}
