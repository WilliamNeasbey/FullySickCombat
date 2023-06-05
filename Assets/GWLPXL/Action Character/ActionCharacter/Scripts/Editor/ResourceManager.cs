using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter.Editor
{

    /// <summary>
    /// static class for loading and saving data in json using the interface
    /// </summary>
    public static class ResourceManager
    {
        const string jsonextension = ".json";
        static readonly string empty = string.Empty;
        public static string ReadFromJson(UnityEngine.Object ob)
        {
            string json = string.Empty;
#if UNITY_EDITOR
            string savepath = UnityEditor.AssetDatabase.GetAssetPath(ob);

            json = ReadJson(savepath);
#endif
            return json;
        }

        public static string ReadJson(string savepath)
        {
            string json;
            using (System.IO.FileStream fs = new System.IO.FileStream(savepath, System.IO.FileMode.Open))
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(fs))
                {
                    json = reader.ReadToEnd();
                }
            }

            return json;
        }

        public static void SaveJsonFile(string json, string path, string name)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(path);
            sb.Append("/");
            sb.Append(name);
            sb.Append(jsonextension);

            string savepath = sb.ToString();
            Debug.Log(savepath);
            SaveJson(json, savepath);

#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
        }

        public static void SaveJson(string json, string savepath)
        {
            using (System.IO.FileStream fs = new System.IO.FileStream(savepath, System.IO.FileMode.Create))
            {
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(fs))
                {
                    writer.Write(json);
                }
            }
        }













    }
}
