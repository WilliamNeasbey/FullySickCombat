

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{

    /// <summary>
    /// base mono that subs to hit taker on hurt collision and trigger events
    /// </summary>
    public abstract class HitTakeDamage : MonoBehaviour
    {
      
        [SerializeField]
        protected bool debug = false;

        protected ActorHitBoxes boxes;
      
        protected virtual void Awake()
        {
            boxes = GetComponent<ActorHitBoxes>();
        }


        protected virtual void OnEnable()
        {
            SubManager.SubTakeHit(boxes, HurtCollision);
            SubManager.SubTakeHit(boxes, HurtTrigger);
        }

        protected virtual void OnDisable()
        {
            SubManager.UnSubTakeHit(boxes, HurtCollision);
            SubManager.UnSubTakeHit(boxes, HurtTrigger);

        }

        protected virtual void HurtCollision(HitContextCollision contextCollision) 
        {
            if (contextCollision.Attacker != null)
            {
                DebugMessage(contextCollision.Attacker.name);
            }
           
        }

        protected virtual void HurtTrigger(HitContextTrigger contextCollision) 
        {
            if (contextCollision.Attacker != null)
            {
                DebugMessage(contextCollision.Attacker.name);
            }
          
        }

        protected void DebugMessage(string message)
        {
            if (debug)
            {
                Debug.Log(message);
            }
        }

    }
}