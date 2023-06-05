using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{
    public class ReturnToPool : MonoBehaviour
    {
        public float Lifetime = 3;
        protected Timer timer;

        protected virtual void Awake()
        {

        }
        protected virtual void OnEnable()
        {

        }

       
        public virtual void StartTimer()
        {
            if (timer != null)
            {
                StopTimer();
            }

            timer = new Timer(Lifetime, ReturnMe);
            timer.AddTicker();
        }

        protected virtual void Start()
        {

        }

        protected virtual void OnDisable()
        {
            StopTimer();
        }

        public virtual void StopTimer()
        {
            if (timer != null) timer.RemoveTicker();
            timer = null;
        }

        protected virtual void ReturnMe()
        {
            SimplePool.Despawn(this.gameObject);
            StopTimer();
        }
    }
}
