using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{
    public class PlayerCharacterRBLoco : LocomotionBase
    {

        protected PlayerCharacterRB player;
        public PlayerCharacterRBLoco(PlayerCharacterRB instance, Movement movement) : base(instance, movement)
        {
            this.player = instance;
            this.movement = movement;
        }


        protected override void AssignInputs()
        {

            inputx = player.InputWrapper.GetXAxis();
            inputz = player.InputWrapper.GetZAxis();
            base.AssignInputs();

        }

        public override void Tick()
        {
            base.Tick();
            
        }
       


    }
    /// <summary>
    /// experimental, dont use
    /// </summary>
    public class PlayerCharacterRB : PlayerCharacter
    {
        public Rigidbody RB { get => rb; }
        protected CharacterController cc;
        protected Rigidbody rb;
        [SerializeField]
        protected Collider movecoll;
        protected bool ccdetectcollisions = true;
        protected PlayerCharacterRBLoco locostate;

        public override void InitializeLocomotion()
        {

            if (locostate != null) locostate.RemoveTicker();
            locostate = new PlayerCharacterRBLoco(this, MovementRuntime.Movement);
            locostate.AddTicker();
        }

        public override void RemoveLocotion()
        {

            if (locostate != null) locostate.RemoveTicker();
        }

        //public override float GetHeight()
        //{
        //    return cc.height;
        //}

        //public override float GetRadius()
        //{

        //    return cc.radius;
        //}

        //public override Vector3 GetCenter()
        //{

        //    return cc.center;
        //}


        public override void ResetCharacter()
        {

            base.ResetCharacter();
            rb = GetComponent<Rigidbody>();
            cc = GetComponent<CharacterController>();
            rb.detectCollisions = ccdetectcollisions;
            cc.detectCollisions = ccdetectcollisions;
         //   SetSlopeLimit(cc.slopeLimit);
          //  SetStepHeight(cc.stepOffset);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(outsideVel * 500f, transform.position);
            Gizmos.DrawSphere(outsideVel * 500f, 1f);
        }
        public override void Move()
        {
            Debug.Log(outsideVel);
            if (hasOutsideVel && !actionCheck)
            {
                rb.MovePosition(transform.position + outsideVel);
               // cc.Move(outsideVel);
                outsideVel = new Vector3(0, 0, 0);
                hasOutsideVel = false;
            }
            else if (hasOutsideVel)
            {
                rb.MovePosition(transform.position + outsideVel);
               // rb.MovePosition( outsideVel);
                //rb.AddForce(outsideVel, ForceMode.VelocityChange);
               //rb.velocity += outsideVel;
                outsideVel = new Vector3(0, 0, 0);
                hasOutsideVel = false;
            }
           
        }

        public override bool TryStartActionSequence(string actionName)
        {
            bool success = base.TryStartActionSequence(actionName);
            if (success)
            {
                movecoll.enabled = false;
                rb.isKinematic = true;

                rb.velocity = new Vector3(0, 0, 0);
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                rb.interpolation = RigidbodyInterpolation.Interpolate;
                rb.useGravity = false;
                rb.isKinematic = true;
                rb.detectCollisions = false;
                rb.isKinematic = true;

                cc.enabled = true;
                cc.Move(new Vector3(0, 0, 0));
            }
            return success;












        }



        public override void ActionSequenceEnded(string actionName)
        {
          //  rb.velocity = cc.velocity;
            rb.isKinematic = false;
           // rb.velocity = cc.velocity;
            //rb.useGravity = true;
           // rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.interpolation = RigidbodyInterpolation.None;
            rb.detectCollisions = true;
            cc.enabled = false;
            movecoll.enabled = true;


            base.ActionSequenceEnded(actionName);

        }
    }
}
