using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GWLPXL.ActionCharacter
{
    /// <summary>
    /// clears hit box static dictionaries
    /// </summary>
    public static class ActorHitBoxManager
    {
#if UNITY_2019_3_OR_NEWER && UNITY_EDITOR // Introduced in 2019.3. Also can cause problems in builds so only for editor.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init()
        {
           
       actorHitBoxDic = new Dictionary<int, ActorHitBoxes>();//owner id, parent obj
       sceneRunner = null;
        hasSceneRunner = false;
    }
#endif
        public static event System.Action PersistSub;
        public static Dictionary<int, ActorHitBoxes> hitboxOwners => actorHitBoxDic;
        static Dictionary<int, ActorHitBoxes> actorHitBoxDic = new Dictionary<int, ActorHitBoxes>();//owner id, parent obj
        static ActorHitBoxSceneRunner sceneRunner = null;
        static bool hasSceneRunner = false;

        
        public static void Add(int id, ActorHitBoxes instance)
        {
            if (hasSceneRunner == false)
            {
                sceneRunner = GameObject.FindObjectOfType<ActorHitBoxSceneRunner>();
                if (sceneRunner == null)
                {
                    GameObject obj = new GameObject();
                    sceneRunner = obj.AddComponent<ActorHitBoxSceneRunner>();
                    obj.name = "Hit Box Scene Runner";
                }
                hasSceneRunner = true;
            }

            if (actorHitBoxDic.ContainsKey(id) == false)
            {
                actorHitBoxDic[id] = instance;
            }
        }
        public static void Remove(int id)
        {
            if (actorHitBoxDic.ContainsKey(id))
            {
                actorHitBoxDic.Remove(id);

            }
        }

        public static  GameObject GetHitBoxObject(int id)
        {
            if (actorHitBoxDic.ContainsKey(id))
            {
                return actorHitBoxDic[id].gameObject;
            }
            return null;
        }
        public static void ResetD(Scene scene, LoadSceneMode mode)
        {
            actorHitBoxDic.Clear();
            HitGiverManager.ClearDictionaries();
            HitBoxTeamManager.ClearDictionaries();
            HitTakerManager.ClearDictionaries();
            PersistSub?.Invoke();
            sceneRunner = null;
            hasSceneRunner = false;
            
        }
        public static void ResetD(Scene scene)
        {
            actorHitBoxDic.Clear();
            HitGiverManager.ClearDictionaries();
            HitBoxTeamManager.ClearDictionaries();
            HitTakerManager.ClearDictionaries();
            PersistSub?.Invoke();
            sceneRunner = null;
            hasSceneRunner = false;
        }

        
    }

    /// <summary>
    /// singleton to reset the hitbox manager on scene load
    /// </summary>
    public class ActorHitBoxSceneRunner : MonoBehaviour
    {
        public static ActorHitBoxSceneRunner Instance => instance;
        static ActorHitBoxSceneRunner instance;

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
                SceneManager.sceneLoaded += ActorHitBoxManager.ResetD;
                    }
 
        }


        protected virtual void OnDestroy()
        {
            SceneManager.sceneLoaded -= ActorHitBoxManager.ResetD;

        }

       



    }






}
