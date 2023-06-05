

using UnityEngine;

namespace GWLPXL.ActionCharacter
{
    /// <summary>
    /// base action ticker class, ActionTickerCC implements and overrides
    /// </summary>
    public class ActionTicker : ITick
    {
        public event System.Action OnExit;
        public float Percent => percent;

        protected bool allowComboExit = false;
        protected float percent = 0;
        protected ActionCharacter instance = null;
        protected ActionVars vars = null;
        protected bool paused = false;
        protected float timerMulti = 1f;
        public ActionTicker(ActionCharacter instance, ActionVars vars)
        {
            percent = 0;
            this.instance = instance;
            this.vars = vars;
        }

        public virtual void AllowComboExit(bool allow)
        {
            allowComboExit = allow;
        }
        public virtual void AddTicker()
        {
            TickManager.AddTicker(this);
        }

        public virtual float GetTickDuration()
        {
            return Time.deltaTime;
        }

        public virtual void RemoveTicker()
        {
            TickManager.RemoveTicker(this);
            OnExit?.Invoke();
        }

        public virtual void Pause()
        {
            paused = true;
            timerMulti = 0;
        }

        public virtual void UnPause()
        {
            paused = false;
            timerMulti = 1;
        }
        public virtual void SetTimerMulti(float newMulit)
        {
            timerMulti = newMulit;
        }
        public virtual void ResetActionObject()
        {

        }
        public virtual void Tick()
        {
            
        }
    }


}