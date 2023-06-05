

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace GWLPXL.ActionCharacter
{
    /// <summary>
    /// The action logic 
    /// </summary>
    public class ActionTickerCC : ActionTicker
    {

        public bool HasTarget => hasTarget;
        public ActionCCVars Vars => ccvars;
        public Vector3 Target => target;
        public ActionCharacter Character => instance;

        #region protected fields
        protected ActionCCVars ccvars;
        protected IAnimatorController animator;
        protected ActivateHitBox hitbox;
        protected Vector3 start;
        protected Vector3 goal;
        protected float timer;

        protected float goalx = 0;
        protected float goaly = 0;
        protected float goalz = 0;

        protected int nextmove = -1;
        protected bool normalized = false;
        protected float normalizedx;
        protected float normalziedy;
        protected float normalizedz;
        protected bool hasXCurve = false;
        protected bool hasYCurve = false;
        protected bool hasZCurve = false;
        protected bool stopmovex = false;
        protected bool stopmovey = false;
        protected bool stopmovez = false;
        protected float x = 0;
        protected float y = 0;
        protected float z = 0;
        protected float totald = 0;
        protected bool extend;
        protected RaycastHit hit;
        protected Vector3 inputaccum = new Vector3(0, 0, 0);
        protected Vector3 lastStep = new Vector3(0, 0, 0);

        protected bool complete;

        #region movement
        protected Vector3 lerpmove = new Vector3(0, 0, 0);
        protected Vector3 zero = new Vector3(0, 0, 0);
      

        #endregion
        #region check blocking
        protected Vector3 floor;
        protected Vector3 head;
        protected Vector3 headoffset;
        protected float radius = 1;
        protected float height = 1;
        protected Vector3 center;
        protected Collider[] overlaps = new Collider[0];
        protected bool blockexit = false;
        protected Vector3 blockdir;
        #endregion

        #region early exits
        protected CharacterActionsSO extras1;
        protected bool exitenabled = false;
        protected bool earlyExit = false;
        protected bool hasExitState = false;
        protected string exitState = string.Empty;
        protected List<string> earlyExits = new List<string>();
        #endregion
        protected bool customcodeini = false;
        protected bool hasCustomCode = false;
        protected float normalizedTarget;
        protected Vector3 target;
        protected bool initialized;
        protected bool animinitalized;
        protected bool hasTarget;
        protected Transform t1;
        protected float previoustimer;
        protected bool isPlayer = false;
        protected int instanceId = 0;
        protected bool animready = false;
        protected AnimatorStateInfo state;
        protected int framecount = 0;
        protected bool block;


        protected List<CustomActionCodeSO> customCodeAssets = new List<CustomActionCodeSO>();

        #endregion

        #region constructor
        public ActionTickerCC(ActionCharacter controller, ActionCCVars vars) : base(controller, vars)
        {
            this.ccvars = vars;
            this.instance = controller;
            this.animator = controller.AnimatorController;
            this.instanceId = controller.ID;
            complete = false;
            isPlayer = controller as PlayerCharacter != null;

            height = instance.Height;
            radius = instance.Radius;
            center = instance.Center;

            headoffset = new Vector3(0, height, 0);

            t1 = instance.transform;
        }

        #endregion

        #region Iticker interface override

        public override void Tick()
        {
            Behavior();

        }


        #endregion

        public override void Pause()
        {
            base.Pause();
            //animator.SetAnimatorSpeed(0);
            animator.SetFloatParam(instance.Config.Defaults.AnimSpeedParam, 0);
        }

        public override void UnPause()
        {
            base.UnPause();
            animator.SetFloatParam(instance.Config.Defaults.AnimSpeedParam, ccvars.AnimatorVars.AnimatorSpeed);
           // animator.SetAnimatorSpeed(1);
        }

        public override void SetTimerMulti(float newMulit)
        {
            base.SetTimerMulti(newMulit);
            animator.SetFloatParam(instance.Config.Defaults.AnimSpeedParam, newMulit);
        }

        float wait = 0;
        protected virtual void Behavior()
        {
            if (complete) RemoveTicker();
            wait += GetTickDuration() * timerMulti;
            framecount++;
            AnimationInitialization();

            if (framecount == 1) return;
            framecount = 2;

      
            TargetInitialization();//target must come first
            InitializeTimers();
            InitializeCustomCode();

           

            float dt = GetTickDuration() * timerMulti;
            timer += dt;
            timer = Mathf.Clamp(timer, 0, totald);

            percent = timer / totald;
            CollectActionInputs(percent);
            CheckExtending(percent, dt);
            CheckEarlyExit(percent);
            CheckHitBoxes(percent);
            CheckBlocking();
            CheckCustomCode(percent);

            

            if (blockexit)
            {
                ActionManager.EndAction(instance, this, true);
            }


            if (ccvars.AnimatorVars.ApplyRootMotion == false)//only move if no root.
            {


                Vector3 dir = CalculateMovement();

                Movement(dir);

                lastStep = dir;

            }





            previoustimer = timer;

           

            if (earlyExit == true)
            {
                if (hasExitState == false)
                {
                    ActionManager.EndAction(instance, this, true);
                }
                else
                {
                    ActionManager.TransitionAction(instance, this, exitState);
                }
            }

            if (extend == false)
            {
                CheckNaturalExit();
            }



        }


        protected virtual void CheckBlocking()
        {
            blockdir = instance.RootMotionController.GetRootMotionDirection();
            block = CheckInitialBlocking();
            if (block) instance.RootMotionController.SetRootMotionActive(false);
           
        }

        protected virtual void CheckNaturalExit()
        {
            state = animator.GetStateInfo(ccvars.AnimatorVars.Layer);

            InputBufferMap map = ActionManager.GetInputBufferMap(instanceId);
            if (map.InputSlotIndex > -1)
            {
                if (timer >= early || allowComboExit)
                {
                    ActionManager.TransitionAction(instance, this, map.ActionName);
                }
     
            }
           else if (timer >= totald)
            {
                complete = true;
                if (map.InputSlotIndex > -1)
                {
                    ActionManager.TransitionAction(instance, this, map.ActionName);
                }
                else
                {
                    ActionManager.ContinueActionSequence(instance, this);
                }
         
            }

        }

        Vector3 fallaccum;

        /// <summary>
        /// only works when target available, doesn't take into account dynamic inputs
        /// </summary>
        /// <param name="percent"></param>
        /// <returns></returns>
        public virtual Vector3 GetPositionAt(float percent)
        {
            return Vector3.Lerp(start, target, percent);
        }
        protected virtual Vector3 CalculateMovement()
        {
            if (block)
            {
                goalx = 0;
                goaly = 0;
                goalz = 0;
                return zero;
            }

            float percentX = timer / normalizedx;
            float percentY = timer / normalziedy;
            float percentZ = timer / normalizedz;

            if (hasTarget == false)
            {

                if (stopmovex == false)
                {
                    x = ActionManager.Calculate(ccvars.TravelX, percentX, goalx, hasXCurve);
                    if (percentX >= 1)
                    {
                        stopmovex = true;
                    }
                }


                if (stopmovey == false)
                {
                    y = ActionManager.Calculate(ccvars.TravelY, percentY, goaly, hasYCurve);
                    if (percentY >= 1)
                    {
                        stopmovey = true;
                    }
                }



                if (stopmovez == false)
                {
                    z = ActionManager.Calculate(ccvars.TravelZ, percentZ, goalz, hasZCurve);
                    if (percentZ >= 1)
                    {
                        stopmovez = true;
                    }
                }




                Vector3 step = ActionManager.GetStep(start, t1, x, y, z);
                lerpmove = step;


            }
            else
            {
                //controller.transform.forward = target - start;

                float percent = ccvars.TargetOptions.TargetCurve.Evaluate(timer / normalizedTarget);
                lerpmove = Vector3.Lerp(start, target, percent);
            }



     

            FreeFormState state = instance.GetCharacterStateAC();
            switch (state)
            {
                case FreeFormState.Airborne:
                    fallaccum +=  instance.GetFallDirection() * instance.GetCurrentFallingVelocity() * instance.GetFallMulti();
                    inputaccum += instance.GetInputMoveDirection() * instance.GetAirMulti();
                    break;
                case FreeFormState.Ground:
                    inputaccum += instance.GetInputMoveDirection() * instance.GetGroundMulti();
                    fallaccum += instance.GetFallDirection() * instance.GetCurrentFallingVelocity() * instance.GetFallMulti();
                    break;
                case FreeFormState.Coyote:
                    inputaccum += instance.GetInputMoveDirection() * instance.GetGroundMulti();
                    fallaccum += instance.GetFallDirection() * instance.GetCurrentFallingVelocity() * instance.GetFallMulti();
                    break;
            }

            
            Vector3 dir = (lerpmove - t1.position) + inputaccum + fallaccum;

            blockdir = dir;
           
           block = CheckInitialBlocking();
            if (block)
            {
                Debug.Log("Blocking");
            }

            return dir;


        }
        protected virtual void Movement(Vector3 dir)
        {
            instance.AddStep(dir);
           

        }

        #region methods
        

        protected virtual void InitializeCustomCode()
        {
            if (customcodeini) return;

            for (int i = 0; i < ccvars.CustomCodeAssets.CustomCodes.Count; i++)
            {
                if (ccvars.CustomCodeAssets.CustomCodes[i] == null) continue;
                ccvars.CustomCodeAssets.CustomCodes[i].Initialize(this);
                customCodeAssets.Add(ccvars.CustomCodeAssets.CustomCodes[i]);
            }

            customcodeini = true;
            if (customCodeAssets.Count > 0)
            {
                hasCustomCode = true;
            }
            else
            {
                hasCustomCode = false;
            }
        }

        protected virtual void CheckCustomCode(float percent)
        {
            if (hasCustomCode == false) return;

            for (int i = 0; i < customCodeAssets.Count; i++)
            {
                CustomActionCodeSO custom = customCodeAssets[i];
                float start = customCodeAssets[i].GetCustomCode().GetStartPercent();
                float end = customCodeAssets[i].GetCustomCode().GetEndPercent();
                if (percent >= start && percent <= end)
                {
                    custom.Tick(this);
                    CustomCodeType type = custom.GetCustomCode().GetCodeType();
                    switch (type)
                    {
                        case CustomCodeType.Once:
                            customCodeAssets.RemoveAt(i);
                            break;
                    }
                    
                }
            }
        }

        public virtual void ReCalculateTimers(float atSpeed)
        {
            normalized = false;
            animator.SetFloatParam(instance.Config.Defaults.AnimSpeedParam, atSpeed);
        }

        float early = 1f;
        protected virtual void InitializeTimers()
        {
            if (normalized == true) return;
            normalized = true;

            instance.SetGroundMulti(ccvars.Multipliers.MoveMulti);
            instance.SetAirMulti(ccvars.Multipliers.AirborneMulti);
            instance.SetRotateMulti(ccvars.Multipliers.RotateMulti);
            instance.SetFallMulti(ccvars.Multipliers.FallMulti);

            AnimatorStateInfo state = animator.GetStateInfo(vars.AnimatorVars.Layer);
            float length = 0;
            if (ccvars.AnimatorVars.Clip == null)
            {
                length = state.length;
            }
            else
            {
                length = ccvars.AnimatorVars.Clip.length;
            }

            if (string.IsNullOrEmpty(instance.Config.Defaults.AnimSpeedParam))
            {
                totald = (length);
            }
            else
            {
                totald = ((length) * (1 / animator.GetFloatParam(instance.Config.Defaults.AnimSpeedParam)));
            }

            totald += ccvars.TimingOptions.AdditionalDuration + wait + ccvars.AnimatorVars.CrossFadeDuration;

            early = ccvars.TimingOptions.MinActionDuration * totald;

            if (ccvars.TargetOptions.AcquireTargetType != AcquireTargetType.None)
            {
                normalizedTarget = totald * ccvars.TargetOptions.AcquireDuration;
            }

            normalizedx = totald;
            normalziedy = totald;
            normalizedz = totald;

            if (ccvars.TravelX.Duration > 0)
            {
                normalizedx = ccvars.TravelX.Duration * totald;
            }


            if (ccvars.TravelY.Duration > 0)
            {
                normalziedy = ccvars.TravelY.Duration * totald;
            }


            if (ccvars.TravelZ.Duration > 0)
            {
                normalizedz = ccvars.TravelZ.Duration * totald;
            }


            for (int i = 0; i < ccvars.HitBoxOptions.GiverOptions.Boxes.Count; i++)
            {
                ActivateHitBox hitbox = ccvars.HitBoxOptions.GiverOptions.Boxes[i];
                hitbox.EnableRealTime = hitbox.EnableNormalizedTime * totald;
                hitbox.DisableRealTime = hitbox.DisableNormalizedTime * totald;
                hitbox.IsEnabled = false;
            }

            for (int i = 0; i < ccvars.HitBoxOptions.TakerOptions.Boxes.Count; i++)
            {
                ActivateHitBox hitbox = ccvars.HitBoxOptions.TakerOptions.Boxes[i];
                hitbox.EnableRealTime = hitbox.EnableNormalizedTime * totald;
                hitbox.DisableRealTime = hitbox.DisableNormalizedTime * totald;
                hitbox.IsEnabled = false;
            }





        }
        protected virtual void TargetInitialization()
        {
            if (initialized) return;

            if (isPlayer)
            {
                ActionManager.UpdateInputBuffer(instanceId, new InputBufferMap(-1, ""));//reset input
            }
            start = t1.position;

            initialized = true;

            instance.transform.rotation = instance.GetFacingDirection();

            if (instance.HasLockOnTarget)
            {
                instance.transform.forward = Vector3.ProjectOnPlane(instance.LockOnTarget.position - instance.Transform.position, Vector3.up);
            }

            if (instance.HasLockOnTarget)
            {
                Vector3 dir = new Vector3(0, 0, 0);
                hasTarget = false;
                switch (ccvars.TargetOptions.AcquireTargetType)
                {
                    case AcquireTargetType.MoveToTarget:
                        switch (ccvars.TargetOptions.AcquireFinalLocation)
                        {
                            case AcquireType.ClosestSpot:
                                //
                                dir = instance.Transform.position - instance.LockOnTarget.position + instance.Center;
                                target = instance.LockOnTarget.position + (dir.normalized * ccvars.TargetOptions.AcquireDistance);
                                hasTarget = true;
                                break;
                            case AcquireType.FurthestSpot:
                                dir = instance.LockOnTarget.position - instance.Transform.position;
                                target = instance.LockOnTarget.position + (dir.normalized * ccvars.TargetOptions.AcquireDistance);
                                hasTarget = true;
                                break;
                            case AcquireType.TargetBack:
                                target = Detection.ConvertDirection(DirectionType.Back, instance.LockOnTarget) * ccvars.TargetOptions.AcquireDistance + instance.LockOnTarget.position;
                                hasTarget = true;
                                break;
                            case AcquireType.TargetFront:
                                target = Detection.ConvertDirection(DirectionType.Front, instance.LockOnTarget) * ccvars.TargetOptions.AcquireDistance + instance.LockOnTarget.position;
                                hasTarget = true;
                                break;
                            case AcquireType.TargetLeft:
                                target = Detection.ConvertDirection(DirectionType.Left, instance.LockOnTarget) * ccvars.TargetOptions.AcquireDistance + instance.LockOnTarget.position;
                                hasTarget = true;
                                break;
                            case AcquireType.TargetRight:
                                target = Detection.ConvertDirection(DirectionType.Right, instance.LockOnTarget) * ccvars.TargetOptions.AcquireDistance + instance.LockOnTarget.position;
                                hasTarget = true;
                                break;
                        }

                        break;
                    case AcquireTargetType.MoveAwayFromTarget:
                        {
                            switch (ccvars.TargetOptions.AcquireFinalLocation)
                            {
                                case AcquireType.ClosestSpot:
                                    //
                                    dir = instance.Transform.position - instance.LockOnTarget.position + instance.Center;
                                    target = instance.LockOnTarget.transform.position + -(dir.normalized * ccvars.TargetOptions.AcquireDistance);
                                    hasTarget = true;
                                    break;
                                case AcquireType.FurthestSpot:
                                    dir = instance.Transform.position - instance.LockOnTarget.position + instance.Center;
                                    target = instance.LockOnTarget.transform.position + -(dir.normalized * ccvars.TargetOptions.AcquireDistance);
                                    hasTarget = true;
                                    break;
                                case AcquireType.TargetBack:
                                    target = -Detection.ConvertDirection(DirectionType.Back, instance.LockOnTarget) * ccvars.TargetOptions.AcquireDistance + instance.LockOnTarget.position;
                                    hasTarget = true;
                                    break;
                                case AcquireType.TargetFront:
                                    target = -Detection.ConvertDirection(DirectionType.Front, instance.LockOnTarget) * ccvars.TargetOptions.AcquireDistance + instance.LockOnTarget.position;
                                    hasTarget = true;
                                    break;
                                case AcquireType.TargetLeft:
                                    target = -Detection.ConvertDirection(DirectionType.Left, instance.LockOnTarget) * ccvars.TargetOptions.AcquireDistance + instance.LockOnTarget.position;
                                    hasTarget = true;
                                    break;
                                case AcquireType.TargetRight:
                                    target = -Detection.ConvertDirection(DirectionType.Right, instance.LockOnTarget) * ccvars.TargetOptions.AcquireDistance + instance.LockOnTarget.position;
                                    hasTarget = true;
                                    break;
                            }
                            break;
                        }
                }


                if (hasTarget)
                {
                    normalizedTarget = totald * ccvars.TargetOptions.AcquireDuration;
                }
            }
            else if (ccvars.TargetOptions.AcquireTargetType != AcquireTargetType.None && instance.HasLockOnTarget)
            {
                Debug.Log("Trying to acquire a target but character has none.");
            }
           

            if (hasTarget == false)
            {
                SetdistanceGoal();
            }

            timer = 0;
            totald = 0;

            goal = start + (t1.transform.right * goalx) + (t1.transform.up * goaly) + (t1.transform.forward * goalz);

            previoustimer = timer;

        }
       

        protected virtual void AnimationInitialization()
        {
            if (animinitalized) return;
            animinitalized = true;

            block = CheckInitialBlocking();
            if (block)
            {
                instance.RootMotionController.SetRootMotionActive(false);
                if (ccvars.BlockingOptions.InitialBlocking.EndActionOnBlocked)
                {
                    blockexit = true;

                }
            }
            else
            {
                instance.RootMotionController.SetRootMotionActive(ccvars.AnimatorVars.ApplyRootMotion);
            }



            animator.SetFloatParam(instance.Config.Defaults.AnimSpeedParam, ccvars.AnimatorVars.AnimatorSpeed);
            animator.PlayCrossFadeFixed(ccvars.AnimatorVars.AnimatorStateName, ccvars.AnimatorVars.CrossFadeDuration, ccvars.AnimatorVars.Layer, 0);



        }
        protected virtual void SetdistanceGoal()
        {

            if (ccvars.TravelX.Duration > 0)
            {
                goalx =  ccvars.TravelX.Distance;
                hasXCurve = ccvars.TravelX.Curve != null;
            }
            else
            {
                stopmovex = true;
            }


            if (ccvars.TravelY.Duration > 0)
            {
                goaly =   ccvars.TravelY.Distance;
                hasYCurve = ccvars.TravelY.Curve != null;
            }
            else
            {
                stopmovey = true;
            }


            if (ccvars.TravelZ.Duration > 0)
            {
                goalz = ccvars.TravelZ.Distance;
                hasZCurve = ccvars.TravelZ.Curve != null;
            }else
            {
                stopmovez = true;
            }
        }
        #endregion

        #region resets
        public override void ResetActionObject()
        {
            if (instance != null)
            {
                DisableAllBoxes();
                if (exitenabled)
                {
                    instance.Flow.EnableEarlyExits(false);
                    exitenabled = false;
                }
       
                instance.RootMotionController.SetRootMotionActive(instance.Config.Defaults.UseRoot);
                animator.SetFloatParam(instance.Config.Defaults.AnimSpeedParam, instance.Config.Defaults.AnimatorSpeed);
            }


            hasTarget = false;
            if (ccvars.TargetOptions.KeepTargetOnComplete == false)
            {
                instance.SetLockOnTarget(null);
               
            }
           
            
            instance.SetRotateType(instance.Config.Defaults.RotateType);

            instance.SetGroundMulti(instance.Config.Defaults.MoveMulti);
            instance.SetRotateMulti(instance.Config.Defaults.RotateMulti);
            instance.SetFallMulti(instance.Config.Defaults.FallMulti);
            instance.SetAirMulti(instance.Config.Defaults.AirborneMulti);




        }
        protected virtual void DisableAllBoxes()
        {
            instance.AnimationEvents.DisableAllHitGivers();
            instance.AnimationEvents.DisableAllHitTakers();
        }
        #endregion

        #region hitboxes
        protected virtual void CheckHitBoxes(float percent)
        {


            for (int i = 0; i < ccvars.HitBoxOptions.GiverOptions.Boxes.Count; i++)
            {
                hitbox = ccvars.HitBoxOptions.GiverOptions.Boxes[i];
                ManuallyActivateHitboxes(percent, hitbox, true);

            }



            for (int i = 0; i < ccvars.HitBoxOptions.TakerOptions.Boxes.Count; i++)
            {
                hitbox = ccvars.HitBoxOptions.TakerOptions.Boxes[i];
                ManuallyActivateHitboxes(percent, hitbox, false);
            }
        }

        protected virtual void ManuallyActivateHitboxes(float percent, ActivateHitBox hitbox, bool hitGiver)
        {
            if (percent >= hitbox.EnableNormalizedTime && percent < hitbox.DisableNormalizedTime && hitbox.IsEnabled == false)
            {
                hitbox.IsEnabled = true;
                if (hitGiver)
                {
                    instance.AnimationEvents.EnableHitGiver(hitbox.HitBoxName);
                }
                else
                {
                    instance.AnimationEvents.EnableHitTaker(hitbox.HitBoxName);
                }

                //turn it on
            }

            if (percent >= hitbox.DisableNormalizedTime && hitbox.IsEnabled == true)
            {
                hitbox.IsEnabled = false;
                if (hitGiver)
                {
                    instance.AnimationEvents.DisableHitGiver(hitbox.HitBoxName);
                }
                else
                {
                    instance.AnimationEvents.DisableHitTaker(hitbox.HitBoxName);
                }

                //disable
            }
        }

        #endregion

        #region player inputs
        protected virtual void CollectActionInputs(float percent)
        {
            if (isPlayer == true)
            {
                if (percent >= ccvars.BufferVars.MinBufferInputNormalizedTime && percent <= ccvars.BufferVars.MaxBufferInputNormalziedTime)
                {
                    ActionManager.GetActionInputRequirements(instance as PlayerCharacter, true);

                }
                

            }


        }
        #endregion

        #region player input extending
        /// <summary>
        /// 
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="dt"></param>
        protected virtual void CheckExtending(float percent, float dt)
        {
            if (isPlayer == true)
            {
                if (ccvars.ExtendOptions.UseExtend)
                {
                    


                    InputBufferMap mymap = ActionManager.GetActionMap(instanceId);
                    
                    //is always false...
                    extend = ActionManager.GetInputRequirements(
                    instance,
                    mymap.InputSlotIndex, mymap.ActionName);

           

                    if (ccvars.ExtendOptions.ShouldExitOnFail && extend == false)
                    {
                        timer = totald;
                    }
                }
            }
           
        }
        #endregion

        #region blocking
 



       


    
        protected bool CheckInitialBlocking()
        {
            floor = t1.position + Vector3.up * instance.GetStepHeight();
            head = t1.position + headoffset;

            InitialBlockingConditions initial = ccvars.BlockingOptions.InitialBlocking;
            List<DirectionType> types = initial.DirectionChecks;

            if (types.Count <= 0) return false;

            for (int i = 0; i < types.Count; i++)
            {
                if (types[i] == DirectionType.None)
                {
                    return false;
                }
            }


            for (int i = 0; i < types.Count; i++)
            {
                Vector3 dir = Detection.ConvertDirection(types[i], t1);
                int colls = Detection.CapsuleOverlapAllNonAlloc(floor + dir, head + dir, radius/2, ccvars.BlockingOptions.InitialBlocking.BlockingLayers);

                if (colls > 0)
                {
                    return true;
                }

              
            }
            
            blockdir = instance.RootMotionController.GetRootMotionDirection();
            bool blocked = Detection.SimpleCapsuleCast(floor, head, radius, blockdir, ccvars.BlockingOptions.InitialBlocking.DistanceCheck, ccvars.BlockingOptions.InitialBlocking.BlockingLayers);

            return blocked;
            //int col = Detection.CapsuleOverlapAllNonAlloc(floor + blockdir, head + blockdir, radius / 2, ccvars.BlockingOptions.InitialBlocking.BlockingLayers);
            //if (col > 0)
            //{
            //    return true;
            //}
            //return false;


        }

       



        
        #endregion

        #region early exits
        protected virtual void CheckEarlyExit(float percent)//curently doesnt check early exit states
        {

            FreeFormState state = instance.GetCharacterStateAC();
            for (int i = 0; i < ccvars.EarlyExitOptions.EarlyExitStates.Count; i++)
            {
                if (state == ccvars.EarlyExitOptions.EarlyExitStates[i].State && percent >= ccvars.EarlyExitOptions.EarlyExitStates[i].CanExitTime)
                {
                    earlyExit = true;
                    hasExitState = false;
                }
            }

            if (percent >= ccvars.EarlyExitOptions.EarlyExits.CanExitTime && exitenabled == false)
            {
                exitenabled = true;
                instance.Flow.EnableEarlyExits(true);
            }


            if (exitenabled)
            {
                earlyExits = instance.Flow.GetEarlyExits(ActionManager.GetActionName(instanceId));
                if (ActionManager.GetInputBufferMap(instanceId).InputSlotIndex > -1)
                {
                    InputBufferMap map = ActionManager.GetInputBufferMap(instanceId);
                    string currentinput = CommonFunctions.StringKey(map.ActionName);

                    for (int i = 0; i < earlyExits.Count; i++)
                    {
                        string early = CommonFunctions.StringKey(earlyExits[i]);
                        if (CommonFunctions.WordEquals(early, currentinput))
                        {
                            earlyExit = true;
                            hasExitState = true;
                            exitState = map.ActionName;
                            break;
                        }
                    }
                    
                }
            }
           

        }
        #endregion

    }

    #region helpers

    public enum AcquireTargetType
    {
        None = 0,
        MoveToTarget = 1,
        MoveAwayFromTarget = 2
    }
    public enum TargetType
    {
        None = 0,
        LockTarget = 1,
        LockPosition = 2
    }

    public enum AcquireType
    {
        None = 0,
        ClosestSpot = 1,
        FurthestSpot = 2,
        TargetFront = 3,
        TargetBack = 4,
        TargetRight = 5,
        TargetLeft = 6
    }
    [System.Serializable]
    public class TargetingOptions
    {
        [Header("After Action")]
        [Tooltip("Keep target after action?")]
        public bool KeepTargetOnComplete = true;
        public AcquireTargetType AcquireTargetType = AcquireTargetType.None;
        public float AcquireDistance = 1;
        public AcquireType AcquireFinalLocation = AcquireType.None;//rethink directions, we dont probably care about the target's back but instead target closest or behind the target or to right or left of target
        [Tooltip("Normalized time to reach the target. At 0.5f, that means we reach the target at 50% of the action length.")]
        [Range(0f, 1f)]
        public float AcquireDuration = .75f;
        [Tooltip("Accelerate curve for acquiring the target. ")]
        public AnimationCurve TargetCurve;

    }
    /// <summary>
    /// distance variables for the scripted movement
    /// </summary>
    [System.Serializable]
    public class Distances
    {
        [Tooltip("Local distance, negatives go in reverse.")]
        public float Distance = 0;
        [Range(0, 1)]
        [Tooltip("Duration is normalized to the action length.")]
        public float Duration = 0;
        public AnimationCurve Curve = default;

        public Distances(float d, float duration, AnimationCurve curve = null)
        {
            Duration = duration;
            Distance = d;
            Curve = curve;
        }
    }
    /// <summary>
    /// early exit variables
    /// </summary>
    [System.Serializable]
    public class EarlyExitAction
    {
        [Range(0, 1)]
        [Tooltip("Normalized can exit time.")]
        public float CanExitTime = 1;
        public EarlyExitAction(float exitnormalziedtime)
        {
            CanExitTime = exitnormalziedtime;
        }
    }
    /// <summary>
    /// early exit state data
    /// </summary>
    [System.Serializable]
    public class EarlyExitState
    {
        public FreeFormState State = default;
        [Range(0, 1)]
        [Tooltip("Normalized can exit time.")]
        public float CanExitTime = 1;

    }
    /// <summary>
    /// base class for early exit options
    /// </summary>
    [System.Serializable]
    public class EarlyExitOptions
    {
        public List<EarlyExitState> EarlyExitStates = new List<EarlyExitState>();
        public EarlyExitAction EarlyExits = new EarlyExitAction(.1f);
    }
    /// <summary>
    /// animator vars for the action
    /// </summary>
    [System.Serializable]
    public class AnimatorVars
    {
        public bool ApplyRootMotion = false;
        public string AnimatorStateName = string.Empty;
        [Range(0f, 1f)]
        public float CrossFadeDuration = .02f;
        [Tooltip("Animator layer")]
        public int Layer = 0;
        [Tooltip("Animator speed param")]
        public float AnimatorSpeed = 1;
        [Tooltip("Clip is provided for length. For FBX files where action isn't the first clip, this is required. If left blank, will use the state length.")]
        public AnimationClip Clip = default;


    }
    /// <summary>
    /// input buffer variables
    /// </summary>
    [System.Serializable]
    public class InputBufferVars
    {

        [Tooltip("should we be able to change direction?")]
        public bool UseBufferedInputForwardAtStart = true;
        [Header("Transition Buffers")]
        [Range(0, 1f)]
        [Tooltip("Normalized time to start collecting inputs for buffer")]
        public float MinBufferInputNormalizedTime = .25f;
        [Range(0, 1f)]
        [Tooltip("Normalized time to stop collecting inputs for buffer")]
        public float MaxBufferInputNormalziedTime = .75f;

        public InputBufferVars(float min, float max)
        {
            MinBufferInputNormalizedTime = min;
            MaxBufferInputNormalziedTime = max;
        }
    }
    /// <summary>
    /// blocking variables
    /// </summary>
    [System.Serializable]
    public class Blocking
    {

        public bool EndActionOnBlocked = true;
        [Tooltip("Layers that will stop root motion movement. Good to prevent root motion from clipping through walls.")]
        public LayerMask BlockingLayers = default;
        public float DistanceCheck = 1;
        [Tooltip("WHen you want to check particular directions.")]
        public InitialBlockingConditions DirectionChecks = new InitialBlockingConditions();

    }

    [System.Serializable]
    public class InitialBlockingConditions
    {
        public bool EndActionOnBlocked = true;
        public List<DirectionType> DirectionChecks = new List<DirectionType>();
        public LayerMask BlockingLayers = default;
        public float DistanceCheck = 1;
    }
    /// <summary>
    /// list of blocking options
    /// </summary>
    [System.Serializable]
    public class BlockingOptions
    {
        public InitialBlockingConditions InitialBlocking = new InitialBlockingConditions();
       // public List<InitialBlockingConditions> Blocking = new List<InitialBlockingConditions>();//eventually remove, not really used at the moment

        
    }
    /// <summary>
    /// extend options for actions
    /// </summary>
    [System.Serializable]
    public class ExtendOptions
    {
        [Tooltip("Should successful inputs keep us in this state? e.g. holding an input to keep blocking.")]
        public bool UseExtend = false;
        [Tooltip("If we are using extend and we fail, should we immediately exit?")]
        public bool ShouldExitOnFail = true;
    }

    [System.Serializable]
    public class HitGiversOptions
    {
        public List<ActivateHitBox> Boxes = new List<ActivateHitBox>();
    }
    [System.Serializable]
    public class HitTakerOptions
    {
        public List<ActivateHitBox> Boxes = new List<ActivateHitBox>();
    }
    [System.Serializable]
    public class ActivateHitBox//to do, add group options
    {
        [Tooltip("Name of the hit box")]
        public string HitBoxName = string.Empty;
        [Range(0, 1f)]
        [Tooltip("Normalized time to enable hitbox")]
        public float EnableNormalizedTime = .1f;
        [Range(0, 1f)]
        [Tooltip("Normalized time to disable hitbox")]
        public float DisableNormalizedTime = .9f;

        [HideInInspector]
        public float EnableRealTime = 0;
        [HideInInspector]
        public float DisableRealTime = 0;
        [HideInInspector]
        public bool IsEnabled = false;
    }

    [System.Serializable]
    public class TimingOptions
    {
        [Tooltip("Any additional time we want to add? The action length default is the length of the animation, so animation + this additional duration = total.")]
        public float AdditionalDuration = 0;
        [Tooltip("Normalized time we want to exit this action to proceed to the next action. Typically it's 1, but who knows.")]
        [Range(.1f, 1f)]
        public float MinActionDuration = 1;

    }

    [System.Serializable]
    public class HitBoxOptions
    {
        public HitGiversOptions GiverOptions = new HitGiversOptions();
        public HitTakerOptions TakerOptions = new HitTakerOptions();
    }

    [System.Serializable]
    public class CustomCodeAssets
    {
        public List<CustomActionCodeSO> CustomCodes = new List<CustomActionCodeSO>();
    }

    [System.Serializable]
    public class ActionVars
    {
        public AnimatorVars AnimatorVars = new AnimatorVars();
    }


    
    /// <summary>
    /// action variables for the action state
    /// </summary>
    [System.Serializable]
    public class ActionCCVars : ActionVars
    {

        public BlockingOptions BlockingOptions = new BlockingOptions();
        public MovementMultipliers Multipliers = new MovementMultipliers();
        public TargetingOptions TargetOptions;
        public Distances TravelX;
        public Distances TravelY;
        public Distances TravelZ;

        public InputBufferVars BufferVars = new InputBufferVars(.25f, .75f);//not common to actions, this is it...

        public ExtendOptions ExtendOptions = new ExtendOptions();
        public EarlyExitOptions EarlyExitOptions = new EarlyExitOptions();
        [Tooltip("Used to manually toggle hitboxes without animation events")]
        public HitBoxOptions HitBoxOptions = new HitBoxOptions();
        public TimingOptions TimingOptions = new TimingOptions();
        public CustomCodeAssets CustomCodeAssets = new CustomCodeAssets();
       
    }

   

    #endregion
}