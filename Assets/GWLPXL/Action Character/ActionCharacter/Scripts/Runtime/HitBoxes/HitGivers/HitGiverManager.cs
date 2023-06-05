using System.Collections.Generic;
using UnityEngine;


namespace GWLPXL.ActionCharacter

{
    /// <summary>
    /// Static Manager to track the HitGivers
    /// </summary>

    public static class HitGiverManager
    {
#if UNITY_2019_3_OR_NEWER && UNITY_EDITOR // Introduced in 2019.3. Also can cause problems in builds so only for editor.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init()
        {

            damagersDic = new Dictionary<int, HitGivers>();
          colliderOwnersDic = new Dictionary<Collider, HitGivers>();
            owner = null;
        }
#endif
        //transform id
        static Dictionary<int, HitGivers> damagersDic = new Dictionary<int, HitGivers>();
        static Dictionary<Collider, HitGivers> colliderOwnersDic = new Dictionary<Collider, HitGivers>();
        static GameObject owner = null;
        public static void ClearDictionaries()
        {
            damagersDic.Clear();
        }
        public static bool IsOwner(int key, Collider collider)
        {
            if (damagersDic.ContainsKey(key))
            {
                HitGivers givers = damagersDic[key];
                return givers.IsOwner(collider);
            }
        
            return false;
        }

        public static GameObject GetOwner(Collider collider)
        {
            owner = null;
            if (colliderOwnersDic.ContainsKey(collider))
            {
                HitGivers takers = colliderOwnersDic[collider];
                int id = takers.OwnerID;
                owner = ActorHitBoxManager.GetHitBoxObject(id);
            }


            return owner;

        }

        public static void AddNewCollider(Collider collider, HitGivers owner)
        {
            if (colliderOwnersDic.ContainsKey(collider) == false)
            {
                colliderOwnersDic[collider] = owner;
            }
        }

        public static void RemoveCollider(Collider collider)
        {
            if (colliderOwnersDic.ContainsKey(collider) == true)
            {
                colliderOwnersDic.Remove(collider);
            }
        }

        /// <summary>
        /// key is owner id, area is name of hitboxes
        /// </summary>
        /// <param name="key"></param>
        /// <param name="area"></param>
        public static void EnableHitGivers(int key, List<string> vars)
        {
            for (int i = 0; i < vars.Count; i++)
            {
                EnableHitGiver(key, vars[i]);
            }
        }
        /// <summary>
        /// key is owner id, area is name of hitboxes
        /// </summary>
        /// <param name="key"></param>
        /// <param name="area"></param>
        public static void DisableHitGivers(int key, List<string> vars)
        {
            for (int i = 0; i < vars.Count; i++)
            {
                DisableHitGiver(key, vars[i]);
            }

        }
        /// <summary>
        /// key is owner id, area is name of hitboxes
        /// </summary>
        /// <param name="key"></param>
        /// <param name="area"></param>
        public static void DisableHitGiver(int key, string area)
        {
            if (damagersDic.ContainsKey(key))
            {
                HitGivers assigner = damagersDic[key];
                assigner.DisableHitGiver(area);
            }

        }
        /// <summary>
        /// key is owner id, area is name of hitboxes
        /// </summary>
        /// <param name="key"></param>
        /// <param name="area"></param>
        public static void EnableHitGiver(int key, string area)
        {

            if (damagersDic.ContainsKey(key))
            {
                HitGivers assigner = damagersDic[key];
                assigner.EnableHitGiver(area);
            }

        }
        public static HitGivers GetHitGiverBoxes(int ownerID)
        {
            if (damagersDic.ContainsKey(ownerID))
            {
                return damagersDic[ownerID];
            }

            //try to place it in there.
            return null;
        }

        public static void Register(int ownerid, HitGivers assigner)
        {
            if (damagersDic.ContainsKey(ownerid) == false)
            {
                damagersDic[ownerid] = assigner;
            }

        }

        public static void UnRegister(int ownerID)
        {
            if (damagersDic.ContainsKey(ownerID))
            {
                damagersDic.Remove(ownerID);
            }
        }
    }
}