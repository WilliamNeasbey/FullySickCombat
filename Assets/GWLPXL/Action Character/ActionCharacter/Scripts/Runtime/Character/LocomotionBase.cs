
using UnityEngine;

namespace GWLPXL.ActionCharacter
{
    public class LocomotionBase : ITick
    {
        public System.Action OnLanded;
        public System.Action OnAirborne;

        protected ActionCharacter instance;
        protected Transform transform;
        protected float falltimer;

        protected bool cachegrounded;
        protected bool hasfallcurve;

        protected Vector3 accumulatedStep;
        protected Vector3 zero = new Vector3(0, 0, 0);
        protected bool moved;
        protected float dt;
        protected float bufferedrotatetimer = 0;
        protected float bufferedmovetimer = 0;
        protected Vector3 floor;
        protected Vector3 head;
        protected Vector3 headoffset;
        protected float radius = 1;
        protected float height = 1;
        protected Vector3 center;
        protected float buffertimer = 0;
        protected float coyoteTimer = 0;
        protected float inputx = 0;
        protected float inputz = 0;
        protected float secondaryx = 0;
        protected float secondaryz = 0;
        protected Movement movement;

    
        public LocomotionBase(ActionCharacter controller, Movement movement)
        {
            this.movement = movement;
            this.instance = controller;
            this.transform = controller.Transform;
            this.radius = controller.Radius;
            this.height = controller.Height;
            this.center = controller.Center;
            this.headoffset = new Vector3(0, this.height, 0);
        }

        public virtual void AddTicker()
        {
            TickManager.AddTicker(this);
        }

        public float GetTickDuration()
        {
            return Time.deltaTime;
        }

        public virtual void RemoveTicker()
        {
            TickManager.RemoveTicker(this);
        }

        protected virtual void AssignInputs()
        {


            movement.Standard.Locomotion.Locomotion.Movement.X = inputx;
            movement.Standard.Locomotion.Locomotion.Movement.Z = inputz;

            movement.Standard.Locomotion.Rotate.Rotation.X = inputx;
            movement.Standard.Locomotion.Rotate.Rotation.Z = inputz;


        }

        float fpscounter = 0;

        public virtual void Tick()
        {

            AssignInputs();

            dt = GetTickDuration();

            accumulatedStep += MovingBehavior(dt);
            accumulatedStep = Project(accumulatedStep);
            accumulatedStep += FallingBehavior(dt);

            Move(accumulatedStep);
            Rotate(dt);
            ResetStep();

            fpscounter += dt;
            if (fpscounter >= (float)(1/instance.FPS))
            {
                instance.Rotate();
                instance.Move();
                instance.SetAnimatorValues();
               
           
                fpscounter = 0;
          
            }

            if (instance.ActionCheckEnabled)
            {
                instance.CheckInputs();
            }




            float speed = instance.GetSpeed();
         
        //    instance.AnimatorController.SetBoolParam(instance.Config.Defaults.IsGrounded, instance.GetGrounded());


        }

        protected virtual void ResetStep()
        {
            accumulatedStep = new Vector3(0, 0, 0);

        }
        protected virtual void Move(Vector3 accumulatedStep)
        {
            instance.AddStep(accumulatedStep);

        }

     

        protected virtual Vector3 Project(Vector3 accumulatedStep)
        {
            FallingVars vars = movement.Standard.Locomotion.Fall.Falling;
            InputMoveVars invars = movement.Standard.Locomotion.Locomotion.Movement;

            FreeFormState state = instance.GetCharacterStateAC();
            if (state == FreeFormState.Ground)// || state == FreeFormState.Coyote)
            {
               // RaycastHit hit = Detection.CapsuleCast(instance.Transform.position, instance.Transform.position + Vector3.up * instance.GetHeight() * .9f, instance.Radius, Vector3.down, instance.GetHeight(), vars.GroundLayer);
                RaycastHit hit = Detection.SimpleRaycastHit(instance.transform.position + transform.up * instance.Height * .75f, -transform.up, instance.Height, vars.GroundLayer);
                if (hit.collider != null)
                {
                    bool valid = Detection.HasSight(transform, transform.forward, hit.point, invars.SlopeLimit);
                    if (valid)
                    {
                        accumulatedStep = Vector3.ProjectOnPlane(accumulatedStep, hit.normal);
                        invars.GroundPlane = hit.normal;
                    }

                  
                }

                //snap
                hit = Detection.SimpleRaycastHit(transform.position + accumulatedStep, -transform.up, instance.Height * instance.GetStepHeight(), vars.GroundLayer);
                if (hit.collider != null)
                {

                    if (instance.IsRidingPlatform == false)
                    {
                        Vector3 dst = hit.point -  (transform.position + accumulatedStep);
                        accumulatedStep.y += dst.y;// * dt;
                    }
          
                }
            }
           
            return accumulatedStep;
        }


