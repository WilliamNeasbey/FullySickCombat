
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GWLPXL.ActionCharacter
{

   

    /// <summary>
    /// player action character that uses unity CC
    /// </summary>
    /// 
    [RequireComponent(typeof(CharacterController))]
    public class PlayerCharacterCC : PlayerCharacter
    {
        public CharacterController CC { get => cc; }
        protected CharacterController cc;
        [SerializeField]
        protected bool ccdetectcollisions = true;
        protected PlayerCharacterCCLoco locostate;
        [SerializeField]
        protected float groundCheckRadius = .25f;

       
        public override void InitializeLocomotion()
        {

            if (locostate != null) locostate.RemoveTicker();
            locostate = new PlayerCharacterCCLoco(this, MovementRuntime.Movement);
            locostate.AddTicker();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(transform.position + GetSteps(), groundCheckRadius);
        }
        public override bool GetGrounded()
        {
            if (CC.isGrounded == false)
            {
                return Physics.CheckSphere(transform.position + GetSteps(), groundCheckRadius, runtime.Movement.Standard.Locomotion.Fall.Falling.GroundLayer);
            }
            return CC.isGrounded;
        }
        public override void RemoveLocotion()
        {

            if (locostate != null) locostate.RemoveTicker();
            locostate = null;
        }

        public override float GetHeight()
        {
            return cc.height;
        }

        public override float GetRadius()
        {

            return cc.radius;
        }

        public override Vector3 GetCenter()
        {

            return cc.center;
        }

       
        public override void ResetCharacter()
        {

            base.ResetCharacter();
            cc = GetComponent<CharacterController>();
            cc.detectCollisions = ccdetectcollisions;
            SetSlopeLimit(cc.slopeLimit);
            SetStepHeight(cc.stepOffset);
        }



    
        public override void Move()
        {
           if (hasOutsideVel && canMove)
            {
                cc.Move(outsideVel);
                outsideVel = new Vector3(0, 0, 0);
                hasOutsideVel = false;
            }
        }




    }
}