using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{
    /// <summary>
    /// Static Manager to track the Hurt Boxes, hitboxes that receive damage
    /// </summary>

    public static class HitTakerManager
    {
#if UNITY_2019_3_OR_NEWER && UNITY_EDITOR // Introduced in 2019.3. Also can cause problems in builds so only for editor.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init()
        {

           damagersDic = new Dictionary<int, HitTakers>();
             colliderOwnersDic = new Dictionary<Collider, HitTakers>();
           owner = null;
             empty = new List<HitCollider>(0);
        }
#endif
        static Dictionary<int, HitTakers> damagersDic = new Dictionary<int, HitTakers>();
        static Dictionary<Collider, HitTakers> colliderOwnersDic = new Dictionary<Collider, HitTakers>();
        static GameObject owner = null;
        static List<HitCollider> empty = new List<HitCollider>(0);

        public static void ClearDictionaries()
        {
            damagersDic.Clear();
            colliderOwnersDic.Clear();
        }

        //this is returning null..
        public static GameObject GetOwner(Collider collider)
        {
            owner = null;
            if (colliderOwnersDic.ContainsKey(collider))
            {
                HitTakers takers = colliderOwnersDic[collider];
                int id = takers.OwnerID;
                owner = ActorHitBoxManager.GetHitBoxObject(id);
            }


            return owner;

        }

       

        public static string GetHitTakerName(int owner, Collider collider)
        {
            if (damagersDic.ContainsKey(owner))
            {
                return damagersDic[owner].GetHitTakerName(collider);
            }
            return string.Empty;
        }
        public static bool IsOwner(int owner, Collider collider)
        {
            if (damagersDic.ContainsKey(owner))
            {
                return damagersDic[owner].IsOwner(collider);
            }
            return false;
        }
        public static void SendDamageEvent(HitContextCollision hurt)
        {
            if (colliderOwnersDic.ContainsKey(hurt.Collision.collider))
            {
                HitTakers takers = colliderOwnersDic[hurt.Collision.collider];
                takers.HitReceiver.ReceiveHit(hurt);

            }
        }
        public static void SendDamageEvent(HitContextTrigger hurt)
        {
            if (colliderOwnersDic.ContainsKey(hurt.Other))
            {

                HitTakers takers = colliderOwnersDic[hurt.Other];
                takers.HitReceiver.ReceiveHit(hurt);


            }
        }
        public static void AddNewCollider(Collider collider, HitTakers owner)
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

        public static HitCollider GetHitTaker(int key, Collider area)
        {
            if (damagersDic.ContainsKey(key))
            {
                return damagersDic[key].GetHitTaker(area);
            }

            return new HitCollider("NULL", null);
        }
        public static List<HitCollider> GetHitTaker(int key, string area)
        {
            if (damagersDic.ContainsKey(key))
            {
                return damagersDic[key].GetHitTaker(area);
            }

            return empty;
        }
        public static void EnableHitTakers(int key, List<string> vars)
        {
            for (int i = 0; i < vars.Count; i++)
            {
                EnableHitTaker(key, vars[i]);
            }
        }
        public static void DisableHitTakers(int key, List<string> vars)
        {
            for (int i = 0; i < vars.Count; i++)
            {
                DisableHitTaker(key, vars[i]);
            }

        }
        public static void DisableHitTaker(int key, string area)
        {
            if (damagersDic.ContainsKey(key))
            {
                HitTakers assigner = damagersDic[key];
                assigner.DisableHitTaker(area);
            }

        }
        public static void EnableHitTaker(int key, string area)
        {

            if (damagersDic.ContainsKey(key))
            {
                HitTakers assigner = damagersDic[key];
                assigner.EnableHitTaker(area);
            }

        }
        public static HitTakers GetHitTakers(int ownerID)
        {
            if (damagersDic.ContainsKey(ownerID))
            {
                return damagersDic[ownerID];
            }

            //try to place it in there.
            return null;
        }
        public static void Register(int ownerID, HitTakers assigner)
        {
            if (damagersDic.ContainsKey(ownerID) == false)
            {
                damagersDic[ownerID] = assigner;
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