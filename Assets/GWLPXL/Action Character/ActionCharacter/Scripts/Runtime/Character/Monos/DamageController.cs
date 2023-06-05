using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace GWLPXL.ActionCharacter
{
   
    [Serializable]
    public class DamageValueTypes
    {
        [Tooltip("For Type damage. E.g. Actions - Combo_A_01 does 3 damage.")]
        public List<DamageWrapper> KeyValues = new List<DamageWrapper>();


    }
    [Serializable]
    public class ReductionValueTypes
    {

        [Tooltip("For Type damage. E.g. Actions - Combo_A_01 does 3 damage.")]
        public List<ReductionWrapper> KeyValues = new List<ReductionWrapper>();


    }
    public enum ReductionType
    {
        DefenderActions = 0,
        HurtBoxes = 1
    }
    public enum DamageType
    {
        AttackerActions = 0,
        HitBoxes = 1,
        InterpretedReactions = 2
    }

    [System.Serializable]
    public class DamageWrapper
    {
        public DamageType Type;
        public List<DamageKeyValue> Values = new List<DamageKeyValue>();
    }

    [Serializable]
    public class ReductionWrapper
    {
        public ReductionType Type;
        public List<ReduceKeyValue> Values = new List<ReduceKeyValue>();
    }
    [System.Serializable]
    public class DamageKeyValue
    {
        public string Key;
        public int Value;
    }

    [Serializable]
    public class ReduceKeyValue
    {
        public string Key;
        [Tooltip("Normalized percent, e.g. 1 = 100%, 0.5f = 50%")]
        [Range(0, 1f)]
        public float Value;
    }
    public interface ITakeDamage
    {
        public event Action<ActionCharacter> OnDied;
        public event Action<HitContextCollision> OnDiedCollision;
        public event Action<HitContextTrigger> OnDiedTrigger;
        public event Action<float> OnDamaged;//returns percent health
        bool IsDead { get; }
    }
  
    /// <summary>
    /// base damage controller
    /// </summary>
    public class DamageController : HitTakeDamage, ITakeDamage
    {
        #region interface ITakeDamage
        public event Action<ActionCharacter> OnDied;
        public event Action<HitContextCollision> OnDiedCollision;
        public event Action<HitContextTrigger> OnDiedTrigger;
        public event Action<float> OnDamaged;

        public bool IsDead => dead;
        #endregion

        public int MaxHP = 5;

        [Header("Damage Values")]
        [Tooltip("Take Damage values")]
        public DamageValueTypes TakeDamageValues = new DamageValueTypes();
        [Header("Damage Reduction Values")]
        [Tooltip("Any reduction modifiers, e.g. hit body versus hit leg.")]
        public ReductionValueTypes Reduction = new ReductionValueTypes();

        protected int current = 0;
        protected bool dead = false;

        protected ActionCharacter actionCharater;


        protected override void Awake()
        {
            base.Awake();
            actionCharater = GetComponent<ActionCharacter>();
            current = MaxHP;
            dead = false;
        }


        protected virtual void Start()
        {

            actionCharater.Events.OnSpawned += Setup;
        }
        protected virtual void OnDestroy()
        {
            if (actionCharater != null)
            {
                actionCharater.Events.OnSpawned -= Setup;
            }

        }
        protected virtual void Setup(CharacterArgs args)
        {

            dead = false;
            current = MaxHP;
            OnDamaged?.Invoke(1);
        }
      
        protected virtual int GetDamage(HitContextCollision ctx)
        {
            return CombatHelper.GetDamage(ctx, TakeDamageValues);
        }
        protected virtual int GetDamage(HitContextTrigger ctx)
        {
            return CombatHelper.GetDamage(ctx, TakeDamageValues);
        }

        protected virtual float GetReduction(HitContextCollision ctx)
        {
            return CombatHelper.GetReduction(ctx, Reduction);
        }
        protected virtual float GetReduction(HitContextTrigger ctx)
        {
            return CombatHelper.GetReduction(ctx, Reduction);
        }

        
        protected override void HurtCollision(HitContextCollision contextCollision)
        {
            if (dead)
            {
                Debug.Log("Cant dmg the dead");
                return;
            }

           
            string action = contextCollision.AttackerAction;
            int value = GetDamage(contextCollision);
            float reduction = GetReduction(contextCollision);
            value = Mathf.RoundToInt(value * (1 - reduction));

            current -= value;
            if (current <= 0)
            {
                dead = true;
            }
         
      
            if (value > 0)
            {
                OnDamaged?.Invoke((float)current / (float)MaxHP);
            }

            if (dead)
            {
                OnDied?.Invoke(actionCharater);
                OnDiedCollision?.Invoke(contextCollision);
            }
        }

      
        protected override void HurtTrigger(HitContextTrigger contextCollision)
        {
            if (dead)
            {
                Debug.Log("Cant dmg the dead");
                return;
            }
            string action = contextCollision.AttackAction;
            //  string interpreted = rc.InterpretHit(action);

            int value = GetDamage(contextCollision);
            float reduction = GetReduction(contextCollision);
            value = Mathf.RoundToInt(value * (1 - reduction));
            current -= value;
            if (current <= 0)
            {
                dead = true;
            }

            if (value > 0)
            {
                OnDamaged?.Invoke((float)current / (float)MaxHP);
            }

            if (dead)
            {
                OnDied?.Invoke(actionCharater);
                OnDiedTrigger?.Invoke(contextCollision);
            }
        }

        
    }
}
