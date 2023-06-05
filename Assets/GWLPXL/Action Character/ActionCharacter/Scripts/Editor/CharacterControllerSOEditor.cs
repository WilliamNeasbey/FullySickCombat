#if UNITY_EDITOR


using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;

namespace GWLPXL.ActionCharacter.Editor
{

    [UnityEditor.CustomEditor(typeof(MovementSO))]
    public class CharacterControllerSOEditor : UnityEditor.Editor
    {
        //int index = 0;
        protected string[] options = new string[4] { "Original", "Locomotion", "Rotation", "Fall" };
        protected SerializedProperty locomotion;
        protected  SerializedProperty rotation;
        protected SerializedProperty falling;

        protected SerializedProperty movementparent;
        protected SerializedProperty standardchild;
        protected SerializedProperty locomotionparent;
        protected Vector2 scrollv;
        protected int selection;

        
        public override VisualElement CreateInspectorGUI()
        {


            return base.CreateInspectorGUI();
        }
        public override void OnInspectorGUI()
        {
            movementparent = serializedObject.FindProperty("Movement");
            ActionEditorHelper.AddToDispose(movementparent);
            standardchild = movementparent.FindPropertyRelative("Standard");
            ActionEditorHelper.AddToDispose(standardchild);

            locomotionparent = standardchild.FindPropertyRelative("Locomotion");
            ActionEditorHelper.AddToDispose(locomotionparent);

            rotation = locomotionparent.FindPropertyRelative("Rotate");
            ActionEditorHelper.AddToDispose(rotation);

            locomotion = locomotionparent.FindPropertyRelative("Locomotion");
            ActionEditorHelper.AddToDispose(locomotion);

            falling = locomotionparent.FindPropertyRelative("Fall");
            ActionEditorHelper.AddToDispose(falling);


            serializedObject.Update();

            selection = GUILayout.SelectionGrid(selection, options, 3);

            switch (selection)
            {
                case 0:
                    scrollv = GUILayout.BeginScrollView(scrollv);
                    base.OnInspectorGUI();
                    GUILayout.EndScrollView();
                    break;
                case 1:
                    //
                    DrawProperty(locomotion);
                    break;
                case 2:
                    DrawProperty(rotation);
                    break;
                case 3:
                    DrawProperty(falling);
                    break;

            }





            serializedObject.ApplyModifiedProperties();
            ActionEditorHelper.Dispose();

        }

        public void DrawProperty(SerializedProperty prop)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            EditorGUILayout.PropertyField(prop);
            GUILayout.Space(10);
            GUILayout.EndHorizontal();
        }
    }

}

#endif

