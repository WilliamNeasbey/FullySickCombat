using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{
    /// <summary>
    /// example
    /// to do, addon with a better designed system
    /// </summary>
    public class FreezeFrameSub : HitDamage
    {
        public float hitrate = .25f;
        public float FreezeRate = .2f;
        bool iframe = false;
        ActionTicker ticker = null;
        protected override void OnEnable()
        {
            base.OnEnable();
            ActionManager.OnActionStart += ActionInfo;
            ActionManager.OnActionComplete += ActionInfoRelease;
        }

        protected virtual void ActionInfo(ActionContext ctx)
        {
            if (ctx.Character.ID != GetComponent<ActionCharacter>().ID) return;
            ticker = ctx.Action;
        }
        protected virtual void ActionInfoRelease(ActionContext ctx)
        {
            if (ctx.Character.ID != GetComponent<ActionCharacter>().ID) return;
            ticker = null;
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            ActionManager.OnActionStart -= ActionInfo;
            ActionManager.OnActionComplete -= ActionInfoRelease;
        }
        protected override void DamageCollision(HitContextCollision ctx)
        {
            if (iframe == false && ticker != null)
            {
                StartCoroutine(ResetTime());
                StartCoroutine(HitRate());
                iframe = true;
            }
          
        }
        protected override void DamageTrigger(HitContextTrigger ctx)
        {
            if (iframe == false && ticker != null)
            {
                StartCoroutine(ResetTime());
                StartCoroutine(HitRate());
                iframe = true;
            }
        }

        IEnumerator HitRate()
        {
            iframe = true;
            yield return new WaitForSecondsRealtime(hitrate);
            iframe = false;
        }
        IEnumerator ResetTime()
        {
            ticker.Pause();
            yield return new WaitForSecondsRealtime(FreezeRate);
            ticker.UnPause();

        }

        
    }
}
