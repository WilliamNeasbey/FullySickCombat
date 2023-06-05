using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace GWLPXL.ActionCharacter.Editor
{
    [CustomEditor(typeof(ActionCharacter), true)]
    public class ActionCharacterEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();


        }

       
        protected virtual void DrawCreateNewMovementTemplate(ActionCharacter character)
        {
            bool confirm = ActionEditorHelper.Button("Create New Movement Template");
            if (confirm)
            {
                string path = EditorUtility.SaveFilePanelInProject("Create Movement Template", character.gameObject.name + "_Movement", "asset", "Choose where to create the Movement Template");
                if (path.Length > 0)
                {
                    MovementSO movement = ScriptableObject.CreateInstance<MovementSO>();
                    character.MovementTemplate = movement;
                    AssetDatabase.CreateAsset(movement, path);

                }

            }
        }
        protected virtual void DrawCreateNewConfig(ActionCharacter character)
        {
            bool confirm = ActionEditorHelper.Button("Create New Config");
            if (confirm)
            {
                string path = EditorUtility.SaveFilePanelInProject("Create Config", character.gameObject.name + "_Config", "asset", "Choose where to create the Config");
                if (path.Length > 0)
                {
                    CharacterConfig config = ScriptableObject.CreateInstance<CharacterConfig>();
                    character.Config = config;
                    AssetDatabase.CreateAsset(config, path);
            
                }
        
            }
        }
        protected virtual void DrawAnimatorHelperButton(Animator target)
        {
           if (target != null)
            {
                //check if already have
                IAnimationEvents events = target.GetComponent<IAnimationEvents>();
                IRootMotion root = target.GetComponent<IRootMotion>();
                IAnimatorController controller = target.GetComponent<IAnimatorController>();
                IActionIK IK = target.GetComponent<IActionIK>();
                if (events != null && root != null && controller != null && IK != null)
                {
                    //all good.
                    return;
                }
                bool confirm = ActionEditorHelper.Button("Add Required Animator Scripts");

                if (confirm)
                {
                    if (events == null)
                    {
                        target.gameObject.AddComponent<AnimationEventScript>();

                    }
                    if (root == null)
                    {
                        target.gameObject.AddComponent<RootMotionScript>();
                    }
                    if (controller == null)
                    {
                        target.gameObject.AddComponent<AnimatorControllerScript>();
                    }
                    if (IK == null)
                    {
                        target.gameObject.AddComponent<IKController>();
                    }
                }
            }
        }
    }
}
