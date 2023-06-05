using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace GWLPXL.ActionCharacter
{
    [System.Serializable]
    public class SearchVars
    {
        public SearchType SearchType = SearchType.Closest;
        public Transform Origin = null;
        public bool WithLineOfSight = true;
        public float SearchDistance = 5;
        public DirectionType[] DirectionFilters = new DirectionType[1] { DirectionType.Any };
        public LayerMask Blocking;
        public float Cost = 1;

    }
    [System.Serializable]
    public class AggroVars
    {
        public float AggroDistance = 5;
        public float Cost = 25;

    }

    [System.Serializable]
    public class AttackVars
    {
        public float AttackDistance = 1.25f;
        public List<int> AttackSequence = new List<int>() { 0 };
        public float Cost = 50;
    }
    public interface INPCBrain
    {
        void SetState(int newState);
        bool IsActive();
        float GetActionRate();
        void UpdateBody(float dt);
    }

    public enum BrainState
    {
        None = 0,
        Searching = 1,
        Aggroing = 2,
        Attacking = 3,
        InAction = 4,
        Recovering = 5,
        InReaction = 6
    }


    public class BrainTicker : ITick
    {
        INPCBrain brain;
        public BrainTicker(INPCBrain brain)
        {
            this.brain = brain;
        }
        public void AddTicker()
        {
            TickManager.AddTicker(this);
        }

        public float GetTickDuration()
        {
            return brain.GetActionRate();
        }

        public void RemoveTicker()
        {
            TickManager.RemoveTicker(this);
        }

        public void Tick()
        {
            brain.UpdateBody(brain.GetActionRate());
           
        }
    }

    /// <summary>
    /// example class for an AI brain that speaks with the blackboard.
    /// integrate to other AI assets by calling into the INPCControl as appropriate
    /// </summary>
    public class NPC_Brain : MonoBehaviour, INPCBrain
    {
        public BrainState State = BrainState.None;
        public SearchVars SearchVars = new SearchVars();
        public AggroVars AggroVars = new AggroVars();
        public AttackVars AttackVars = new AttackVars();
        public float ResetTime = 5;
        public List<int> TargetTeams = new List<int>() { 1 };
        [SerializeField]
        protected float attackpoints = 100;
        [SerializeField]
        protected float recoverRate = 25;
        protected float resettimer = 0;
        protected int attackindex = 0;
        protected BrainTicker brainticker = null;
        protected NPCBlackboard control = null;
        protected ActionCharacter ac;

        protected float aggrocheckrate = .02f;
        protected float aggrochecktimer = 0;
        protected float actionRate = .02f;
        protected float actiontimer = 0;
        protected bool isactive = false;
        protected bool paused;

        protected virtual void Awake()
        {
            ac = GetComponent<ActionCharacter>();
            control = GetComponent<NPCBlackboard>();
        }

        protected virtual void OnEnable()
        {
            Enable(new CharacterArgs(ac));

        }
        protected virtual void OnDisable()
        {
            Disable(new CharacterArgs(ac));

        }
        protected virtual void Died(ActionCharacter ac)
        {

            Disable(new CharacterArgs(ac));

        }
       
        
        protected virtual void StartAction(CharacterActionArgs args)
        {
            ActionManager.ResetBufferMap(args.Instance);
            if (State != BrainState.Recovering)
            {
                SetState(BrainState.InAction);
            }
    
            attackindex++;
            if (attackindex > AttackVars.AttackSequence.Count - 1)
            {
                attackindex = 0;
            }
        }

        protected virtual void EndAction(CharacterActionArgs args)
        {
            ActionManager.ResetBufferMap(args.Instance);
            if (State != BrainState.Recovering)
            {
                SetState(BrainState.Attacking);
            }
           
        }

        protected virtual void Disable(CharacterArgs args)
        {
            if (isactive == false)
            {
                return;
            }

            ac.Events.OnActionStarted -= StartAction;
            ac.Events.OnActionEnded -= EndAction;

            if (brainticker != null) brainticker.RemoveTicker();
            SetState(BrainState.None);
            brainticker = null;
            isactive = false;
            this.enabled = false;
           // Debug.Log("Brain Ticker Removed", this.gameObject);
        }

        protected virtual void Enable(CharacterArgs args)
        {
            if (isactive == true)
            {
                return;
            }

            ac.Events.OnActionStarted += StartAction;
            ac.Events.OnActionEnded += EndAction;


            if (SearchVars.Origin == null) SearchVars.Origin = this.transform;

            if (brainticker != null) brainticker.RemoveTicker();
            this.enabled = true;
            brainticker = null;
            brainticker = new BrainTicker(this);
            brainticker.AddTicker();
            isactive = true;

        }
        protected virtual void Start()
        {
            if (ac.DamageController != null)
            {
                ac.DamageController.OnDied += Died;
            }

    




        }
        protected virtual void OnDestroy()
        {
            if (ac.DamageController != null)
            {
                ac.DamageController.OnDied -= Died;
            }
  
        
            if (brainticker != null) brainticker.RemoveTicker();
            brainticker = null;
        }
        public virtual void SetState(int newstate)
        {
            SetState((BrainState)newstate);
        }
        public virtual void SetState(BrainState newstate)
        {
            State = newstate;
            switch (State)
            {
                case BrainState.Searching:
                    attackindex = 0;
                    break;
                case BrainState.InReaction:
                    attackindex = 0;
                    control.SetInput(0, 0);
                    control.ConsumeInputs();
                    resettimer = 0;
                    break;
            }
        }



        protected virtual void Search()
        {

            Transform target = CombatHelper.GetTarget(SearchVars.SearchType, SearchVars.Origin, TargetTeams[0],SearchVars.SearchDistance, SearchVars.WithLineOfSight, SearchVars.DirectionFilters);
            if (target != null)
            {
                control.SetInput(0, 0);
                control.SetTarget(target);
                SetState(BrainState.Aggroing);
            }
            else
            {
                control.SetInput(0, 0);
                
            }
            
        }

        /// <summary>
        /// used to stop brain during reactions
        /// </summary>
        /// <param name="isPaused"></param>
        public virtual void Pause(bool isPaused)
        {
            paused = isPaused;
            if (paused)
            {
                control.ConsumeInputs();
                ActionManager.ResetBufferMap(ac);
                control.SetInput(0, 0);
            }
        }


        public virtual void UpdateBody(float dt)
        {
           

            if (State == BrainState.InReaction)
            {
                return;
            }
            resettimer += dt;
            if (State == BrainState.Recovering)
            {
                control.SetTarget(null);
                control.SetInput(0, 0);
                attackpoints += recoverRate * dt;
                attackpoints = Mathf.Clamp(attackpoints, 0f, 100f);
                if (attackpoints >= 100)
                {
                    SetState(BrainState.Searching);
                }
                return;
            }
            else
            {
                if (attackpoints <= 0)
                {
                    SetState(BrainState.Recovering);
                    //recover

                }
            }

            if (resettimer >= ResetTime)
            {
                control.SetTarget(null);
                resettimer = 0;
            }

            Transform target = control.GetTarget();
            

                if (target == null || target.gameObject.activeInHierarchy == false)
                {
                    control.SetTarget(null);
                    SetState(BrainState.Searching);
                }
                if (State == BrainState.None)
                {
                    SetState(BrainState.Searching);
                }
            
            

            if (ActionManager.GetMyCombos(ac).Count <= 0)
            {
                OnTriggerLoadoutSwitch t = FindObjectOfType<OnTriggerLoadoutSwitch>();
                if (t != null)
                {
                    target = t.transform;
                    Vector3 find = target.transform.position - this.transform.position;
                    find = Vector3.ProjectOnPlane(find, Vector3.up);
                    control.SetInput(find.x, find.z);
                    return;
                }
            }


         

            switch (State)
            {
                case BrainState.Searching:
                    attackpoints -= SearchVars.Cost * dt;
                    Search();
                    break;
                case BrainState.Aggroing:
                    //overlap sphere check?
                    attackpoints -= AggroVars.Cost * dt;
                    aggrochecktimer += dt;
                    if (aggrochecktimer >= aggrocheckrate)
                    {
                        aggrochecktimer = 0;
                        if (Vector3.Distance(target.position, this.transform.position) > AttackVars.AttackDistance)
                        {
                            DirectionType dirT = Detection.DetectHitDirection(transform, target);
                            if (dirT != DirectionType.Above || dirT != DirectionType.Below)
                            {
                   
                                Vector3 dir = target.position - this.transform.position;
                                dir = Vector3.ProjectOnPlane(dir, Vector3.up);
                                dir.Normalize();
                                control.SetInput(dir.x, dir.z);
                            }
                            else
                            {
                                //control.SetInput(0, 0);
                            }


                        }
                        else
                        {
                            SetState(BrainState.Attacking);
                        }
                    }
                    
                    break;
                case BrainState.Attacking:
                    if (target != null)
                    {
                        SetState(BrainState.InAction);
                    }
                    else
                    {
                        SetState(BrainState.Searching);
                    }
                    break;
                case BrainState.InAction:

                    if (target == null)
                    {
                        SetState(BrainState.Searching);
                        return;
                    }
                   

                    attackpoints -= AttackVars.Cost * dt;
               
                    float d = Vector3.Distance(target.position, this.transform.position);
                    if (d < AttackVars.AttackDistance)
                    {
               
                        control.SetInput(0, 0);
                        if (attackindex > AttackVars.AttackSequence.Count - 1)
                        {
                            attackindex = 0;
                        }
                        control.TriggerActionSlot(AttackVars.AttackSequence[attackindex]);
                        

                    }
                    else
                    {
                        SetState(BrainState.Aggroing);
                    }
                    break;

            }
            
        }

        public virtual float GetActionRate()
        {
            return actionRate;
        }

        public bool IsActive()
        {
            return isactive;
        }
    }
}
