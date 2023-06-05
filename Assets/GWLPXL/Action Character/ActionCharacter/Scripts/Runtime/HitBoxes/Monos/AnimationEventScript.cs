
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GWLPXL.ActionCharacter
{
    [System.Serializable]
    public class HitBoxAnimationEvent : UnityEvent<string> { }

    [System.Serializable]
    public class UnityEventsHitBoxAnimationEvents
    {
        [Header("Groups")]
        public HitBoxAnimationEvent OnEnabledGroup;
        public HitBoxAnimationEvent OnDisabledGroup;
        [Header("Givers")]
        public HitBoxAnimationEvent OnEnableHitGiver;
        public HitBoxAnimationEvent OnDisableHitGiver;

        [Header("Takers")]
        public HitBoxAnimationEvent OnEnableHitTaker;
        public HitBoxAnimationEvent OnDisableHitTaker;

    }

   
    /// <summary>
    /// to do, add keyword events, use args
    /// </summary>
    [System.Serializable]
    public class HitBoxAnimationEvents
    {
        public event System.Action<string> OnEnableGroup;
        public event System.Action<string> OnDisableGroup;

        public event System.Action<string> OnEnableHitGiver;
        public event System.Action<string> OnDisableHitGiver;
        public event System.Action<string> OnEnableHitTaker;
        public event System.Action<string> OnDisableHitTaker;

        public event System.Action OnDisableAllHitGivers;
        public event System.Action OnDisableAllHitTakers;
        public void FireOnDisableAllHitTakers()
        {
            OnDisableAllHitTakers?.Invoke();
        }
        public void FireOnDisableAllHitGivers()
        {
            OnDisableAllHitGivers?.Invoke();
        }
        public void FireOnEnableGroup(string args)
        {
            OnEnableGroup?.Invoke(args);
        }
        public void FireOnDisableGroup(string args)
        {
            OnDisableGroup?.Invoke(args);
        }
        public void FireOnEnableHitGiver(string args)
        {
            OnEnableHitGiver?.Invoke(args);
        }
        public void FireOnDisableHitGiver(string args)
        {
            OnDisableHitGiver?.Invoke(args);
        }
        public void FireOnEnableHitTaker(string args)
        {
            OnEnableHitTaker?.Invoke(args);
        }
        public void FireOnDisableHitTaker(string args)
        {
            OnDisableHitTaker?.Invoke(args);
        }
        public UnityEventsHitBoxAnimationEvents UnityEvents;
    }

    [System.Serializable]
    public class HitBoxAnimationEventController
    {
        public HitBoxAnimationEvents HitBoxAnimationEvents;
        GroupHitBoxController giverController;
        GroupHitBoxController takerController;
        HitTakers takers;
        HitGivers givers;
        HitBoxes owner;
        public virtual void Subscribe(HitBoxes owner)
        {
            this.owner = owner;
            takers = owner.HitTakers;
            givers = owner.HitGivers;

            giverController = owner.HitGivers.HitBoxesBase.GroupHitboxEnabler;
            takerController = owner.HitTakers.HitBoxesBase.GroupHitboxEnabler;

            //group
            if (HitBoxAnimationEvents == null)
            {
                Debug.LogError("Animation Events are not assigned on the gameobject with the Animator. Did you forget to click Assign Required Animator Scripts on the action controller base script?");
            }
            HitBoxAnimationEvents.OnEnableGroup += giverController.EnableBoxes;
            HitBoxAnimationEvents.OnEnableGroup += takerController.EnableBoxes;
            HitBoxAnimationEvents.OnDisableGroup += giverController.DisableBoxes;
            HitBoxAnimationEvents.OnDisableGroup += takerController.DisableBoxes;

            //boxes
            HitBoxAnimationEvents.OnEnableHitGiver += givers.EnableHitGiver;
            HitBoxAnimationEvents.OnDisableHitGiver += givers.DisableHitGiver;
            HitBoxAnimationEvents.OnEnableHitTaker += takers.EnableHitTaker;
            HitBoxAnimationEvents.OnDisableHitTaker += takers.DisableHitTaker;


        }
        public virtual void UnSub()
        {
            if (owner == null) return;
            //group
            HitBoxAnimationEvents.OnEnableGroup -= giverController.EnableBoxes;
            HitBoxAnimationEvents.OnEnableGroup -= takerController.EnableBoxes;

            HitBoxAnimationEvents.OnDisableGroup -= giverController.DisableBoxes;
            HitBoxAnimationEvents.OnDisableGroup -= takerController.DisableBoxes;

            //boxes
            HitBoxAnimationEvents.OnEnableHitGiver -= givers.EnableHitGiver;
            HitBoxAnimationEvents.OnDisableHitGiver -= givers.DisableHitGiver;

            HitBoxAnimationEvents.OnEnableHitTaker -= takers.EnableHitTaker;
            HitBoxAnimationEvents.OnDisableHitTaker -= takers.DisableHitTaker;

            owner = null;
        }
    }

    public interface IAnimationEvents
    {
        void Setup(HitBoxes owner);
        void CloseDown();
        void EnableGroupBoxes(string name);
        void DisableGroupBoxes(string name);
        void EnableHitGiver(string name);
        void DisableHitGiver(string name);
        void EnableHitTaker(string name);
        void DisableHitTaker(string name);
        void DisableAllHitGivers();
        void DisableAllHitTakers();
        void DisableAllActionBoxes();

    }
    /// <summary>
    /// animation event script, used to link hitboxes to unity's animation system.
    /// Inherit IAnimationEvents if you want to create your own
    /// </summary>
    public class AnimationEventScript : MonoBehaviour, IAnimationEvents
    {
        public HitBoxAnimationEventController HitBoxAnimationController = new HitBoxAnimationEventController();
        [SerializeField]
        protected bool useDebug = true;

        //used to reset if early exit
        protected List<string> enabledgivers = new List<string>();
        protected List<string> enabledtakers = new List<string>();
        protected List<string> actions = new List<string>();
        protected bool usingHitBoxes  = false;

        protected HitBoxes boxes;
        public virtual void Setup(HitBoxes owner)
        {
            HitBoxAnimationController.Subscribe(owner);
            DebugMessage("Animation Event Setup complete", this.gameObject);
            usingHitBoxes = owner != null;
            boxes = owner;
        }



 

        protected virtual void DebugMessage(string message, UnityEngine.Object ctx)
        {
            if (useDebug)
            {
                Debug.Log(message, ctx);
            }
        }
        public virtual void CloseDown()
        {
            HitBoxAnimationController.UnSub();
            enabledgivers.Clear();
            enabledtakers.Clear();
            actions.Clear();
            DebugMessage("Animation Event Close Complete", this.gameObject);
        }

        public virtual void EnableComboExit()
        {
            if (usingHitBoxes)
            {
                if (ActionManager.InActionSequence(boxes.Owner))
                {
                    ActionTickerCC ticker = ActionManager.GetActionTickerCC(boxes.Owner);
                    ticker.AllowComboExit(true);
                }
              
            }
        }
        public virtual void DisableComboExit()
        {
            if (usingHitBoxes)
            {
                if (ActionManager.InActionSequence(boxes.Owner))
                {
                    ActionTickerCC ticker = ActionManager.GetActionTickerCC(boxes.Owner);
                    ticker.AllowComboExit(false);
                }

            }
        }
        public virtual void EnableGroupBoxes(string groupname)
        {
            if (usingHitBoxes == false) return;

            HitBoxAnimationController.HitBoxAnimationEvents.FireOnEnableGroup(groupname);
            HitBoxAnimationController.HitBoxAnimationEvents.UnityEvents.OnEnabledGroup?.Invoke(groupname);
            DebugMessage("Animation Event Enable Action Boxes for " + groupname, this.gameObject);
            AddToTrackingLists(actions, groupname);
        }

        public virtual void DisableGroupBoxes(string groupname)
        {
            if (usingHitBoxes == false) return;

            HitBoxAnimationController.HitBoxAnimationEvents.FireOnDisableGroup(groupname);
            HitBoxAnimationController.HitBoxAnimationEvents.UnityEvents.OnDisabledGroup?.Invoke(groupname);
            DebugMessage("Animation Event Disable Action Boxes for " + groupname, this.gameObject);
            RemoveFromTrackingList(actions, groupname);
        }

        public virtual void DisableAllActionBoxes()
        {
            if (usingHitBoxes == false) return;

            for (int i = 0; i < actions.Count; i++)
            {
                DisableGroupBoxes(actions[i]);
            }
        }
        public virtual void DisableAllHitGivers()
        {
            if (usingHitBoxes == false) return;

            for (int i = 0; i < enabledgivers.Count; i++)
            {
                DisableHitGiver(enabledgivers[i]);
            }
        }

        public virtual void DisableAllHitTakers()
        {
            if (usingHitBoxes == false) return;

            for (int i = 0; i < enabledtakers.Count; i++)
            {
                DisableHitGiver(enabledtakers[i]);
            }
        }


        public virtual void EnableHitGiver(string giverName)
        {
            if (usingHitBoxes == false) return;

            HitBoxAnimationController.HitBoxAnimationEvents.FireOnEnableHitGiver(giverName);
            HitBoxAnimationController.HitBoxAnimationEvents.UnityEvents.OnEnableHitGiver?.Invoke(giverName);
            DebugMessage("Animation Event Enable Hit Giver " + giverName, this.gameObject);
            AddToTrackingLists(enabledgivers, giverName);
        }

      
        public virtual void DisableHitGiver(string giverName)
        {
            if (usingHitBoxes == false) return;

            HitBoxAnimationController.HitBoxAnimationEvents.FireOnDisableHitGiver(giverName);
            HitBoxAnimationController.HitBoxAnimationEvents.UnityEvents.OnDisableHitGiver?.Invoke(giverName);
            DebugMessage("Animation Event Disable Hit Giver " + giverName, this.gameObject);
            RemoveFromTrackingList(enabledgivers, giverName);
        }


        public virtual void EnableHitTaker(string takerName)
        {
            if (usingHitBoxes == false) return;

            HitBoxAnimationController.HitBoxAnimationEvents.FireOnEnableHitTaker(takerName);
            HitBoxAnimationController.HitBoxAnimationEvents.UnityEvents.OnEnableHitTaker?.Invoke(takerName);
            DebugMessage("Animation Event Enable Hit Taker " + takerName, this.gameObject);
            AddToTrackingLists(enabledtakers, takerName);
        }

        public virtual void DisableHitTaker(string takerName)
        {
            if (usingHitBoxes == false) return;

            HitBoxAnimationController.HitBoxAnimationEvents.FireOnDisableHitTaker(takerName);
            HitBoxAnimationController.HitBoxAnimationEvents.UnityEvents.OnDisableHitTaker?.Invoke(takerName);
            DebugMessage("Animation Event Disabe Hit Taker " + takerName, this.gameObject);
            RemoveFromTrackingList(enabledtakers, takerName);
        }


        protected virtual void AddToTrackingLists(List<string> tracking, string name)
        {
            if (usingHitBoxes == false) return;

            if (tracking.Contains(name) == false)
            {
                tracking.Add(name);
            }

        }
        protected virtual void RemoveFromTrackingList(List<string> tracking, string name)
        {
            if (usingHitBoxes == false) return;

            if (tracking.Contains(name) == true)
            {
                tracking.Remove(name);
            }

        }
    }
}