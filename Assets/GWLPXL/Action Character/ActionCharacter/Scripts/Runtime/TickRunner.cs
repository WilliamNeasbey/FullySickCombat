﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GWLPXL.ActionCharacter
{


    #region mono
    /// <summary>
    /// tick manager mono, used to wrap all update or fixed update methods into one object call. 
    /// Use the interface ITick to add new classes to the tickrunner, then addtick/remove tick as needed
    /// </summary>
    public class TickRunner : MonoBehaviour
    {

        int count = TickManager.TickersArr.Count;
        int unscaled = TickManager.UnscaledTickersArr.Count;
        int phys = TickManager.PhysicsTickersARr.Count;
        protected bool debug = true;
        protected bool safe = false;
       protected virtual void OnDestroy()
        {
            safe = false;
            TickManager.Clear();
        }

        protected virtual void Start()
        {
            safe = true;
        }
        protected virtual void Update()
        {
            TickUpdateBehavior();

        }
        protected virtual void FixedUpdate()
        {
            TickFixedUpdateBehavior();
        }

        protected virtual void TickUpdateBehavior()
        {
            if (TickManager.IsFrozen() || safe == false) return;

            for (int i = TickManager.UnscaledTickersArr.Count - 1; i >= 0; i--)
            {
                TickManager.UnscaledTickersArr[i].currentTimer += Time.unscaledDeltaTime;
                if (TickManager.UnscaledTickersArr[i].currentTimer >= TickManager.UnscaledTickersArr[i].TickDuration)
                {
                    TickManager.UnscaledTickersArr[i].currentTimer = 0;
                    TickManager.UnscaledTickersArr[i].Ticker.UnscaledTick();
                }
            }


            for (int i = TickManager.TickersArr.Count - 1; i >= 0; i--)
            {
                TickManager.TickersArr[i].currentTimer += Time.deltaTime;
                if (debug)
                {
                    Type objectType = TickManager.TickersArr[i].Ticker.GetType();
                    Debug.Log(objectType.Name);
                }
                if (TickManager.TickersArr[i].currentTimer >= TickManager.TickersArr[i].TickDuration)
                {
                    TickManager.TickersArr[i].currentTimer -= 0;
                    TickManager.TickersArr[i].Ticker.Tick();

                    
                }

            }
        }



        protected virtual void TickFixedUpdateBehavior()
        {
            if (TickManager.IsFrozen() || safe == false) return;

            for (int i = TickManager.PhysicsTickersARr.Count - 1; i >= 0; i--)
            {
                TickManager.PhysicsTickersARr[i].currentTimer += Time.fixedDeltaTime;
                if (TickManager.PhysicsTickersARr[i].currentTimer >= TickManager.PhysicsTickersARr[i].TickDuration)
                {
                    TickManager.PhysicsTickersARr[i].currentTimer = 0;
                    TickManager.PhysicsTickersARr[i].Ticker.PhysicsTick();
                }
            }
        }
    }

    #endregion
    /// <summary>
    /// class for tickers with unscaled time
    /// </summary>
    [System.Serializable]
    public class ModifyTimeScale : IUnscaledTick
    {
        AnimationCurve timecurve;
        float endtimescale;
        float duration;
        float starttimescale;
        float timer = 0;
        float original;
        bool start = false;
        public ModifyTimeScale(float endtimescale, float duration, float starttimescale, AnimationCurve timecurve)
        {
            this.original = Time.timeScale;
            this.endtimescale = endtimescale;
            this.duration = duration;
            this.starttimescale = starttimescale;
            Time.timeScale = this.endtimescale;
            this.timecurve = timecurve;
        }

        public void AddTicker()
        {
            if (start == false)//only allow one per instance
            {
                timer = 0;
                start = true;
                TickManager.AddTicker(this);
            }

        }

        public float GetTickDuration() => Time.unscaledDeltaTime;


        public void RemoveTicker()
        {
            if (start == true)
            {
                Time.timeScale = original;
                start = false;
                TickManager.RemoveTicker(this);
            }

        }

        public void UnscaledTick()
        {
            timer += Time.unscaledDeltaTime;
            if (timer > duration)
            {
                RemoveTicker();
                return;
            }


            float percent = timer / duration;
            float lerp = percent;
            if (timecurve != null)
            {
                lerp = Mathf.Lerp(starttimescale, endtimescale, timecurve.Evaluate(percent));
            }
            Time.timeScale = lerp;




        }



    }

    /// <summary>
    /// interface for unscaled tickers
    /// </summary>
    public interface IUnscaledTick
    {
        void AddTicker();
        void RemoveTicker();
        void UnscaledTick();
        float GetTickDuration();
    }

    /// <summary>
    /// interface for update ticks
    /// </summary>
    public interface ITick
    {
        void AddTicker();
        void RemoveTicker();
        void Tick();
        float GetTickDuration();
    }
    /// <summary>
    /// interface for pyhsics ticks (fixed update)
    /// </summary>
    public interface IPhysicsTick
    {
        void AddTicker();
        void RemoveTicker();
        void PhysicsTick();
        float GetTickDuration();
    }

    public class TickerUnscaledInstance
    {
        public float TickDuration;
        public IUnscaledTick Ticker;
        public float currentTimer;

        public TickerUnscaledInstance(float duration, IUnscaledTick ticker)
        {
            TickDuration = duration;
            currentTimer = 0;
            Ticker = ticker;
        }
    }

    public class TickerInstance
    {
        public float TickDuration;
        public ITick Ticker;
        public float currentTimer;

        public TickerInstance(float duration, ITick ticker)
        {
            TickDuration = duration;
            currentTimer = 0;
            Ticker = ticker;
        }
    }
    public class TickerFixedUpdateInstance
    {
        public float TickDuration;
        public IPhysicsTick Ticker;
        public float currentTimer;

        public TickerFixedUpdateInstance(float duration, IPhysicsTick ticker)
        {
            TickDuration = duration;
            currentTimer = 0;
            Ticker = ticker;
        }
    }
    /// <summary>
    /// sealed class that all tickers register and remove themselves from. 
    /// will create a tick runner on first use of ITick
    /// </summary>
    public sealed class TickManager
    {
        #region singleton
        public static TickManager Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                        instance = new TickManager();
                    return instance;
                }
            }
        }

