

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace GWLPXL.ActionCharacter
{
    /// <summary>
    /// interface for hit box labels
    /// implement to get the actor hit box editor functionality on custom scripts
    /// </summary>
    public interface IHaveHitBoxes
    {

        List<string> GetHitGivers();
        List<string> GetHitTakers();
        void SetHitTakers(List<string> takers);
        void SetHitGivers(List<string> givers);


    }
    [System.Serializable]
    public class CharacterActionSetArgs : System.EventArgs
    {
        public ActionCharacter Instance;
        public List<ActionSet> Set;
        public CharacterActionSetArgs(ActionCharacter instance, List<ActionSet> set)
        {
            Instance = instance;
            Set = set;

        }
    }

    [System.Serializable]
    public class CharacterArgs : System.EventArgs
    {
        public ActionCharacter Instance;

        public CharacterArgs(ActionCharacter instance)
        {
            Instance = instance;

        }
    }
    [System.Serializable]
    public class CharacterTargetArgs : System.EventArgs
    {
        public ActionCharacter Instance;
        public Transform Target;
        public CharacterTargetArgs(ActionCharacter instance, Transform target)
        {
            Instance = instance;
            Target = target;

        }
    }
    [System.Serializable]
    public class CharacterDestinationArgs : System.EventArgs
    {
        public ActionCharacter Instance;
        public Vector3 Target;
        public CharacterDestinationArgs(ActionCharacter instance, Vector3 target)
        {
            Instance = instance;
            Target = target;

        }
    }

    [System.Serializable]
    public class CharacterActionArgs : System.EventArgs
    {
        public ActionCharacter Instance;
        public string ActionName;
        public CharacterActionArgs(ActionCharacter instance, string actionname)
        {
            Instance = instance;
            ActionName = actionname;

        }
    }

    [System.Serializable]
    public class ActionCharacterSetupUnityEvent : UnityEvent<ActionCharacter> { };
    [System.Serializable]
    public class ActionCharacterUnityEvent : UnityEvent<CharacterActionArgs> { };

    [System.Serializable]
    public class UnityActionCharacterEvents
    {
        public ActionCharacterSetupUnityEvent OnSetupComplete;
        public ActionCharacterUnityEvent OnActionSequenceStarted;
        public ActionCharacterUnityEvent OnActionSequenceEnded;

        public ActionCharacterUnityEvent OnActionAdded;
        public ActionCharacterUnityEvent OnActionRemoved;

    }
    [System.Serializable]
    public class ActionCharacterEvents
    {
        public event System.Action<CharacterArgs> OnSpawned;
        public event System.Action<CharacterArgs> OnDespawned;
        public event System.Action<CharacterArgs> OnPlayerControlled;
        public event System.Action<CharacterArgs> OnNewInputWrapper;
        public event System.Action<CharacterArgs> OnNewActionMap;
        public event System.Action<CharacterArgs> OnSetupComplete;
        public event System.Action<CharacterArgs> OnLockStateEnter;
        public event System.Action<CharacterArgs> OnLockStateExit;
        public event System.Action<CharacterActionArgs> OnActionStarted;
        public event System.Action<CharacterActionArgs> OnActionEnded;
        public event System.Action<CharacterActionArgs> OnNextSequence;
        public event System.Action<CharacterActionArgs> OnActionAdded;
        public event System.Action<CharacterActionArgs> OnActionRemoved;
       

        public event System.Action<CharacterTargetArgs> OnTargetAcquired;
        public event System.Action<CharacterDestinationArgs> OnDestinationAcquired;
        public event System.Action<CharacterTargetArgs> OnTargetLost;
        public event System.Action<CharacterActionSetArgs> OnLoadoutAdded;
        public event System.Action<CharacterActionSetArgs> OnLoadoutRemoved;
        public UnityActionCharacterEvents UnityEvents;
        public void FireOnSpawned(CharacterArgs args)
        {
            OnSpawned?.Invoke(args);
        }
        public void FireOnDespawn(CharacterArgs args)
        {
            OnDespawned?.Invoke(args);
        }
        public void FireLockStateEnter(CharacterArgs args)
        {
            OnLockStateEnter?.Invoke(args);
        }
        public void FireLockStateExit(CharacterArgs args)
        {
            OnLockStateExit?.Invoke(args);
        }
        public void FirePlayerControlled(CharacterArgs args)
        {
            OnPlayerControlled?.Invoke(args);
        }
        public void FireNewInputWrapper(CharacterArgs args)
        {
            OnNewInputWrapper?.Invoke(args);
        }
        public void FireNewActionMap(CharacterArgs args)
        {
            OnNewActionMap?.Invoke(args);
        }
        public void FireLoadoutRemove(CharacterActionSetArgs args)
        {
            OnLoadoutRemoved?.Invoke(args);
        }
        public void FireLoadoutAdded(CharacterActionSetArgs args)
        {
            OnLoadoutAdded?.Invoke(args);
        }
        public void FireTargetLost(CharacterTargetArgs args)
        {
            OnTargetLost?.Invoke(args);
        }
        public void FireTargetAcquired(CharacterDestinationArgs args)
        {
            OnDestinationAcquired?.Invoke(args);
        }
        public void FireTargetAcquired(CharacterTargetArgs args)
        {
            OnTargetAcquired?.Invoke(args);
        }
        public void FireSetupComplete(CharacterArgs args)
        {
            OnSetupComplete?.Invoke(args);
        }
        public void FireActionStarted(CharacterActionArgs args)
        {
            OnActionStarted?.Invoke(args);
        }
        public void FireNextSequenceStarted(CharacterActionArgs args)
        {
            OnNextSequence?.Invoke(args);
        }
        public void FireActionEnded(CharacterActionArgs args)
        {
            OnActionEnded?.Invoke(args);
        }
        public void FireActionAdded(CharacterActionArgs args)
        {
            OnActionAdded?.Invoke(args);
        }
        public void FireActionRemoved(CharacterActionArgs args)
        {
            OnActionRemoved?.Invoke(args);
        }

    }

    [System.Serializable]
    public class LoadoutInstanceOverrides
    {
        public CharacterActionLoadoutSO Loadout = null;
        public List<GameObject> SyncedObjects = new List<GameObject>();
    }

    [System.Serializable]
    public class RuntimeInstanceOverrides
    {
        public string Name;
        public List<GameObject> Enabled = new List<GameObject>();

        public RuntimeInstanceOverrides(string name, List<GameObject> enabled)
        {
            Name = name;
            Enabled = enabled;
        }

        public void EnableObjects()
        {
            for (int i = 0; i < Enabled.Count; i++)
            {
                Enabled[i].SetActive(true);
            }
        }
        public void DisableObjects()
        {
            for (int i = 0; i < Enabled.Count; i++)
            {
                Enabled[i].SetActive(false);
            }
        }
    }
    /// <summary>
    /// abstract class that defines a character that performs actions
    /// </summary>
    public abstract class ActionCharacter : MonoBehaviour, IHaveHitBoxes
    {
        public ActionCharacterEvents Events = new ActionCharacterEvents();

        public int PlayerNumber => playerNumber;
        public ActorHitBoxes GetHitBoxes() => hitboxes;

        [SerializeField]
        protected bool persist = false;
        [SerializeField]
        protected int playerNumber = 0;
        public virtual void SetPlayerNumber(int number)
        {
            playerNumber = number;
        }

        public InputActionMapSO InputRequirements
        {
            get
            {
                return inputActionMap;
            }
            set
            {
                SetNewActionMap(value);
            }
        }

        public virtual void SetNewInputWrapper(InputWrapperSO newWrapper)
        {
            inputWrapper = newWrapper;
            Events.FireNewInputWrapper(new CharacterArgs(this));
        }
        public virtual void SetNewActionMap(InputActionMapSO newMap)
        {
            if (newMap == null) return;
            //clone the inputs
            for (int i = 0; i < newMap.InputSlots.Count; i++)
            {
                InputSlot slot = newMap.InputSlots[i];
                InputActionSlotSO clone = ScriptableObject.Instantiate(slot.Requirements);
                clone.name = slot.Requirements.name;
                clone.Key = slot.Requirements.Key;
                slot.Requirements = clone;

            }


            inputActionMap = newMap;
            inputActionMap.Setup();
            Events.FireNewActionMap(new CharacterArgs(this));

        }
        public InputWrapperSO InputWrapper
        {
            get
            {
                return inputWrapper;
            }
            set
            {
                SetNewInputWrapper(value);
            }
        }
        public bool IsPlayedControlled => playerControlled;
        public virtual void SetPlayerControlled(bool isControlled)
        {
            playerControlled = isControlled;
            Events.FirePlayerControlled(new CharacterArgs(this));
        }
        protected LockOnState lockstate = null;
        protected bool hasStrafe = false;
        protected bool hasLockOn = false;
        [SerializeField]
        protected bool playerControlled = false;
        [Header("Input")]
        [SerializeField]
        protected InputActionMapSO inputActionMap;

        [SerializeField]
        protected InputWrapperSO inputWrapper;

        public LockOn LockOn => lockOn;
        public Strafe Strafe => strafe;
        [SerializeField]
        protected LockOn lockOn = new LockOn();
        [SerializeField]
        protected Strafe strafe = new Strafe();
        public Animator Animator => AnimatorController == null ? anim : AnimatorController.GetAnimator();

        protected List<RuntimeInstanceOverrides> currentOverrides = new List<RuntimeInstanceOverrides>();
        public bool IsRidingPlatform => isRidingPlatform;
        protected bool isRidingPlatform = false;
        protected bool canMove = true;
        public virtual void SetRidingPlatform(bool isRiding)
        {
            isRidingPlatform = isRiding;
        }
        public virtual void SetMoveEnabled(bool isEnabled)
        {
            canMove = isEnabled;
        }
        public int FPS => targetfps;
        public bool HasLockOnTarget => lockOnTarget;
        public Transform LockOnTarget => lockOnTarget;
        public bool HasLockOnPosition => hasLockOnDestination;
        public Vector3 LockOnPosition => lockOnPosition;
        protected Vector3 lockOnPosition;
        protected bool hasLockOnDestination;

        public bool ActionCheckEnabled => actionCheck;

        public CharacterConfig Config { get => config; set => config = value; }


        public Transform Transform { get => t; }
        public MovementSO MovementRuntime { get => runtime; }

        public IRootMotion RootMotionController { get => rootmotion; }
        public IAnimationEvents AnimationEvents { get => events; }
        public IAnimatorController AnimatorController { get => animController; }
        public ITakeDamage DamageController { get => takeDamageController; }
        public FlowControlSO Flow { get => flowControls; set => flowControls = value; }
        public int ID { get => id; }
        public ActionsDatabaseSO ActionsDatabase { get => actionsdatabaseSO; set => actionsdatabaseSO = value; }
        public CharacterActionLoadoutSO Loadout { get => loadOutSO; set => loadOutSO = value; }

        public Dictionary<string, ActionSO> Actions => actions;

        public float Height { get => GetHeight(); }
        public float Radius { get => GetRadius(); }
        public Vector3 Center { get => GetCenter(); }

        public MovementSO MovementTemplate { get => movementTemplate; set => movementTemplate = value; }
        [SerializeField]
        protected MovementSO movementTemplate = default;
        protected MovementSO runtime = default;

        protected FlowControlSO flowControls = default;
        [Header("Config")]
        [SerializeField]
        protected CharacterConfig config = default;
        [Header("Actions")]
        [Tooltip("Default loadout to start with")]
        [SerializeField]
        protected CharacterActionLoadoutSO loadOutSO = default;

        [SerializeField]
        protected List<LoadoutInstanceOverrides> loadoutOverrides = new List<LoadoutInstanceOverrides>();
        [SerializeField]
        protected ActionsDatabaseSO actionsdatabaseSO = default;
        [SerializeField]
        protected int targetfps = 60;
        [SerializeField]
        protected Animator anim = default;
        protected Transform lockOnTarget = default;
        protected bool hasLockOnTarget = false;
        protected Dictionary<string, ActionSO> actions = new Dictionary<string, ActionSO>();
        protected List<string> filteredactions = new List<string>();

        protected IRootMotion rootmotion = default;
        protected IAnimationEvents events = default;
        protected IAnimatorController animController = default;
        protected IActionIK ikController = default;
        protected ITakeDamage takeDamageController = default;
        protected Transform t = default;
        protected int id = 0;
        protected bool actionCheck = false;
        protected bool hasHealthSystem = false;

        protected List<ActionSO> empty = new List<ActionSO>();

        protected ActorHitBoxes hitboxes = null;
        protected bool disableHitBoxesOnDeath = true;
        protected Vector3 outsideVel = new Vector3(0, 0, 0);
        protected Vector3 emptyvector = new Vector3(0, 0, 0);
        protected Vector3 rotateInputVector = new Vector3(0, 0, 0);
        protected Vector3 movementInputVector = new Vector3(0, 0, 0);
        protected bool hasOutsideVel = false;
        protected bool hasMovement = false;
        protected float one = 1;
        protected int airborneActions = 0;
        protected float stepheight = 0.3f;
        protected RotateType rotatype = RotateType.None;
        protected float slopelimit = 45f;

        #region protected
        protected virtual void NewScene(Scene scene, LoadSceneMode mode)
        {
            RemoveLocotion();
            InitializeLocomotion();

        }

        public virtual void Despawn()
        {
            RemoveLocotion();
            hitboxes.CloseDownLocoCollider();
            hitboxes.CloseDown();
            ActionManager.UnRegister(id);
            Events.FireOnDespawn(new CharacterArgs(this));
        }
        public virtual void Spawned()
        {
            ActionManager.Register(this);
            SetGroundMulti(Config.Defaults.MoveMulti);
            SetRotateMulti(Config.Defaults.RotateMulti);
            SetFallMulti(Config.Defaults.FallMulti);
            SetAirMulti(Config.Defaults.AirborneMulti);
            hitboxes.SetupLocoCollider();
            hitboxes.SetUp();
            InitializeLocomotion();
            Events.FireOnSpawned(new CharacterArgs(this));
        }

        protected virtual void Resub()
        {

            hitboxes.CloseDown();
            hitboxes.SetUp();
        }


        #endregion

        public abstract void InitializeLocomotion();
        public abstract void RemoveLocotion();
        public virtual void CheckInputs()
        {
            InputBufferMap map = ActionManager.GetActionInputRequirements(this, true);
            if (map.InputSlotIndex > -1)
            {
                bool started = TryStartActionSequence(map.ActionName);
                if (started)
                {
                    return;//proceeds all
                }
            }
            
            if (hasLockOn)
            {
                bool trylock = ActionManager.InputCheck(this, lockOn.Input.InputRequirements);
                if (trylock)
                {
                    if (GetRotateType() == RotateType.Locked)
                    {
                        SetRotateType(RotateType.Free);
                        if (lockstate != null)
                        {
                            lockstate.RemoveTicker();
                            lockstate = null;
                        }
                    }
                    else if (GetRotateType() != RotateType.Locked)
                    {
                        SetRotateType(RotateType.Locked);
                        if (lockstate == null)
                        {
                            lockstate = new LockOnState(this, lockOn.Vars);
                            lockstate.AddTicker();
                        }
                    }
                    
                }
            }

            if (hasStrafe)
            {
                bool trystrafe = ActionManager.InputCheck(this, strafe.Input.InputRequirements);
                if (trystrafe)
                {
                    if (lockstate != null)
                    {
                        lockstate.RemoveTicker();
                        lockstate = null;
                    }
                    if (GetRotateType() == RotateType.Strafe)
                    {
                        SetRotateType(RotateType.Free);
                        if (lockstate != null)
                        {
                            lockstate.RemoveTicker();
                            lockstate = null;
                        }
                    }
                    else if (GetRotateType() != RotateType.Strafe)
                    {
                        SetRotateType(RotateType.Strafe);
                        if (lockstate != null)
                        {
                            lockstate.RemoveTicker();
                            lockstate = null;
                        }
                    }
                }
            }

        }

        public virtual void ResetLockOnPosition()
        {
            hasLockOnDestination = false;
        }
        public virtual void SetLockOnDestination(Vector3 target)
        {

            Events.FireTargetAcquired(new CharacterDestinationArgs(this, target));
            hasLockOnDestination = true;
            lockOnPosition = target;
 
        }
        public virtual void SetLockOnTarget(Transform target)
        {
            if (lockOnTarget != target && target != null)
            {
                Events.FireTargetAcquired(new CharacterTargetArgs(this, target));
            }
            else if (lockOnTarget != null && target == null)
            {
                Events.FireTargetLost(new CharacterTargetArgs(this, lockOnTarget));
            }

            lockOnTarget = target;
            hasLockOnTarget = lockOnTarget != null;

            if (hasLockOnTarget)
            {
               
                if (GetRotateType() != RotateType.Locked)
                {
                    SetRotateType(RotateType.Locked);
                    if (lockstate == null)
                    {
                        lockstate = new LockOnState(this, lockOn.Vars);
                        lockstate.AddTicker();
                    }
                }

            }
            else
            {
                if (GetRotateType() == RotateType.Locked)
                {
                    SetRotateType(RotateType.Free);
                    if (lockstate != null)
                    {
                        lockstate.RemoveTicker();
                        lockstate = null;
                    }
                }
            }
        }
        protected virtual void OnEnable() { }
        protected virtual void OnDisable() { }

        public virtual void AddActionFilters(List<string> newactionfilters)
        {
            for (int i = 0; i < newactionfilters.Count; i++)
            {
                string key = CommonFunctions.StringKey(newactionfilters[i]);
               if (filteredactions.Contains(key) == false)
                {
                    filteredactions.Add(key);
                }

            }
        }

        public virtual void SetStepHeight(float newheight)
        {
            stepheight = newheight;
        }
        public virtual float GetStepHeight()
        {
            return stepheight;
        }
        public virtual int GetCurrentAirborneActions()
        {
            return airborneActions;
        }
        public virtual void ResetAirborneActions()
        {
            airborneActions = 0;
        }
        public virtual void ModifyAirborneActions(int amount)
        {
            airborneActions += amount;
            
        }
        public virtual bool CanAirborneAction()
        {
            if (hasMovement) return airborneActions < runtime.Movement.Standard.Locomotion.Fall.Falling.MaxAirborneActions;
            return false;
        }

        public virtual void SetRootmotion(bool isRoot)
        {
            rootmotion.SetRootMotionActive(isRoot);
        }
        public virtual void ClearSteps()
        {
            outsideVel = new Vector3(0, 0, 0);
            hasOutsideVel = false;
        }
        public virtual void ClearActionFilters()
        {
            filteredactions.Clear();
            
        }
        public virtual void AddStep(Vector3 newStep)
        {
            outsideVel += newStep;
            hasOutsideVel = true;
        }
       
        public virtual void Rotate()
        {
            if (hasMovement == false) return;
            switch (GetRotateType())
            {
                case RotateType.Strafe:
                    //
                    break;
                case RotateType.Free:
                    transform.rotation = runtime.Movement.Standard.Locomotion.Rotate.Rotation.FaceDirection;
                    break;
                case RotateType.None:
                    //
                    break;
                case RotateType.Locked:
                    if (HasLockOnTarget)
                    {
                        Vector3 input = LockOnTarget.position - transform.position;
                        if (input.sqrMagnitude > .01)
                        {
                            input.y = 0;
                            Quaternion rot = Quaternion.LookRotation(input, Vector3.up);
                            Quaternion rotatet = Quaternion.RotateTowards(Transform.rotation, rot, GetSpeed());
                            transform.rotation = rotatet;

                        }
                        
                    }
                    else
                    {
                        transform.rotation = runtime.Movement.Standard.Locomotion.Rotate.Rotation.FaceDirection;

                    }
                    break;
            }

            
        }

  
        public virtual RotateType GetRotateType()
        {
            if (hasMovement == false) return rotatype;
            return runtime.Movement.Standard.Locomotion.Rotate.Rotation.RotateType;
        }

        public virtual void SetRotateType(RotateType newType)
        {
            if (hasMovement == false) rotatype = newType;
            runtime.Movement.Standard.Locomotion.Rotate.Rotation.RotateType = newType;
        }
        public virtual void Move()
        {
            if (hasOutsideVel && canMove)
            {
                t.position += outsideVel;
                outsideVel = new Vector3(0, 0, 0);
                hasOutsideVel = false;
            }

        }

       
        public virtual FreeFormState GetCharacterStateAC()
        {
            FreeFormState state = FreeFormState.Ground;
            if (hasHealthSystem)
            {
                if (takeDamageController.IsDead)
                {
                    state = FreeFormState.Dead;
                    return state;
                }
            }
           
           
            if (runtime.Movement.Standard.Locomotion.Fall.Falling.IsGrounded == false)
            {
                if (runtime.Movement.Standard.Locomotion.Fall.Falling.CoyoteAllowed == true)
                {
                    state = FreeFormState.Coyote;
                }
                else
                {
                    state = FreeFormState.Airborne;
                }

            }
            else
            {
                airborneActions = 0;
            }
            return state;

        }

        public virtual void SetSlopeLimit(float newLimit)
        {
            slopelimit = newLimit;
            if (hasMovement) runtime.Movement.Standard.Locomotion.Locomotion.Movement.SlopeLimit = newLimit;
        }
        public virtual float GetSlopeLimit()
        {
            if (hasMovement == false) return slopelimit;
            return runtime.Movement.Standard.Locomotion.Locomotion.Movement.SlopeLimit;
        }
        public virtual void SetRotateInputVector(Vector3 rotate)
        {

            rotateInputVector = rotate;
        }
        public virtual void SetMovementInputVector(Vector3 movement)
        {
            movementInputVector = movement;
        }
        public virtual Vector3 GetRotationInputVector()
        {
            if (hasMovement == false) return rotateInputVector;
            return runtime.Movement.Standard.Locomotion.Rotate.Rotation.TranslatedRotate;
        }

        public virtual Vector3 GetInputMoveDirection()
        {
            if (hasMovement == false) return movementInputVector;
            return runtime.Movement.Standard.Locomotion.Locomotion.Movement.TranslatedMoveDirection;
        }

        public virtual Quaternion GetFacingDirection()
        {
            if (hasMovement == false) return Transform.rotation;
            return runtime.Movement.Standard.Locomotion.Rotate.Rotation.FaceDirection;
        }


        public virtual float GetRotateBufferedX()
        {
            if (hasMovement == false) return 0;
            return runtime.Movement.Standard.Locomotion.Rotate.Rotation.BufferedX;
        }

        public virtual float GetRotateBufferedZ()
        {
            if (hasMovement == false) return 0;
            return runtime.Movement.Standard.Locomotion.Rotate.Rotation.BufferedZ;
        }

        public virtual float GetSpeed()
        {
            if (hasMovement == false) return 0;
            return (runtime.Movement.Standard.Locomotion.Locomotion.Movement.GlobalInput * runtime.Movement.Standard.Locomotion.Locomotion.Movement.Speed).magnitude;
        }

        public virtual Vector3 GetBufferedMoveDirection()
        {
            if (hasMovement == false) return emptyvector;
            return runtime.Movement.Standard.Locomotion.Locomotion.Movement.BufferedMoveDir;
        }

        public virtual Vector3 GetSteps()
        {
            if (hasOutsideVel) return outsideVel;
            return emptyvector;
        }

       
        public virtual Vector3 GetFallDirection()
        {
            if (hasMovement == false) return emptyvector;
            return runtime.Movement.Standard.Locomotion.Fall.Falling.GravityDirection;
        }
        //public virtual float GetFallMulti()
        //{
        //    if (hasMovement == false) return one;
        //    return runtime.Movement.Standard.Locomotion.Fall.Falling.CurrentFallingSpeed;
        //}

        public virtual float GetGroundMulti()
        {

            if (hasMovement == false) return one;
            return runtime.Movement.Standard.Locomotion.Locomotion.Movement.Multiplier;
        }

        public virtual float GetAirMulti()
        {

            if (hasMovement == false) return one;
            return runtime.Movement.Standard.Locomotion.Locomotion.Movement.AirborneMulti;
        }

        public virtual float GetCurrentFallingVelocity()
        {
            if (hasMovement == false) return one;
            return runtime.Movement.Standard.Locomotion.Fall.Falling.CurrentFallingSpeed;
        }
        public virtual float GetFallMulti()
        {

            if (hasMovement == false) return one;
            return runtime.Movement.Standard.Locomotion.Fall.Falling.Multiplier;
        }

        public virtual float GetRotateMulti()
        {

            if (hasMovement == false) return one;
            return runtime.Movement.Standard.Locomotion.Rotate.Rotation.Multiplier;
        }

        public virtual void SetGroundMulti(float newmulti)
        {
            if (hasMovement == false) return;
            runtime.Movement.Standard.Locomotion.Locomotion.Movement.Multiplier = newmulti;
        }

        public virtual void SetAirMulti(float newmulti)
        {
            if (hasMovement == false) return;
            runtime.Movement.Standard.Locomotion.Locomotion.Movement.AirborneMulti = newmulti;
        }

        public virtual void SetFallMulti(float newmulti)
        {
            if (hasMovement == false) return;
            runtime.Movement.Standard.Locomotion.Fall.Falling.Multiplier = newmulti;
        }

        public virtual void SetRotateMulti(float newmulti)
        {
            if (hasMovement == false) return;
            runtime.Movement.Standard.Locomotion.Rotate.Rotation.Multiplier = newmulti;
        }

        public virtual bool GetGrounded()
        {
            if (hasMovement == false) return true;
            return runtime.Movement.Standard.Locomotion.Fall.Falling.IsGrounded;
        }

        public virtual void EnableActionCheck(bool isenabled)
        {
            actionCheck = isenabled;
        }
        public virtual float GetHeight()
        {
            return one;
        }

        public virtual float GetRadius()
        {
            return one;
        }

        public virtual Vector2 GetInput()
        {
            return emptyvector;
        }
        public virtual Vector3 GetCenter()
        {
            return emptyvector;
        }


        
        public List<Combos> GetMyCombos()
        {
            return ActionManager.GetMyCombos(this);
        }
        
        

        public virtual void AddActionSet(List<ActionSet> set)
        {
            List<InputActionMap> maps = new List<InputActionMap>();
            for (int i = 0; i < set.Count; i++)
            {
                bool added = AddNewAction(set[i].Action);
                if (added)
                {
                    flowControls.AddFlow(set[i].Flow);
                    maps.Add(set[i].GetActionMap());
                }
            }

            InputActionSet newSet = new InputActionSet(maps);
            inputActionMap.AddInputActionSet(newSet);

            Events.FireLoadoutAdded(new CharacterActionSetArgs(this, set));
        }
   
        

        public virtual void RemoveActionSet(List<ActionSet> set)
        {
            List<InputActionMap> maps = new List<InputActionMap>();
            for (int i = 0; i < set.Count; i++)
            {
                bool removed = RemoveAction(set[i].Action);
                if (removed)
                {
                    flowControls.RemoveFlow(set[i].Flow);
                    maps.Add(set[i].GetActionMap());
                }
            }


            InputActionSet newSet = new InputActionSet(maps);
            inputActionMap.RemoveInputActionSet(newSet);

            Events.FireLoadoutRemove(new CharacterActionSetArgs(this, set));
        }

        public virtual ActionSO GetAction(string name)
        {
            string key = CommonFunctions.StringKey(name);
            if (actions.ContainsKey(key))
            {
                return actions[key];
            }
            return ScriptableObject.CreateInstance<CharacterActionsSO>();
        }

        public virtual bool AddNewAction(string name)
        {
            string key = CommonFunctions.StringKey(name);
            if (actions.ContainsKey(key) == false)
            {
                ActionSO action = actionsdatabaseSO.Database.GetAction(key);
                if (action == null)
                {
                    Debug.LogWarning("Did you forget to put the action in the database? " + key);
                }
                else
                {
                    actions[key] = action;
                    CharacterActionArgs args = new CharacterActionArgs(this, key);
                    Events.FireActionAdded(args);
                    Events.UnityEvents.OnActionAdded?.Invoke(args);
                    return true;
                }

            }


            return false;
        }

        public virtual void RemoveAllLoadoutOverrides()
        {
            for (int i = 0; i < currentOverrides.Count; i++)
            {
                currentOverrides[i].DisableObjects();
                currentOverrides.RemoveAt(i);
            }
        }
        public virtual void RemoveAllActions()
        {
            inputActionMap.Setup();//must run setup first before clear all
            List<string> keys = actions.Keys.ToList();
            for (int i = 0; i < keys.Count; i++)
            {
                RemoveAction(keys[i]);
            }
            
        }
        public virtual bool RemoveAction(string name)
        {
            string key = CommonFunctions.StringKey(name);
            if (actions.ContainsKey(key))
            {
                Flow.RemoveFlow(key);
                actions.Remove(key);
                CharacterActionArgs args = new CharacterActionArgs(this, key);
                Events.FireActionRemoved(args);
                Events.UnityEvents.OnActionRemoved?.Invoke(args);
                return true;
            }


            return false;
        }

        /// <summary>
        /// disable loadout override objects
        /// </summary>
        /// <param name="loadoutName"></param>
        public virtual void RemoveLoadoutOverride(string loadoutName)
        {
            string otherkey = CommonFunctions.StringKey(loadoutName);

            for (int i = 0; i < currentOverrides.Count; i++)
            {
                string key = CommonFunctions.StringKey(loadoutOverrides[i].Loadout.name);
                if (CommonFunctions.WordEquals(key, otherkey))
                {
                    currentOverrides[i].DisableObjects();
                    currentOverrides.RemoveAt(i);
                }
            }
        }
        /// <summary>
        /// enabled loadout override objects
        /// </summary>
        /// <param name="loadoutName"></param>
        public virtual void AddLoadoutOverride(string loadoutName)
        {

            string otherkey = CommonFunctions.StringKey(loadoutName);

            for (int i = 0; i < currentOverrides.Count; i++)
            {
                if (CommonFunctions.WordEquals(otherkey, currentOverrides[i].Name))
                {
                    Debug.Log("Already have this loadout override enabled");
                    return;
                }
            }


            for (int i = 0; i < loadoutOverrides.Count; i++)
            {
                string key = CommonFunctions.StringKey(loadoutOverrides[i].Loadout.LoadoutName);
                if (CommonFunctions.WordEquals(key, otherkey))
                {
                    RuntimeInstanceOverrides runtime = new RuntimeInstanceOverrides(key, loadoutOverrides[i].SyncedObjects);
                    runtime.EnableObjects();
                    currentOverrides.Add(runtime);
                    
                }
            }
            
        }
        protected virtual void Awake()
        {
          
            ResetCharacter();
      
            if (persist)
            {
                if (playerControlled)
                {
                    if (PlayerCharacterManager.HasPlayer(playerNumber))
                    {
                        if (PlayerCharacterManager.GetPlayer(playerNumber) != null && PlayerCharacterManager.GetPlayer(playerNumber) != this)
                        {
                            Destroy(this.gameObject);
                        }
                    }
                    else
                    {
                        PlayerCharacterManager.AddPlayer(playerNumber, this);
                        DontDestroyOnLoad(this.gameObject);
                        SceneManager.sceneLoaded += NewScene;

                    }
                }
                else
                {
                    PlayerCharacterManager.AddPlayer(playerNumber, this);
                }

            }



        }

        protected virtual void Start()
        {
            Initialization();

        }

        public virtual void Initialization()
        {
            if (hitboxes != null)
            {
                hitboxes.Setup(AnimationEvents);
            }

            //checks if animator states exist
            if (animController != null)
            {
                Animator anim = animController.GetAnimator();
                ActionManager.CheckAnimatorStates(anim, actionsdatabaseSO);

            }

            if (persist)
            {
                ActorHitBoxManager.PersistSub += Resub;
            }

            ReloadActionsAndInputs();

            ResetLocomotion();
            EnableActionCheck(true);
            Events.FireSetupComplete(new CharacterArgs(this));
            Events.UnityEvents.OnSetupComplete.Invoke(this);
        }

        public virtual void ReloadActionsAndInputs()
        {
            flowControls = ScriptableObject.CreateInstance<FlowControlSO>();
            if (inputActionMap != null)
            {
                inputActionMap.Setup();//must run first
            }
           
            flowControls.SetCurrent(string.Empty);
            flowControls.Setup(this);

            for (int i = 0; i < loadoutOverrides.Count; i++)
            {
                List<GameObject> objects = loadoutOverrides[i].SyncedObjects;
                for (int j = 0; j < objects.Count; j++)
                {
                    objects[j].SetActive(false);
                }
            }

            if (loadOutSO != null)
            {
                ActionManager.AddActionSet(this, loadOutSO, ComboDynamicType.ReplaceAll);
            }
        }






        protected virtual void OnDestroy()
        {
            Destruction();
        }

        public virtual void Destruction()
        {
            if (lockstate != null) lockstate.RemoveTicker(); lockstate = null;


            RemoveLocotion();
            if (playerControlled)
            {
                if (PlayerCharacterManager.HasPlayer(playerNumber) && PlayerCharacterManager.GetPlayer(playerNumber) == this && persist)
                {
                    ActorHitBoxManager.PersistSub -= Resub;
                    SceneManager.sceneLoaded -= NewScene;
                    PlayerCharacterManager.RemovePlayer(playerNumber);
                }
            }
            if (hasHealthSystem)
            {
                if (takeDamageController != null)
                {
                    takeDamageController.OnDied -= Death;

                }


            }
            ActionManager.UnRegister(id);
        }

        protected virtual void ResetLocomotion()
        {
            RemoveLocotion();
            InitializeLocomotion();
        }

        protected virtual bool CheckFilter(string actionName)
        {


            string key = CommonFunctions.StringKey(actionName);

            if (filteredactions.Count > 0)
            {
                bool allow = false;
                for (int i = 0; i < filteredactions.Count; i++)
                {
                    if (CommonFunctions.WordEquals(key, filteredactions[i]))
                    {
                        //allow
                        allow = true;
                        break;
                    }
                }

                return allow;

            }
            else
            {
                return true;
            }
        }

        public virtual void ForceAction(string actionName)
        {
            if (string.IsNullOrWhiteSpace(actionName)) return;
            ActionManager.TryStartAction(this, actionName, true);
            EnableActionCheck(false);
            CharacterActionArgs args = new CharacterActionArgs(this, actionName);
            Events.FireActionStarted(args);
            Events.UnityEvents.OnActionSequenceStarted?.Invoke(args);
        }
        public virtual bool TryStartActionSequence(string actionName)
        {
            if (string.IsNullOrWhiteSpace(actionName)) return false;

            bool checkfilter = CheckFilter(actionName);

            if (checkfilter == false)
            {
                //return
                return false;
            }

            bool starated = ActionManager.TryStartAction(this, actionName);
            if (starated)
            {
                EnableActionCheck(false);
                CharacterActionArgs args = new CharacterActionArgs(this, actionName);
                Events.FireActionStarted(args);
                Events.UnityEvents.OnActionSequenceStarted?.Invoke(args);
            }
            return starated;

        }


        public virtual void ActionSequenceEnded(string actionName)
        {
   
            EnableActionCheck(true);
            CharacterActionArgs args = new CharacterActionArgs(this, actionName);
            Events.FireActionEnded(args);
            Events.UnityEvents.OnActionSequenceEnded?.Invoke(args);

        }
        public virtual void SetLockonInput(InputActionSlotSO input)
        {
            lockOn.Input = input;
            hasLockOn = lockOn.Input != null;
        }
        public virtual void SetStrafeInput(InputActionSlotSO input)
        {
            strafe.Input = input;
            hasStrafe = strafe.Input != null;
        }


        protected virtual void Death(ActionCharacter ac)
        {
            if (disableHitBoxesOnDeath)
            {
                hitboxes.CloseDown();
                hitboxes.CloseDownLocoCollider();
            }
        }

       
        public virtual void ResetCharacter()
        {
            ActionManager.UnRegister(id);
            ActionManager.Register(this);
            if (inputActionMap != null)
            {
                SetNewActionMap(ScriptableObject.Instantiate(inputActionMap));
            }
            else
            {
                SetNewActionMap(null);
            }
            if (InputWrapper != null)
            {
                SetNewInputWrapper(ScriptableObject.Instantiate(InputWrapper));
            }
            else
            {
                SetNewInputWrapper(null);
            }
  

            if (movementTemplate != null)
            {
                runtime = Instantiate(movementTemplate);

            }
            hasMovement = runtime != null;
            SetGroundMulti(Config.Defaults.MoveMulti);
            SetRotateMulti(Config.Defaults.RotateMulti);
            SetFallMulti(Config.Defaults.FallMulti);
            SetAirMulti(Config.Defaults.AirborneMulti);

            hasLockOn = lockOn.Input != null;
            hasStrafe = strafe.Input != null;

            hitboxes = GetComponent<ActorHitBoxes>();
            takeDamageController = GetComponent<ITakeDamage>();
            hasHealthSystem = takeDamageController != null;
            if (hasHealthSystem)
            {
                takeDamageController.OnDied -= Death;
                takeDamageController.OnDied += Death;

            }
            t = this.transform;
            id = t.GetInstanceID();
            if (anim == null)
            {
                anim = GetComponentInChildren<Animator>();
            }
            if (anim != null)
            {
                ikController = anim.gameObject.GetComponent<IActionIK>();
                if (ikController == null)
                {
                    ikController = anim.gameObject.AddComponent<IKController>();
                }
                ikController.SetControllerObject(this.transform);
             
               
                rootmotion = anim.gameObject.GetComponent<IRootMotion>();
                if (rootmotion == null)
                {
                    rootmotion = anim.gameObject.AddComponent<RootMotionScript>();
                }
                rootmotion.SetControllerObject(this.transform);
                events = anim.gameObject.GetComponent<IAnimationEvents>();
                if (events == null)
                {
                    events = anim.gameObject.AddComponent<AnimationEventScript>();
                }
                animController = anim.gameObject.GetComponent<IAnimatorController>();
                if (animController == null)
                {
                    animController = anim.gameObject.AddComponent<AnimatorControllerScript>();
                }
            }

            
        }


        Vector3 lastforward;
        protected bool updateAnim = true;
        
       
        public virtual void SetAnimatorValues()
        {
            if (hasMovement == false)
            {
                return;
            }

            

            float dotz = 0;
            float dotx = 0;
            float speed = new Vector3(runtime.Movement.Standard.Locomotion.Locomotion.Movement.X, 0, runtime.Movement.Standard.Locomotion.Locomotion.Movement.Z).normalized.magnitude;
            
            switch (runtime.Movement.Standard.Locomotion.Locomotion.Movement.Reference)
            {
                case InputReference.Local:
                    dotz = Vector3.Dot(runtime.Movement.Standard.Locomotion.Locomotion.Movement.LocalInput, transform.forward);
                    dotx = Vector3.Dot(runtime.Movement.Standard.Locomotion.Locomotion.Movement.LocalInput, transform.right);
                    break;
                case InputReference.World:
                    dotz = Vector3.Dot(runtime.Movement.Standard.Locomotion.Locomotion.Movement.GlobalInput, transform.forward);
                    dotx = Vector3.Dot(runtime.Movement.Standard.Locomotion.Locomotion.Movement.GlobalInput, transform.right);
                    break;
                case InputReference.Camera:
                    dotz = Vector3.Dot(runtime.Movement.Standard.Locomotion.Locomotion.Movement.CameraInput, transform.forward);
                    dotx = Vector3.Dot(runtime.Movement.Standard.Locomotion.Locomotion.Movement.CameraInput, transform.right);
                    break;
            }




           if (GetRotateType() == RotateType.Free)
            {
                dotz = speed;
                dotx = 0;
            }

            AnimatorController.SetBoolParam(config.Defaults.IsGrounded, runtime.Movement.Standard.Locomotion.Fall.Falling.IsGrounded);
            AnimatorController.SetFloatParam(config.Defaults.VelocityZ, Mathf.Clamp(dotz, -1f, 1f));
            AnimatorController.SetFloatParam(config.Defaults.VelocityX, Mathf.Clamp(dotx, -1f, 1f));

            AnimatorController.SetBoolParam(config.Defaults.Idling, dotz == 0 && dotx == 0 && speed == 0);
            AnimatorController.SetFloatParam(config.Defaults.Speed, speed);
            if (hasHealthSystem)
            {
                AnimatorController.SetBoolParam(config.Defaults.DeadKey, takeDamageController.IsDead);
            }
            

        }
        public List<string> GetHitGivers()
        {
            return config.HitGiverBoxes;
        }

        public List<string> GetHitTakers()
        {
            return config.HitTakerBoxes;
        }

        public void SetHitTakers(List<string> takers)
        {
            config.HitTakerBoxes = takers;
        }

        public void SetHitGivers(List<string> givers)
        {
            config.HitGiverBoxes = givers;
        }

        
    }
}