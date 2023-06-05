using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{

    /// <summary>
    /// inherit and override damage collision / damage trigger 
    /// </summary>
    public abstract class HitDamage : MonoBehaviour
    {
        [SerializeField]
        protected ActorHitBoxes hitboxes;

       
        protected virtual void Awake()
        {
            if (hitboxes == null)
            {
                hitboxes = GetComponent<ActorHitBoxes>();
            }

        }

        protected virtual void OnEnable()
        {
            SubManager.SubGiveDamage(hitboxes, DamageCollision);
            SubManager.SubGiveDamage(hitboxes, DamageTrigger);

        }

        protected virtual void OnDisable()
        {
            SubManager.UnSubGiveDamage(hitboxes, DamageCollision);
            SubManager.UnSubGiveDamage(hitboxes, DamageTrigger);
        }

        protected virtual void DamageCollision(HitContextCollision ctx)
        {

        }


        protected virtual void DamageTrigger(HitContextTrigger ctx)
        {

        }
       
    }
}