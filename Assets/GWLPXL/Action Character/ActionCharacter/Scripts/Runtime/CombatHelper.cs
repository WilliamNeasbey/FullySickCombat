using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{
    public enum SearchType
    {
        None = 0,
        Closest = 1,
        Furthest = 2,
        Random = 3
    }
    public static class CombatHelper
    {
        
        public static Vector2 GetTargetLocation(Transform self, Transform other, AcquireTargetType type, AcquireType acquireType, out bool hasTarget)
        {
            Vector3 target = new Vector3(0, 0, 0);
            Vector3 dir = new Vector3(0, 0, 0);
            hasTarget = false;
            switch (type)
            {
                case AcquireTargetType.MoveToTarget:
                    switch (acquireType)
                    {
                        case AcquireType.ClosestSpot:
                            //
                            dir = self.position - other.position;
                            target = other.position + dir.normalized;
                            hasTarget = true;
                            break;
                        case AcquireType.FurthestSpot:
                            dir = other.position - self.position;
                            target = other.position + dir.normalized;
                            hasTarget = true;
                            break;
                        case AcquireType.TargetBack:
                            target = Detection.ConvertDirection(DirectionType.Back, other) + other.position;
                            hasTarget = true;
                            break;
                        case AcquireType.TargetFront:
                            target = Detection.ConvertDirection(DirectionType.Front, other) + other.position;
                            hasTarget = true;
                            break;
                        case AcquireType.TargetLeft:
                            target = Detection.ConvertDirection(DirectionType.Left, other) + other.position;
                            hasTarget = true;
                            break;
                        case AcquireType.TargetRight:
                            target = Detection.ConvertDirection(DirectionType.Right, other) + other.position;
                            hasTarget = true;
                            break;
                    }

                    break;
            }

            return target;
        }
        public static float GetReduction(HitContextCollision ctx, ReductionValueTypes values)
        {
            float value = 0;
            string key = ctx.AttackerAction;
            for (int i = 0; i < values.KeyValues.Count; i++)
            {
                ReductionType t = values.KeyValues[i].Type;
                List<ReduceKeyValue> reductions = values.KeyValues[i].Values;
                switch (t)
                {
                    case ReductionType.HurtBoxes:
                        key = ctx.DamagedPartName;
                        break;
                    case ReductionType.DefenderActions:
                        key = string.Empty;
                        GameObject owner = HitTakerManager.GetOwner(ctx.Collision.collider);
                        if (owner != null)
                        {
                            ActionCharacter ac = owner.GetComponent<ActionCharacter>();
                            if (ac != null)
                            {
                                key = ActionManager.GetActionName(ac.ID);
                                if (string.IsNullOrEmpty(key))
                                {
                                    continue;
                                }
                            }
                        }
                        break;

                }

                key = CommonFunctions.StringKey(key);
                for (int j = 0; j < reductions.Count; j++)
                {
                    string c = CommonFunctions.StringKey(reductions[j].Key);
                    if (CommonFunctions.WordEquals(key, c))
                    {
                        return reductions[j].Value;
                    }
                }

            }


            return value;
        }
        public static float GetReduction(HitContextTrigger ctx, ReductionValueTypes values)
        {
            float value = 0;
            string key = ctx.AttackAction;
            for (int i = 0; i < values.KeyValues.Count; i++)
            {
                ReductionType t = values.KeyValues[i].Type;
                List<ReduceKeyValue> reductions = values.KeyValues[i].Values;
                switch (t)
                {
                    case ReductionType.HurtBoxes:
                        key = ctx.HitPartName;
                        break;
                    case ReductionType.DefenderActions:
                        key = string.Empty;
                        GameObject owner = HitTakerManager.GetOwner(ctx.Other);
                        if (owner != null)
                        {
                            ActionCharacter ac = owner.GetComponent<ActionCharacter>();
                            if (ac != null)
                            {
                                key = ActionManager.GetActionName(ac.ID);
                                if (string.IsNullOrEmpty(key))
                                {
                                    continue;
                                }
                            }
                        }
                        break;
                       


                }

                key = CommonFunctions.StringKey(key);
                for (int j = 0; j < reductions.Count; j++)
                {
                    string c = CommonFunctions.StringKey(reductions[j].Key);
                    if (CommonFunctions.WordEquals(key, c))
                    {
                        return reductions[j].Value;
                    }
                }

            }


            return value;
        }
        public static int GetDamage(HitContextTrigger ctx, DamageValueTypes values)
        {
            int value = 0;
            string key = ctx.AttackAction;
            for (int i = 0; i < values.KeyValues.Count; i++)
            {
                DamageType t = values.KeyValues[i].Type;
                switch (t)
                {
                    case DamageType.AttackerActions:
                        key = ctx.AttackAction;
                        break;
                    case DamageType.HitBoxes:
                        key = ctx.AttackingPartName;
                        break;
                    case DamageType.InterpretedReactions:
                        Debug.Log("Use Reaction Character to use Interpreted Reactions");
                        continue;

                }

                key = CommonFunctions.StringKey(key);
                for (int j = 0; j < values.KeyValues[i].Values.Count; j++)
                {
                    string c = CommonFunctions.StringKey(values.KeyValues[i].Values[j].Key);
                    if (CommonFunctions.WordEquals(key, c))
                    {
                        return values.KeyValues[i].Values[j].Value;
                    }
                }

            }
            

            return value;
        }
        public static int GetDamage(HitContextCollision ctx, DamageValueTypes values)
        {
            int value = 0;
            string key = ctx.AttackerAction;
            for (int i = 0; i < values.KeyValues.Count; i++)
            {
                DamageType t = values.KeyValues[i].Type;
                switch (t)
                {
                    case DamageType.AttackerActions:
                        key = ctx.AttackerAction;
                        break;
                    case DamageType.HitBoxes:
                        key = ctx.AttackingPartName;
                        break;
                    case DamageType.InterpretedReactions:
                        Debug.Log("Use Reaction Character to use Interpreted Reactions");
                        continue;

                }

                key = CommonFunctions.StringKey(key);
                for (int j = 0; j < values.KeyValues[i].Values.Count; j++)
                {
                    string c = CommonFunctions.StringKey(values.KeyValues[i].Values[j].Key);
                    if (CommonFunctions.WordEquals(key, c))
                    {
                        return values.KeyValues[i].Values[j].Value;
                    }
                }
            }
                return value;
        }
        static Transform GetFurthest(List<HitCollider> hurtboxes, Transform origin, float maxRange, DirectionType[] filters, bool withlos)
        {
            float distance = 0;
            HitCollider nearest = null;
            for (int i = 0; i < hurtboxes.Count; i++)
            {
                if (hurtboxes[i].Collider.gameObject.activeInHierarchy == false) continue;

                Vector3 dir = hurtboxes[i].Collider.transform.position - origin.position;

                if (withlos)
                {
                    bool hassight = Detection.HasLineOfSight(origin, hurtboxes[i].Collider.gameObject.transform, maxRange);
                    if (hassight == false) continue;
                }

                if (dir.sqrMagnitude > distance && dir.sqrMagnitude < maxRange * maxRange)
                {
                    if (filters != null && filters.Length > 0)
                    {
                        for (int j = 0; j < filters.Length; j++)
                        {
                            if (filters[j] == DirectionType.Any)
                            {
                                distance = dir.sqrMagnitude;
                                nearest = hurtboxes[i];
                                break;
                            }
                            DirectionType t = Detection.DetectHitDirection(origin, hurtboxes[i].Collider.transform);
                            if (t == filters[j])
                            {
                                distance = dir.sqrMagnitude;
                                nearest = hurtboxes[i];
                                break;
                            }
                        }
                    }
                    else
                    {
                        distance = dir.sqrMagnitude;
                        nearest = hurtboxes[i];
                    }
                }
            }

            if (distance > 0)
            {
                GameObject owner = HitTakerManager.GetOwner(nearest.Collider);
                return owner.transform;
            }
            return null;
        }
        static Transform GetClosest(List<HitCollider> hurtboxes, Transform origin, float maxRange, DirectionType[] filters, bool withlos)
        {
            float distance = Mathf.Infinity;
            HitCollider nearest = null;
            for (int i = 0; i < hurtboxes.Count; i++)
            {
                if (hurtboxes[i].Collider.gameObject.activeInHierarchy == false) continue;

                Vector3 dir = hurtboxes[i].Collider.transform.position - origin.position;
                
                if (withlos)
                {
                    bool hassight = Detection.HasLineOfSight(origin, hurtboxes[i].Collider.gameObject.transform, maxRange);
                    if (hassight == false) continue;
                }
                if (dir.sqrMagnitude < distance || dir.sqrMagnitude < maxRange * maxRange)
                {
                    if (filters != null && filters.Length > 0)
                    {
                        for (int j = 0; j < filters.Length; j++)
                        {
                            if (filters[j] == DirectionType.Any)
                            {
                                distance = dir.sqrMagnitude;
                                nearest = hurtboxes[i];
                                break;
                            }
                            DirectionType t = Detection.DetectHitDirection(origin, hurtboxes[i].Collider.transform);
                            if (t == filters[j])
                            {
                                distance = dir.sqrMagnitude;
                                nearest = hurtboxes[i];
                                break;
                            }
                        }
                    }
                    else
                    {
                        distance = dir.sqrMagnitude;
                        nearest = hurtboxes[i];
                    }
                
                }
            }

            if (distance < Mathf.Infinity)
            {
                GameObject owner = HitTakerManager.GetOwner(nearest.Collider);
                return owner.transform;
            }
            return null;
        }
        static Transform GetRandom(List<HitCollider> hurtboxes, Transform origin, float maxRange, DirectionType[] filters, bool withlos)
        {
            List<HitCollider> _ = new List<HitCollider>();
            for (int i = 0; i < hurtboxes.Count; i++)
            {
                if (hurtboxes[i].Collider.gameObject.activeInHierarchy == false) continue;
                Vector3 dir = hurtboxes[i].Collider.transform.position - origin.position;

                if (withlos)
                {
                    bool hassight = Detection.HasLineOfSight(origin, hurtboxes[i].Collider.gameObject.transform, maxRange);
                    if (hassight == false) continue;
                }


                if (dir.sqrMagnitude < maxRange * maxRange)
                {
                    if (filters != null && filters.Length > 0)
                    {
                        for (int j = 0; j < filters.Length; j++)
                        {
                            if (filters[j] == DirectionType.Any)
                            {
                                _.Add(hurtboxes[i]);
                                break;
                            }
                            DirectionType t = Detection.DetectHitDirection(origin, hurtboxes[i].Collider.transform);
                            if (t == filters[j])
                            {
                                _.Add(hurtboxes[i]);
                                break;
                            }
                        }
                    }
                    else
                    {
                        _.Add(hurtboxes[i]);
                    }
                    _.Add(hurtboxes[i]);
                }
                
            }
            if (_.Count > 0)
            {
                int rando = Random.Range(0, _.Count);
                GameObject owner = HitTakerManager.GetOwner(_[rando].Collider);
                return owner.transform;
            }
            return null;
        }
        public static Transform GetTarget(SearchType type, Transform origin, int onTeam, float maxRange = Mathf.Infinity, bool withlos = true, DirectionType[] filters = null)
        {
            List<HitCollider> hurtboxes = HitBoxTeamManager.GetHitColliders(onTeam);
            switch (type)
            {
                case SearchType.Closest:
                    return GetClosest(hurtboxes, origin, maxRange, filters, withlos);
                case SearchType.Furthest:
                    return GetFurthest(hurtboxes, origin, maxRange, filters, withlos);
                case SearchType.Random:
                    return GetRandom(hurtboxes, origin, maxRange, filters, withlos);
            }
             

               
            return null;
        }
    }
}
