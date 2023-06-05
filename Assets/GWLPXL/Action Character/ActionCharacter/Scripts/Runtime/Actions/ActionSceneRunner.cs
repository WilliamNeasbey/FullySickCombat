using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GWLPXL.ActionCharacter
{


    public class ActionSceneRunner : MonoBehaviour
    {
        public bool UseDebug = false;
        public static ActionSceneRunner Instance => instance;
        static ActionSceneRunner instance;

        protected virtual void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);

            }
            else
            {
                DontDestroyOnLoad(this.gameObject);
                instance = this;
                SceneManager.sceneLoaded += ResetD;
            }

        }

        protected virtual void Update()
        {
            if (ActionManager.SendDebugMessage != UseDebug)
            {
                ActionManager.SendDebugMessage = UseDebug;
            }
        }
        protected virtual void ResetD(Scene scene, LoadSceneMode mode)
        {
            ActionManager.Clear();

        }

        protected virtual void OnDestroy()
        {
            SceneManager.sceneLoaded -= ActorHitBoxManager.ResetD;

        }
    }
}