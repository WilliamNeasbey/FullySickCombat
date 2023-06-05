using UnityEngine;
using System.Text;
/// <summary>
/// goal, most re-used methods so dont need to re-write things
/// </summary>
namespace GWLPXL.ActionCharacter
{
    public enum DebugMessageType
    {
        Log = 0,
        Warning = 1,
        Error = 2
    }
    public static class DebugHelpers
    {
        
        public static bool UseDebug = true;
        public static StringBuilder sb = new StringBuilder();

        public static string GetString(string message)
        {
            sb.Clear();
            sb.Append(message);
            return sb.ToString();
        }

        
        /// <summary>
        /// 0 is default,
        /// 1 is input
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ctx"></param>
        /// <param name="type"></param>
        /// <param name="messagetype"></param>
        /// 
        public static void DebugMessage(string message, UnityEngine.Object ctx, DebugMessageType type, int messagetype = 0)
        {
            if (UseDebug == false)
            {
                return;
            }

            switch (messagetype)
            {
                case 1:
                    //message = FormatInputDebug(message);
                    break;
            }
            switch (type)
            {
                case DebugMessageType.Log:
                    Debug.Log(message, ctx);
                    break;
                case DebugMessageType.Warning:
                    Debug.LogWarning(message, ctx);
                    break;
                case DebugMessageType.Error:
                    Debug.LogError(message, ctx);
                    break;
            }
     
        }

        public static Color GetColorForInput()
        {
            return Color.green;
        }

        public static string FormatSuccessResponse(string message)
        {
            if (UseDebug == false) return message;

            sb.Clear();
            sb.Append("<color=green>" + message.ToUpper() + "</color>");
            return sb.ToString();
        }

        public static string FormatFailedResponse(string message)
        {
            if (UseDebug == false) return message;

            sb.Clear();
            sb.Append("<color=red>" + message.ToUpper() + "</color>");
            return sb.ToString();
        }
        public static string FormatInputDebug(string message)
        {
            if (UseDebug == false) return message;

            sb.Clear();
            sb.Append("<color=green>" + message + "</color>");
            return sb.ToString();
        }
        public static string FormatFlowDebug(string message)
        {
            if (UseDebug == false) return message;

            sb.Clear();
            sb.Append("<color=aqua>" + message + "</color>");
            return sb.ToString();
        }
        public static string FormatActionDebug(string message)
        {
            if (UseDebug == false) return message;

            sb.Clear();
            sb.Append("<color=lightblue>" + message + "</color>");
            return sb.ToString();
        }
        public static string FormatHitBoxDebug(string message)
        {
            if (UseDebug == false) return message;

            sb.Clear();
            sb.Append("<color=lime>" + message + "</color>");
            return sb.ToString();
        }
        public static void DebugWireSphere(Vector3 center, float radius, Color color)
        {
#if UNITY_EDITOR
            //UnityEditor.Handles.SphereHandleCap(0, center, Quaternion.identity, radius, EventType.Repaint);
           // Gizmos.color = color;
            //Gizmos.DrawWireSphere(center, radius);
#endif
        }
        public static void DebugLine(Vector3 start, Vector3 end, Color color)
        {
#if UNITY_EDITOR
           // UnityEditor.Handles.DrawLine(start, end);
            Debug.DrawLine(start, end, color);
#endif
        }

        
        public static void DrawBezier(Vector3 start, Vector3 end, Color color, Texture2D text, float thickness)
        {
#if UNITY_EDITOR
            Vector3 p1 = start;
            Vector3 p2 = end;
            Color c = color;
            Texture2D tet = text;
            float thick = thickness;
            UnityEditor.Handles.DrawBezier(p1, p2, p1, p2, color, tet, thick);

#endif
        }
    }

}