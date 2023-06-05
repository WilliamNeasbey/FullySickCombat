
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GWLPXL.ActionCharacter
{
    public interface ILockOnTarget
    {
        Transform Transform { get; }
    }
    /// <summary>
    /// component to setup the hitboxes
    /// </summary>
    public class ActorHitBoxes : MonoBehaviour, ILockOnTarget
    {
        public event System.Action<HitBoxes> OnBoxesSetup;
        public event System.Action<HitBoxes> OnBoxesCloseDown;

        public HitBoxes HitBoxes = new HitBoxes();
        public Collider LocoCollider;
        [Tooltip("If you want your loco collider to ignore other teams.")]
        public List<int> LocoIgnoreTeams = new List<int>();
        [Tooltip("If not using action character, Auto Setup will happen on Start. If using Action Character, this can be false.")]
        public bool AutoSetup = false;
        [Tooltip("The Team these boxes are assigned to. Each team ignores other team members in terms of collision by default.")]
        public int TeamTag;

        protected int id;
        protected IAnimationEvents events;

        public Transform Transform => this.transform;

        protected virtual void Awake()
        {
            if (LocoCollider == null) LocoCollider = GetComponent<Collider>();
            id = transform.GetInstanceID();
            Animator animator = GetComponentInChildren<Animator>();
            if (animator == null)
            {
                //Debug.Log("Can't use animation events without an animator. Place an animator in the object hierarchy to use them.", this.gameObject);
            }
            else
            {
                events = animator.gameObject.GetComponent<IAnimationEvents>();
            }

        }

        public virtual void DisableLocoCollider(bool disable)
        {
            LocoCollider.enabled = !disable;
        }
        public virtual void SetupLocoCollider()
        {
            HitBoxTeamManager.AddLocomotionCollider(LocoCollider, TeamTag, LocoIgnoreTeams);
        }
        public virtual void CloseDownLocoCollider()
        {
            HitBoxTeamManager.RemoveLocomotionCollider(LocoCollider, TeamTag);
        }
        protected virtual void Start()
        {

            SetupLocoCollider();

            if (AutoSetup)
            {
                SetUp();
            }
        }

        protected virtual void OnDestroy()
        {
            CloseDownLocoCollider();
            if (AutoSetup)
            {
                CloseDown();
            }
        }

        

        public virtual void Setup(IAnimationEvents events)
        {
            HitBoxes.Setup(id, TeamTag, this);
            OnBoxesSetup?.Invoke(HitBoxes);
            if (events != null)
            {
                events.Setup(HitBoxes);
            }

        }

       

        [ContextMenu("Close Down")]
        public virtual void CloseDown()
        {
            if (Application.isPlaying == false && Application.isEditor)
            {
                Debug.Log("Enter playmode to setup and closedown hitboxes");
                return;
            }

            if (events != null)
            {
                events.CloseDown();
            }

            HitBoxes.CloseDown();
            OnBoxesCloseDown?.Invoke(HitBoxes);
        }
        [ContextMenu("Set Up")]
        public virtual void SetUp()
        {
            if (Application.isPlaying == false && Application.isEditor)
            {
                Debug.Log("Enter playmode to setup and closedown hitboxes");
                return;
            }
            Setup(events);
            
          


        }
    }


}