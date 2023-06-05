#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GWLPXL.ActionCharacter.Editor
{

    public static class SceneRunners 
    {

        public static class ActionRunner
        {
            [MenuItem("GWLPXL/ActionCharacter/Scene Runners/Action")]
            static void InitAction()
            {
                ActionSceneRunner runner = GameObject.FindObjectOfType<ActionSceneRunner>();
                if (runner != null)
                {
                    Debug.LogWarning("Already have scene runners in scene");
                    return;
                }

                GameObject newRunner = new GameObject();
                newRunner.AddComponent<ActionSceneRunner>();
                newRunner.name = "Action Scene Runner";

            }
        }
        
        public static class HitBoxRunner
        {
            [MenuItem("GWLPXL/ActionCharacter/Scene Runners/HitBoxes")]
            static void InitHitBoxes()
            {
                ActorHitBoxSceneRunner runner = GameObject.FindObjectOfType<ActorHitBoxSceneRunner>();
                if (runner != null)
                {
                    Debug.LogWarning("Already have scene runners in scene");
                    return;
                }

                GameObject newRunner = new GameObject();
                newRunner.AddComponent<ActorHitBoxSceneRunner>();
                newRunner.name = "Hit Box Scene Runner";

            }
        }
        
    }
}

#endif