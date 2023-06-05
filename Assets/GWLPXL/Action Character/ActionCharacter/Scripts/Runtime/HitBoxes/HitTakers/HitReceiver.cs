using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{

    /// <summary>
    /// Receives the hit
    /// </summary>
    [System.Serializable]
    public class HitReceiver
    {

        protected int id = 0;
        public HitEvents HitTakerEvents = new HitEvents();

        public bool UseDebug = false;
        public virtual void Setup(int ownderID)
        {
         
            id = ownderID;
        }

        protected virtual void DebugMessage(string message, UnityEngine.Object ctx = null)
        {
            if (UseDebug)
            {
                Debug.Log(message, ctx);
            }
        }

        /// <summary>
        /// receives results of hit giver and hit taker. sends as event. Subscribe to event if you want to pass it along to your own health scripts
        /// </summary>
        /// <param name="dmg"></param>
        public virtual void ReceiveHit(HitContextCollision dmg)
        {

            HitCollider area = HitTakerManager.GetHitTaker(id, dmg.Collision.collider);

            GameObject owner = ActorHitBoxManager.GetHitBoxObject(id);
            DebugMessage("Area " + area.Name, owner);
            dmg.DamagedPartName = area.Name;

            
            HitTakerEvents.FireOnHitCollision(dmg);
            HitTakerEvents.UnityEvents.OnHitCollision?.Invoke(dmg);



        }

        public virtual void ReceiveHit(HitContextTrigger dmg)
        {

            HitCollider area = HitTakerManager.GetHitTaker(id, dmg.Other);
            dmg.HitPartName = area.Name;
            GameObject owner = ActorHitBoxManager.GetHitBoxObject(id);
            DebugMessage("Area " + area.Name, owner);
            HitTakerEvents.FireOnHitTrigger(dmg);
            HitTakerEvents.UnityEvents.OnHitTrigger?.Invoke(dmg);



        }
    }
}