#if UNITY_2019_3_OR_NEWER && UNITY_EDITOR // Introduced in 2019.3. Also can cause problems in builds so only for editor.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init()
        {
         instance = null;
                   tickersunscaledic = new Dictionary<IUnscaledTick, TickerUnscaledInstance>();
         tickersdic = new Dictionary<ITick, TickerInstance>();
       physicsdic = new Dictionary<IPhysicsTick, TickerFixedUpdateInstance>();
         physicsTickersAr = new List<TickerFixedUpdateInstance>();
        unscaledArr = new List<TickerUnscaledInstance>();
         tickersArr = new List<TickerInstance>();
         frozen = false;
    }
#endif
        static TickManager instance = null;
        static readonly object padlock = new object();
        #endregion
        public static Dictionary<IUnscaledTick, TickerUnscaledInstance> tickersunscaledic = new Dictionary<IUnscaledTick, TickerUnscaledInstance>();
        public static Dictionary<ITick, TickerInstance> tickersdic = new Dictionary<ITick, TickerInstance>();
        public static Dictionary<IPhysicsTick, TickerFixedUpdateInstance> physicsdic = new Dictionary<IPhysicsTick, TickerFixedUpdateInstance>();
        static List<TickerFixedUpdateInstance> physicsTickersAr = new List<TickerFixedUpdateInstance>();

        static List<TickerUnscaledInstance> unscaledArr = new List<TickerUnscaledInstance>();
        static List<TickerInstance> tickersArr = new List<TickerInstance>();
        public static List<TickerInstance> TickersArr => tickersArr;
        public static List<TickerUnscaledInstance> UnscaledTickersArr => unscaledArr;
        public static List<TickerFixedUpdateInstance> PhysicsTickersARr => physicsTickersAr;
        static bool frozen = false;
        static TickRunner tickrunnner = null;
        //static List<ITick> tickers = new List<ITick>();
        TickManager()
        {

        }


      
        public static void Clear()
        {
            for (int i = unscaledArr.Count - 1; i >= 0; i--)
            {
                unscaledArr[i].Ticker.RemoveTicker();
               
            }



            for (int i = tickersArr.Count - 1; i >= 0; i--)
            {
                tickersArr[i].Ticker.RemoveTicker();

            }


            for (int i = physicsTickersAr.Count - 1; i >= 0; i--)
            {
                physicsTickersAr[i].Ticker.RemoveTicker();

            }


            unscaledArr.Clear();
            tickersArr.Clear();
            physicsTickersAr.Clear();

        }
        public static bool IsFrozen() => frozen;
        public static void FreezeTicks(bool isForzen) => frozen = isForzen;

        public static void AddTicker(IPhysicsTick ticker)
        {
            if (physicsdic.ContainsKey(ticker) == false)
            {

                TickerFixedUpdateInstance newinstance = new TickerFixedUpdateInstance(ticker.GetTickDuration(), ticker);
                physicsdic[ticker] = newinstance;
                physicsTickersAr.Add(newinstance);


                if (tickrunnner == null)
                {
                    GameObject sceneObject = new GameObject();
                    sceneObject.name = "TickRunner";
                    tickrunnner = sceneObject.AddComponent<TickRunner>();

                }
            }
        }

        public static void AddTicker(IUnscaledTick ticker)
        {
            if (tickersunscaledic.ContainsKey(ticker) == false)
            {

                TickerUnscaledInstance newinstance = new TickerUnscaledInstance(ticker.GetTickDuration(), ticker);
                tickersunscaledic[ticker] = newinstance;
                unscaledArr.Add(newinstance);


                if (tickrunnner == null)
                {
                    GameObject sceneObject = new GameObject();
                    sceneObject.name = "TickRunner";
                    tickrunnner = sceneObject.AddComponent<TickRunner>();
                }
            }
        }
        public static void AddTicker(ITick ticker)
        {
            if (tickersdic.ContainsKey(ticker) == false)
            {

                TickerInstance newinstance = new TickerInstance(ticker.GetTickDuration(), ticker);
                tickersdic[ticker] = newinstance;
                tickersArr.Add(newinstance);


                if (tickrunnner == null)
                {
                    GameObject sceneObject = new GameObject();
                    sceneObject.name = "TickRunner";
                    tickrunnner = sceneObject.AddComponent<TickRunner>();
                }

            }



        }

        public static void RemoveTicker(ITick ticker)
        {
            if (tickersdic.ContainsKey(ticker))
            {
                tickersArr.Remove(tickersdic[ticker]);
                tickersdic.Remove(ticker);

            }
        }

        public static void RemoveTicker(IPhysicsTick ticker)
        {
            if (physicsdic.ContainsKey(ticker))
            {
                physicsTickersAr.Remove(physicsdic[ticker]);
                physicsdic.Remove(ticker);

            }
        }
        public static void RemoveTicker(IUnscaledTick ticker)
        {
            if (tickersunscaledic.ContainsKey(ticker))
            {
                unscaledArr.Remove(tickersunscaledic[ticker]);
                tickersunscaledic.Remove(ticker);

            }
        }
    }
}