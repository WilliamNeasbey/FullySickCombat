using System.Collections;
using UnityEngine.Events;

namespace GWLPXL.ActionCharacter
{


    [System.Serializable]
    public class HitEvents
    {
        public event System.Action<HitContextCollision> OnHitCollision;
        public event System.Action<HitContextTrigger> OnHitTrigger;

        public void FireOnHitCollision(HitContextCollision ctx)
        {
            OnHitCollision?.Invoke(ctx);
        }
        public void FireOnHitTrigger(HitContextTrigger ctx)
        {
            OnHitTrigger?.Invoke(ctx);
        }
        public UnityHurtCallbacks UnityEvents = new UnityHurtCallbacks();
    }


    [System.Serializable]
    public class UnityHurtCallbacks
    {
        public CollisionEvent OnHitCollision;
        public TriggerEvent OnHitTrigger;
    }
    [System.Serializable]
    public class CollisionEvent : UnityEvent<HitContextCollision>
    {
    }

    [System.Serializable]
    public class TriggerEvent : UnityEvent<HitContextTrigger>
    {
    }
}