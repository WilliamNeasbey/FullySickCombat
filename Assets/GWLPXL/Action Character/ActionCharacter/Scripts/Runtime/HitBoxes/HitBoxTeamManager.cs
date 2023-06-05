
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{
    /// <summary>
    /// assigns boxes to teams, sets up ignore matrix.
    /// hit givers on same team ignore each other
    /// hit takers on same team ignore each other
    /// hit givers ignore takers on the same team
    /// hit takers ignore givers on the same team
    /// </summary>
    public static class HitBoxTeamManager
    {
#if UNITY_2019_3_OR_NEWER && UNITY_EDITOR // Introduced in 2019.3. Also can cause problems in builds so only for editor.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init()
        {
            hittakersTeams = new Dictionary<int, List<HitCollider>>();
            hitgiversTeams = new Dictionary<int, List<HitCollider>>();
            empty = new List<HitCollider>();
            locoColliders = new Dictionary<int, List<Collider>>();
            typeIgnore = new Dictionary<Collider, List<int>>();
            // Initialize your stuff here.
        }
#endif

        public static bool SendDebugMessage = false;
        public static Dictionary<int, List<HitCollider>> HittakersTeams => hittakersTeams;
        public static Dictionary<int, List<HitCollider>> HitGiversTeams => hitgiversTeams;
        static Dictionary<int, List<HitCollider>> hittakersTeams = new Dictionary<int, List<HitCollider>>();
        static Dictionary<int, List<HitCollider>> hitgiversTeams = new Dictionary<int, List<HitCollider>>();
        static List<HitCollider> empty = new List<HitCollider>();


        static Dictionary<int, List<Collider>> locoColliders = new Dictionary<int, List<Collider>>();
        static Dictionary<Collider, List<int>> typeIgnore = new Dictionary<Collider, List<int>>();

        public static void RemoveLocomotionCollider(Collider collider, int onTeam)
        {
            if (collider == null)
            {
                Debug.Log("No Locomotion Collider set");
                return;
            }

            List<Collider> _;
            if (locoColliders.ContainsKey(onTeam))
            {
                _ = locoColliders[onTeam];
                if (_.Contains(collider))
                {
                    _.Remove(collider);
                }
                locoColliders[onTeam] = _;
            }

            if (typeIgnore.ContainsKey(collider))
            {
                if (typeIgnore[collider] != null)
                {
                    foreach (var kvp in locoColliders)
                    {
                        if (typeIgnore[collider].Contains(kvp.Key))
                        {
                            for (int i = 0; i < kvp.Value.Count; i++)
                            {
                                Physics.IgnoreCollision(collider, kvp.Value[i], false);
                            }
                        }
                    }
                }

            }
            

           

        }

        
        public static void AddLocomotionCollider(Collider collider, int onTeam, List<int> ignoreTeams = null)
        {
            if (collider == null)
            {
                Debug.Log("No Locomotion Collider set");
                return;
            }

            if (ignoreTeams != null)
            {
                foreach (var kvp in locoColliders)
                {
                    if (ignoreTeams.Contains(kvp.Key))
                    {
                        for (int i = 0; i < kvp.Value.Count; i++)
                        {
                            Physics.IgnoreCollision(collider, kvp.Value[i], true);
                        }
                    }

                }
            }
            

           
            typeIgnore[collider] = ignoreTeams;


        }
        public static List<HitCollider> GetHitColliders(int onTeam)
        {
            if (hittakersTeams.ContainsKey(onTeam))
            {
                return hittakersTeams[onTeam];
            }
            return empty;
        }
        /// <summary>
        /// returns the owner of the hitbox
        /// </summary>
        /// <param name="onTeam"></param>
        /// <param name="pos"></param>
        /// <param name="isActive"></param>
        /// <returns></returns>
        public static GameObject FindNearest(int onTeam, Vector3 pos, bool isActive = true)
        {
            if (hittakersTeams.ContainsKey(onTeam))
            {
                List<HitCollider> hurtboxes = hittakersTeams[onTeam];

                float distance = Mathf.Infinity;
                HitCollider nearest = null;
                for (int i = 0; i < hurtboxes.Count; i++)
                {
                    if (isActive)
                    {
                        if (hurtboxes[i].Collider.gameObject.activeInHierarchy == false) continue;
                    }


                    Vector3 dir = hurtboxes[i].Collider.transform.position - pos;
                    if (dir.sqrMagnitude < distance)
                    {

                        distance = dir.sqrMagnitude;
                        nearest = hurtboxes[i];
                    }
                }

                if (distance < Mathf.Infinity)
                {
                    GameObject owner = HitTakerManager.GetOwner(nearest.Collider);
                    return owner;
                }

            }
            return null;
        }
        public static void ClearDictionaries()
        {
            foreach (var kvp in hittakersTeams)
            {
                kvp.Value.Clear();

            }
            foreach (var kvp in hitgiversTeams)
            {
                kvp.Value.Clear();
            }
            hittakersTeams.Clear();
            hitgiversTeams.Clear();
            locoColliders.Clear();
        }
        public static void RemoveHitGiversFromTeam(HitCollider box, int team)
        {
            RemoveHitBox(hitgiversTeams, box, team);
            ReEnableCollisions(hittakersTeams, box, team);
        }
        public static void RemoveHitTakersFromTeam(HitCollider box, int team)
        {
            RemoveHitBox(hittakersTeams, box, team);
            ReEnableCollisions(hitgiversTeams, box, team);

        }
        public static void AddHitTakersToTeam(HitCollider box, int team)
        {

            AddHitTakerToTeam(hittakersTeams, box, team);

            //ignore all hit givers on our own team
            IgnoreCollisionsTeam(hitgiversTeams, box, team);
        }
        public static void AddHitGiversToTeam(HitCollider box, int team)
        {
            AddHitTakerToTeam(hitgiversTeams, box, team);

            //ignore all hit takers on our own team
            IgnoreCollisionsTeam(hittakersTeams, box, team);
        }


        static void RemoveHitBox(Dictionary<int, List<HitCollider>> tracking, HitCollider box, int team)
        {
            //remove from dic

            if (tracking.ContainsKey(team))
            {
                List<HitCollider> list = tracking[team];
                if (list.Contains(box))
                {
                    list.Remove(box);
                }

                tracking[team] = list;
            }

            //ignore everyone on the team
            IgnoreCollisionsTeam(tracking, box, team);



        }

        static void AddHitTakerToTeam(Dictionary<int, List<HitCollider>> tracking, HitCollider box, int team)
        {
            //add
            if (tracking.ContainsKey(team) == false)
            {
                tracking.Add(team, new List<HitCollider>(1) { box });
            }
            else
            {
                List<HitCollider> list = tracking[team];
                list.Add(box);
                tracking[team] = list;
            }

            //ignore everyone on the team
            IgnoreCollisionsTeam(tracking, box, team);

        }

        static void IgnoreCollisionsTeam(Dictionary<int, List<HitCollider>> tracking, HitCollider box, int team)
        {
            foreach (var kvp in tracking)
            {
                if (kvp.Key == team)//same team ignore colliders, can't hit self
                {
                    List<HitCollider> _ = kvp.Value;
                    for (int i = 0; i < _.Count; i++)
                    {
                        if (_[i] == null || _[i].Collider == null)
                        {
                            _.RemoveAt(i);
                            continue;
                        }

                        Collider coll = _[i].Collider;
                        Physics.IgnoreCollision(box.Collider, coll, true);
                    }

                }
            }
        }

        static void ReEnableCollisions(Dictionary<int, List<HitCollider>> tracking, HitCollider box, int team)
        {
            foreach (var kvp in tracking)
            {
                if (kvp.Key == team)
                {
                    List<HitCollider> _ = kvp.Value;
                    for (int i = 0; i < _.Count; i++)
                    {
                        Collider coll = _[i].Collider;
                        if (coll == null) continue;
                        Physics.IgnoreCollision(box.Collider, coll, false);
                    }
                }
            }
        }
    }
}