using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{
    public class DelayedRelease : ITick
    {
        FauxPlaceHolderControl control;
        int portalIndex = 0;
        int total = 0;
        float delay = 0;
        int iteration = 0;
        System.Action callback;
        public DelayedRelease(FauxPlaceHolderControl control, int total, float delay, System.Action callback)
        {
            this.callback = callback;
            this.delay = delay;
            this.total = total;
            this.control = control;
        }
        public void AddTicker()
        {
            TickManager.AddTicker(this);
        }

        public float GetTickDuration()
        {
            return delay;
        }

        public void RemoveTicker()
        {
            TickManager.RemoveTicker(this);
        }

        public void Tick()
        {
            Transform portal = control.Portals[portalIndex];
            Vector2 offset = Vector2.one * UnityEngine.Random.insideUnitCircle;
            Vector3 pos = portal.position + new Vector3(offset.x, 0, offset.y);
            FauxMoveTowardsPlayer instance = SimplePool.Spawn(control.PlaceHolderPrefab.gameObject, pos, portal.rotation, false).GetComponent<FauxMoveTowardsPlayer>();
            if (control.OverrideTarget != null)
            {
                instance.OverrideTarget = control.OverrideTarget;
            }
            instance.OnDeathComplete += control.ReturnToPool;

            instance.transform.SetParent(control.transform);
            control.Active.Add(instance);

            portalIndex++;
            if (portalIndex > control.Portals.Count - 1)
            {
                portalIndex = 0;
            }

            iteration++;
            if (iteration >= total)
            {
                RemoveTicker();
                callback?.Invoke();
            }
        }
    }
    public class DemoGameMode : ITick
    {
        FauxPlaceHolderControl controller;
        float timer;

        public DemoGameMode(FauxPlaceHolderControl controller)
        {
            this.controller = controller;
        }
        public void AddTicker()
        {
            TickManager.AddTicker(this);
        }

        public float GetTickDuration()
        {
            return Time.deltaTime;
        }

        public void RemoveTicker()
        {
            TickManager.RemoveTicker(this);
        }

        public void Tick()
        {

            float dt = GetTickDuration();
            timer += dt;
            if (timer >= 1)
            {
                controller.GameTimerSeconds--;
                timer = 0;

                if (controller.GameTimerSeconds <= 0)
                {
                    controller.TimeOver();
                    RemoveTicker();
                }
            }
            controller.UpdateMethods();
        }
    }

    /// <summary>
    /// placeholder controller for demo
    /// not intended for production
    /// </summary>
    public class FauxPlaceHolderControl : MonoBehaviour
    {
        public event System.Action OnStarted;
        public static FauxPlaceHolderControl Instance => instance;
        static FauxPlaceHolderControl instance;
        public event System.Action OnGameOver;

        public int EnemyScore = 0;
        public int GameTimerSeconds = 100;
        public bool ShowGUI = true;
        public KeyCode ToggleKey = KeyCode.M;
        public KeyCode AddMoreKey = KeyCode.Alpha0;
        public KeyCode SubtractKey = KeyCode.Alpha9;
        public int ReleaseAmount = 10;
        public int PreloadSize = 500;
        public float IncreaseRate = 50;
        public FauxMoveTowardsPlayer PlaceHolderPrefab;
        public Transform OverrideTarget = null;
        public List<Transform> Portals = new List<Transform>();
        public bool AutoStart = true;
        public List<FauxMoveTowardsPlayer> Active => active;
        [SerializeField]
        protected List<FauxMoveTowardsPlayer> active = new List<FauxMoveTowardsPlayer>();
        protected bool preloaded = false;
        protected FauxMoveTowardsPlayer[] movers = new FauxMoveTowardsPlayer[0];
        public int Killed = 0;

        protected int portalIndex = 0;
        protected int screenLongSide;
        protected Rect boxRect;
        protected Rect boxRect2;
        protected Rect boxRect3;
        protected Rect boxRect4;
        protected GUIStyle style = new GUIStyle();
        protected bool gameover = false;
        protected DemoGameMode demo;
        protected DelayedRelease release;


        protected virtual void Awake()
        {
            instance = this;
            if (PlaceHolderPrefab != null)
            {
                preloaded = true;
                SimplePool.Preload(PlaceHolderPrefab.gameObject, PreloadSize);
            }
     
        }
       
        protected virtual void OnDestroy()
        {
            if (demo != null) demo.RemoveTicker();
            if (release != null) release.RemoveTicker();
            demo = null;
            release = null;
        }
 
        public void TimeOver()
        {
            if (demo != null) demo.RemoveTicker();
            demo = null;
            if (release != null) release.RemoveTicker();
            release = null;
            gameover = true;

            FauxMoveTowardsPlayer[] movers = FindObjectsOfType<FauxMoveTowardsPlayer>();
            for (int i = 0; i < movers.Length; i++)
            {
                movers[i].Move = false;
            }
            UpdateUISize();
            OnGameOver?.Invoke();

        }
        protected virtual void Start()
        {
            if (AutoStart)
            {
                StartMethods();
            }
        }
        public void StartMethods()
        {
            if (demo != null) return;

            demo = new DemoGameMode(this);
            demo.AddTicker();
            if (preloaded)
            {
                EnablePlaceholders(ReleaseAmount);
            }
            else
            {
                Debug.Log("Didn't set a prefab, can't spawn placeholders");
            }

            OnStarted?.Invoke();
        }
        public void UpdateMethods()
        {
            if (UnityEngine.Input.GetKeyDown(ToggleKey))
            {
                for (int i = 0; i < active.Count; i++)
                {
                    active[i].Move = !active[i].Move;
                }
            }

            if (UnityEngine.Input.GetKeyDown(AddMoreKey))
            {
                EnableTen();
            }

            if (UnityEngine.Input.GetKeyDown(SubtractKey))
            {
                DisableTen();
            }
            if (ShowGUI)
            {
                UpdateUISize();
            }
        
        }
        

        [ContextMenu("Release")]
        public virtual void EnableTen()
        {
            EnablePlaceholders(ReleaseAmount);
        }

        [ContextMenu("Disable")]
        public virtual void DisableTen()
        {
            DisablePlaceHolders(ReleaseAmount);
        }


        void ReleaseComplete()
        {
            release = null;
        }

        public virtual void EnablePlaceholders(int amount)
        {
            if (gameover)
            {
                return;
            }
            if (preloaded)
            {
                if (release == null)
                {
                    release = new DelayedRelease(this, amount, Time.deltaTime * 2, ReleaseComplete);
                    release.AddTicker();
                }

               
            }
            
        }
        /// <summary>
        /// means we scored
        /// </summary>
        /// <param name="fauxMoveTowardsPlayer"></param>
        public void ReturnToPoolOnScored(FauxMoveTowardsPlayer fauxMoveTowardsPlayer)
        {
            fauxMoveTowardsPlayer.OnDeathComplete -= ReturnToPool;
            active.Remove(fauxMoveTowardsPlayer);
            SimplePool.Despawn(fauxMoveTowardsPlayer.gameObject);
            EnemyScore++;
            
            if (active.Count < Mathf.RoundToInt(ReleaseAmount * .20f) || active.Count < 1)
            {
                EnablePlaceholders(ReleaseAmount - active.Count);
            }

        }
        public void ReturnToPool(FauxMoveTowardsPlayer fauxMoveTowardsPlayer)
        {
            fauxMoveTowardsPlayer.OnDeathComplete -= ReturnToPool;
            active.Remove(fauxMoveTowardsPlayer);
            SimplePool.Despawn(fauxMoveTowardsPlayer.gameObject);
            Killed += 1;
            if (Killed % IncreaseRate == 0 || active.Count < 10)
            {
                EnablePlaceholders(ReleaseAmount - active.Count);
                //spawn more
            }
        }
        public virtual void DisablePlaceHolders(int amount)
        {
            int removed = 0;
            for (int i = active.Count - 1; i >= 0; i--)
            {
                if (removed == amount)
                {
                    break;
                }
                else
                {
                    SimplePool.Despawn(active[i].gameObject);
                    active.RemoveAt(i);
                }
            }
            
        }

        protected virtual void UpdateUISize()
        {
            screenLongSide = Mathf.Max(Screen.width, Screen.height);
            var rectLongSide = screenLongSide / 10;
            boxRect = new Rect(10, 30, rectLongSide, rectLongSide / 3);
            boxRect2 = new Rect(10, 60, rectLongSide, rectLongSide / 3);
            boxRect3 = new Rect(10, 90, rectLongSide, rectLongSide / 3);
            boxRect4 = new Rect(500, 30, rectLongSide, rectLongSide / 3);
            style.fontSize = (int)(screenLongSide / 36.8);
            style.normal.textColor = Color.white;
        }

        protected virtual void OnGUI()
        {
            if (ShowGUI)
            {
                GUI.Box(boxRect, "");
                GUI.Label(boxRect, "Enemies: " + active.Count, style);
                GUI.Label(boxRect2, "Score: " + Killed, style);
                GUI.Label(boxRect3, "Enemy Score: " + EnemyScore, style);
                GUI.Label(boxRect4, "Time left: " + GameTimerSeconds, style);
            }
       
        }


    }
}
