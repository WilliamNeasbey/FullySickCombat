using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// lock on state
/// </summary>
/// 

namespace GWLPXL.ActionCharacter
{
    [System.Serializable]
    public enum LockStateType
    {
        None = 0,
        ClosestTargetContinuous = 100,
        ClosestTargetOnce = 101
    }

    [System.Serializable]
    public class LockStateVars
    {
        public LockStateType LockType = LockStateType.ClosestTargetContinuous;
        public float Radius = 1;
        public DirectionType[] Direction = new DirectionType[1] { DirectionType.Any };
        public List<int> TeamTargets = new List<int>(1) { 0 };
        public GameObject LockOnIndicatorPrefab;
        public GameObject LockOnColliderPrefab;
        public Vector3 LockColliderOffset = new Vector3(0, 1, 0);
        public Vector3 IndicatorOffset = new Vector3(0, 0, 0);
        public LayerMask Mask;

    }
    public class LockOnState : ITick
    {
        protected LockStateVars vars;
        protected bool Locked;
        protected List<Collider> possible = new List<Collider>();
        protected Transform Target;
        protected int index = 0;
        protected bool lockstate;
        protected float pulserate = .12f;
        protected float pulsetimer = 0;
        protected ActionCharacter ac;
        protected bool once;
        protected GameObject highlight;
        protected GameObject lockCollider;
        protected bool hashighlight = false;
        protected bool hasCollider = false;
        public LockOnState(ActionCharacter ac, LockStateVars vars)
        {
            this.ac = ac;
            this.vars = vars;
        }
        public void AddTicker()
        {
            if (vars.LockOnIndicatorPrefab != null)
            {
                highlight = SimplePool.Spawn(vars.LockOnIndicatorPrefab, ac.transform.position, vars.LockOnIndicatorPrefab.transform.rotation, false);
                highlight.SetActive(false);
            }

            if (vars.LockOnColliderPrefab != null)
            {
                lockCollider = vars.LockOnColliderPrefab;
                hasCollider = lockCollider != null;
            }
            hashighlight = highlight != null;
            ac.Events.FireLockStateEnter(new CharacterArgs(ac));
            TickManager.AddTicker(this);
        }

        public float GetTickDuration()
        {
            return Time.deltaTime;
        }

        public void RemoveTicker()
        {
            ac.SetLockOnTarget(null);
            if (hashighlight)
            {
                SimplePool.Despawn(highlight);
            }
    
            ac.Events.FireLockStateExit(new CharacterArgs(ac));
            TickManager.RemoveTicker(this);
        }

        public void Tick()
        {
            if (hashighlight)
            {
                if (ac.HasLockOnTarget)
                {
                    highlight.transform.SetParent(ac.LockOnTarget);
                    highlight.transform.localPosition = vars.IndicatorOffset;
                    highlight.SetActive(true);
                }
                else
                {
                    highlight.SetActive(false);
                    highlight.transform.SetParent(null);
                }

            }

            if (hasCollider && ac.HasLockOnTarget)
            {
                lockCollider.transform.position = ac.LockOnTarget.position + vars.LockColliderOffset;
            }

            switch (vars.LockType)
            {
                case LockStateType.ClosestTargetOnce:
                    if (once) return;
                    //hitboxes
                    for (int i = 0; i < vars.TeamTargets.Count; i++)
                    {
                        Transform target = CombatHelper.GetTarget(SearchType.Closest, ac.transform, vars.TeamTargets[i]);

                    }
                  
                    possible = Detection.GetLockOnTargets(ac.Transform, vars.Radius, vars.Mask, vars.Direction);
                    index = 0;
                    if (possible.Count > 0)
                    {
                        Target = possible[index].transform;
                        ac.SetLockOnTarget(Target);
                    }
                    once = true;
                    break;
                case LockStateType.ClosestTargetContinuous:
                    pulsetimer += Time.deltaTime;
                    
                    if (pulsetimer >= pulserate)
                    {
                        pulsetimer = 0;
                        possible = Detection.GetLockOnTargets(ac.Transform, vars.Radius, vars.Mask, vars.Direction);
                        index = 0;
                        if (possible.Count > 0)
                        {
                            Target = possible[index].transform;
                            ac.SetLockOnTarget(Target);
                            
              
                        }
                    }
                    break;


            }

        }
    }
}




