#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace GWLPXL.ActionCharacter.Editor
{


    public static class ProjectSetup 
    {

        [MenuItem("GWLPXL/ActionCharacter/Assign Layers")]
        static void Init()
        {
            //add the hitbox layer
            bool success = EditorUtility.DisplayDialog("Layers", "Assign demo layers for hitbox, ground, and wall", "Yes", "No, I'll do it myself.");
            if (success)
            {
                AddLayerIndex(DemoDefaults.HitboxLayerName, DemoDefaults.HitBoxLayer);
                AddLayerIndex(DemoDefaults.GroundLayerName, DemoDefaults.GroundLayer);
                AddLayerIndex(DemoDefaults.WallLayerName, DemoDefaults.WallLayer);
            }

        }
        public static void AddLayerIndex(string layerName, int index)
        {
            UnityEngine.Object[] asset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");

            if ((asset != null) && (asset.Length > 0))
            {
                SerializedObject serializedObject = new SerializedObject(asset[0]);
                SerializedProperty layers = serializedObject.FindProperty("layers");

                if (index > layers.arraySize) return;
        
                if (string.IsNullOrEmpty(layers.GetArrayElementAtIndex(index).stringValue))
                {
                    layers.GetArrayElementAtIndex(index).stringValue = layerName;
                    serializedObject.ApplyModifiedProperties();
                    serializedObject.Update();
                    if (layers.GetArrayElementAtIndex(index).stringValue == layerName)
                    {
                        return;     // to avoid unity locked layer
                    }
                }
                else
                {
                    string name = layers.GetArrayElementAtIndex(index).stringValue;
                    bool overwrite = EditorUtility.DisplayDialog("Overwrite layer?", "Overwrite layer " + name, "Yes", "No, I'll manually assign " + layerName);
                    if (overwrite)
                    {
                        layers.GetArrayElementAtIndex(index).stringValue = layerName;
                        serializedObject.ApplyModifiedProperties();
                        serializedObject.Update();
                        if (layers.GetArrayElementAtIndex(index).stringValue == layerName)
                        {
                            return;     // to avoid unity locked layer
                        }
                    }

                }

               

              
            }
        }
            public static void AddLayer(string layerName)
        {
            UnityEngine.Object[] asset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");

            if ((asset != null) && (asset.Length > 0))
            {
                SerializedObject serializedObject = new SerializedObject(asset[0]);
                SerializedProperty layers = serializedObject.FindProperty("layers");

                for (int i = 0; i < layers.arraySize; ++i)
                {
                    if (layers.GetArrayElementAtIndex(i).stringValue == layerName)
                    {
                        return;     // Layer already present, nothing to do.
                    }
                }

                //  layers.InsertArrayElementAtIndex(0);
                //  layers.GetArrayElementAtIndex(0).stringValue = layerName;


                for (int i = 19; i < layers.arraySize; i++)
                {
                    if (layers.GetArrayElementAtIndex(i).stringValue == "")
                    {
                        // layers.InsertArrayElementAtIndex(i);
                        layers.GetArrayElementAtIndex(i).stringValue = layerName;
                        serializedObject.ApplyModifiedProperties();
                        serializedObject.Update();
                        if (layers.GetArrayElementAtIndex(i).stringValue == layerName)
                        {
                            return;     // to avoid unity locked layer
                        }
                    }
                }
            }
        }
    }
}

#endif