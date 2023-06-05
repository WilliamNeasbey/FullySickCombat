using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace GWLPXL.ActionCharacter.Editor
{
    public static class PopUpWindowBasic
    {
        public static void ShowBasicPopUp(UnityEngine.Object target)
        {
            BasicPopupWindow window = (BasicPopupWindow)EditorWindow.GetWindow(typeof(BasicPopupWindow));
            window.Show();
            window.Target = target;
            if (window.TargetEditor != null)
            {
                UnityEngine.GameObject.DestroyImmediate(window.TargetEditor);
            }
            window.TargetEditor = UnityEditor.Editor.CreateEditor(target);
            window.TargetEditor.CreateInspectorGUI();
        }
    }

    public class BasicPopupWindow : UnityEditor.EditorWindow
    {
        public Object Target;
        public UnityEditor.Editor TargetEditor;
        public virtual void Init(UnityEngine.Object target)
        {
            PopUpWindowBasic.ShowBasicPopUp(target);
        }

        protected virtual void OnGUI()
        {
            if (Target is ScriptableObject)
            {
                ScriptableObject so = Target as ScriptableObject;
                EditorUtility.SetDirty(so);
            }
            SerializedObject obj = new SerializedObject(Target);
            obj.Update();
            if (TargetEditor == null)
            {
                this.Close();
            }
            else
            {
                TargetEditor.OnInspectorGUI();
                if (GUILayout.Button("Close!")) this.Close();
            }
            obj.ApplyModifiedProperties();
        }

    }
}
