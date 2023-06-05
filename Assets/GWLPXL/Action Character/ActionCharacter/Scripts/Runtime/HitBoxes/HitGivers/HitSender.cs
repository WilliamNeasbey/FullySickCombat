
using GWLPXL.ActionCharacter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{
    /// <summary>
    /// sends valid hits
    /// </summary>
    [System.Serializable]
    public class HitSender
    {
        public HitEvents DamageEvents = new HitEvents();
        public bool OnEnter = true;
        public bool OnStay = false;
        public bool OnExit = false;

        protected List<GameObject> hitOwners = new List<GameObject>();
        public int MaxPerAction = 10;
        public bool UseDebug = false;
        protected int hit = 0;
        protected int id = 0;
        public virtual void ResetDamageList(ActionContext actionname)
        {
            hitOwners.Clear();
            hit = 0;
        }
        public virtual void Setup(int ownerID)
        {
            id = ownerID;
           
        }

        protected virtual void DebugMessage(string message, UnityEngine.Object ctx)
        {
            if (UseDebug)
            {
                Debug.Log(message, ctx);
            }
        }
        public void SendDamage(HitContextCollision dmgctx)
        {
            GameObject hurtOwner = HitTakerManager.GetOwner(dmgctx.Collision.collider);
           
            if (hurtOwner == null)
            {
                DebugMessage("Null Hit", ActorHitBoxManager.GetHitBoxObject(id));
                return;
            }
            if (hitOwners.Contains(hurtOwner))
            {
                DebugMessage("Already hit can't hit again " + hurtOwner.name, ActorHitBoxManager.GetHitBoxObject(id));
                return;
            }
            if (hit > MaxPerAction)
            {
                DebugMessage("Reached Maximum Hits, can't hit again", ActorHitBoxManager.GetHitBoxObject(id));
                return;
            }


            DebugMessage("Damage Sent", ActorHitBoxManager.GetHitBoxObject(id));
            HitTakerManager.SendDamageEvent(dmgctx);

            DamageEvents.FireOnHitCollision(dmgctx);
            DamageEvents.UnityEvents.OnHitCollision?.Invoke(dmgctx);

            hit++;
            hitOwners.Add(hurtOwner);
            

        }
        public void SendDamage(HitContextTrigger dmgctx)
        {


            HitTakerManager.SendDamageEvent(dmgctx);
            DamageEvents.FireOnHitTrigger(dmgctx);
            DamageEvents.UnityEvents.OnHitTrigger?.Invoke(dmgctx);


        }
    }
}



