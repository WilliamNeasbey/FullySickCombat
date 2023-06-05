
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{



    [AddComponentMenu("")] // Don't display in add component menu
    public class ExampleHelpWindow : MonoBehaviour
    {
        public void Display()
        {
            Show = true;
        }

        public void Hide()
        {
            Show = false;
        }
        public bool Show = false;
        public string m_Title;
        [TextArea(minLines: 10, maxLines: 50)]
        public string m_Description;
        [TextArea(minLines: 10, maxLines: 50)]
        public string windows_Description;
        [TextArea(minLines: 10, maxLines: 50)]
        public string android_Description;

        private const float kPadding = 40f;

        private void OnGUI()
        {
            if (Show)
            {
                if (Application.platform == RuntimePlatform.WebGLPlayer || Application.platform == RuntimePlatform.WindowsPlayer)
                {
                    m_Description = windows_Description;
                }
                else if (Application.platform == RuntimePlatform.Android)
                {
                    m_Description = android_Description;
                }
                Vector2 size = GUI.skin.label.CalcSize(new GUIContent(m_Description));
                Vector2 halfSize = size * 0.5f;

                float maxWidth = Mathf.Min(Screen.width - kPadding, size.x);
                float left = Screen.width * 0.5f - maxWidth * 0.5f;
                float top = Screen.height * 0.4f - halfSize.y;

                Rect windowRect = new Rect(left, top, maxWidth, size.y);
                GUILayout.Window(400, windowRect, (id) => DrawWindow(id, maxWidth), m_Title);

                TickManager.FreezeTicks(true);

            }
        }

        private void DrawWindow(int id, float maxWidth)
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer || Application.platform == RuntimePlatform.WindowsPlayer)
            {
                m_Description = windows_Description;
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                m_Description = android_Description;
            }
            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label(m_Description);
            GUILayout.EndVertical();
            if (GUILayout.Button("Got it!"))
            {
                Show = false;
                TickManager.FreezeTicks(false);
                FauxPlaceHolderControl pc = FindObjectOfType<FauxPlaceHolderControl>();
                if (pc != null)
                {
                    pc.StartMethods();
                }


            }
        }
    }

}