        protected virtual Vector3 FallingBehavior(float dt)
        {
            FallingVars vars = movement.Standard.Locomotion.Fall.Falling;
            floor = transform.position;
            head = transform.position + center + headoffset;

            bool grounded = instance.GetGrounded();// GroundCheck(vars.GroundLayer);//current state

           
            if (grounded != vars.IsGrounded)
            {
                buffertimer += dt;

                if (buffertimer >= vars.BufferTimer)
                {
                    vars.IsGrounded = grounded;
                }
            }
            else
            {
                buffertimer = 0;
            }
           
           

            if (vars.IsGrounded != cachegrounded)
            {
                cachegrounded = vars.IsGrounded;
               
                if (cachegrounded)
                {
                    coyoteTimer = vars.CoyoteDuration;
                    vars.CoyoteAllowed = true;
                    OnLanded?.Invoke();
                }
                else
                {
                    OnAirborne?.Invoke();
                }
            }

            if (vars.IsGrounded == false)
            {
                coyoteTimer -= dt;
                if (coyoteTimer <= 0)
                {
                    vars.CoyoteAllowed = false;
                }
            }

            falltimer += dt;


            if (vars.Multiplier == 0 || vars.IsGrounded)
            {
                falltimer = dt;
            }
            

            float percent = falltimer / vars.TimeToMaxFallSpeed;
            if (hasfallcurve)
            {
                percent = vars.FallingCurve.Evaluate(percent);
            }
            float newY = percent * vars.FallingSpeed;
            vars.CurrentFallingSpeed = newY * vars.Multiplier * dt ;//falling step
            moved = true;
            return vars.GravityDirection * vars.CurrentFallingSpeed ;
           

        }

       
        protected virtual void Rotate(float dt)
        {
            InputRotateVars vars = movement.Standard.Locomotion.Rotate.Rotation;
            Vector3 newMove = new Vector3(vars.X, 0, vars.Z);
            if (instance.HasLockOnTarget)
            {
                newMove = instance.LockOnTarget.position - instance.Transform.position;
            }
            else if (instance.HasLockOnPosition)
            {
                newMove = instance.LockOnPosition - instance.Transform.position;
            }
            else
            {
                switch (vars.Reference)
                {
                    case InputReference.Camera:
                        newMove = MovementHelpers.TranslateToCamera(newMove);
                        break;
                    case InputReference.Local:
                        newMove = MovementHelpers.TranslateToLocal(transform, newMove);
                        break;
                }
            }
            newMove = Vector3.ProjectOnPlane(newMove, Vector3.up);
            

            Vector3 translatedInput = newMove;

           
            if (vars.Z != 0 || vars.X != 0 || instance.HasLockOnPosition || instance.HasLockOnTarget)
            {
                Quaternion applied = MovementHelpers.GetLookRotation(transform, translatedInput, vars.Speed * vars.Multiplier, dt, vars.Smooth);
                vars.FaceDirection = applied;
            }

            if (Mathf.Approximately(vars.X, 0) && Mathf.Approximately(vars.Z, 0))
            {
                bufferedrotatetimer += dt;
                if (bufferedrotatetimer >= vars.BufferedKeepTime)
                {
                    vars.BufferedX = 0;
                    vars.BufferedZ = 0;
                    bufferedrotatetimer = 0;
                }
            }
            else
            {
                vars.BufferedX = vars.X;
                vars.BufferedZ = vars.Z;
                vars.TranslatedRotate = translatedInput;
            }

          
          

        }

        protected virtual Vector3 MovingBehavior(float dt)
        {
            FreeFormState state = instance.GetCharacterStateAC();
            InputMoveVars vars = movement.Standard.Locomotion.Locomotion.Movement;
            float multi = vars.Multiplier;
            switch (state)
            {
                case FreeFormState.Airborne:
                    multi = vars.AirborneMulti;
                    break;
            }
            Vector3 newDir = new Vector3(vars.X, 0, vars.Z).normalized;
           
            
            Vector3 translated = newDir;

            vars.GlobalInput = new Vector3(vars.X, 0, vars.Z);
            vars.LocalInput = new Vector3(vars.X, 0, vars.Z);


            switch (vars.Reference)
            {
                case InputReference.Camera:
                    vars.CameraInput = MovementHelpers.TranslateToCamera(newDir);
                    translated = vars.CameraInput;
                    break;
                case InputReference.Local:
                    vars.LocalInput = MovementHelpers.TranslateToLocal(transform, newDir);
                    translated = vars.LocalInput;
                    break;
            }

            vars.TranslatedMoveDirection = translated * vars.Speed * multi * dt;

           




            if (Mathf.Approximately(vars.X, 0) && Mathf.Approximately(vars.Z, 0))
            {
                bufferedmovetimer += dt;
                if (bufferedmovetimer >= vars.BufferedKeepTime)
                {
                    vars.BufferedX = 0;
                    vars.BufferedZ = 0;
                    vars.BufferedTranslatedMoveDirection = zero;
                    vars.BufferedMoveDir = zero;
                    bufferedmovetimer = 0;

                }
            }
            else
            {
                vars.BufferedX = vars.X;
                vars.BufferedZ = vars.Z;
                vars.BufferedTranslatedMoveDirection = vars.TranslatedMoveDirection;
                vars.BufferedMoveDir = translated;
            }



   
            moved = true;
            return vars.TranslatedMoveDirection;
        }
    }
}



