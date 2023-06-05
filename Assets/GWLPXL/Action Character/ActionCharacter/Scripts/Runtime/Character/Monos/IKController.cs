using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{
    public interface IActionIK
    {
        public void SetControllerObject(Transform controller);
        public float GetWeight(AvatarIKGoal forGoal);
        public void SetIKPosition(AvatarIKGoal forGoal, Vector3 newPos);
        public void SetIKRotation(AvatarIKGoal forGoal, Quaternion newRot);
        public Vector3 GetIKPosition(AvatarIKGoal forGoal);
        public Quaternion GetIKRotation(AvatarIKGoal forGoal);
        public void SetIKPositionWeight(AvatarIKGoal forGoal, float weight);
        public void SetIKRotationWeight(AvatarIKGoal forGoal, float weight);

    }


    public class IKController : MonoBehaviour, IActionIK
    {
        protected Animator animator;
        protected ActionCharacter actionCharacter;
        protected Transform controller;
        [Range(-1f, 1f)]
        [SerializeField]
        protected float distanceToGround;
        [SerializeField]
        protected bool apply = false;
        [SerializeField]
        protected float footRadius = .1f;
        [SerializeField]
        protected string IKLeftFoot = "IKLeftFoot";
        [SerializeField]
        protected string IKRightFoot = "IKRightFoot";
        protected float zero = 0f;


        protected virtual void Awake()
        {
            animator = GetComponent<Animator>();
        }
        public virtual Vector3 GetIKPosition(AvatarIKGoal forGoal)
        {
            return animator.GetIKPosition(forGoal);
        }

        public virtual Quaternion GetIKRotation(AvatarIKGoal forGoal)
        {
            return animator.GetIKRotation(forGoal);
        }

        public virtual float GetWeight(AvatarIKGoal forGoal)
        {
            switch (forGoal)
            {
                case AvatarIKGoal.LeftFoot:
                    //
                    return animator.GetFloat(IKLeftFoot);

                case AvatarIKGoal.RightFoot:
                    //
                    return animator.GetFloat(IKRightFoot);

            }
            return zero;
        }


        public virtual void SetIKPosition(AvatarIKGoal forGoal, Vector3 newPos)
        {
            animator.SetIKPosition(forGoal, newPos);
        }

        public virtual void SetIKPositionWeight(AvatarIKGoal forGoal, float weight)
        {
            animator.SetIKPositionWeight(forGoal, weight);
        }

        public virtual void SetIKRotation(AvatarIKGoal forGoal, Quaternion newRot)
        {
            animator.SetIKRotation(forGoal, newRot);
        }

        public virtual void SetIKRotationWeight(AvatarIKGoal forGoal, float weight)
        {
            animator.SetIKRotationWeight(forGoal, weight);
        }

        protected virtual void OnAnimatorIK(int layerIndex)
        {

            if (animator)
            {

                if (apply == true)
                {

                    FreeFormState sate = actionCharacter.GetCharacterStateAC();
                    switch (sate)
                    {
                        case FreeFormState.Ground:

                            SetIKTarget(AvatarIKGoal.LeftFoot);
                            SetIKTarget(AvatarIKGoal.RightFoot);
                            SetIKPositionWeight(AvatarIKGoal.LeftFoot, GetWeight(AvatarIKGoal.LeftFoot));
                            SetIKRotationWeight(AvatarIKGoal.LeftFoot, GetWeight(AvatarIKGoal.RightFoot));
                            break;
                        case FreeFormState.Coyote:
                            SetIKTarget(AvatarIKGoal.LeftFoot);
                            SetIKTarget(AvatarIKGoal.RightFoot);
                            SetIKPositionWeight(AvatarIKGoal.LeftFoot, GetWeight(AvatarIKGoal.LeftFoot));
                            SetIKRotationWeight(AvatarIKGoal.LeftFoot, GetWeight(AvatarIKGoal.RightFoot));
                            break;
                    }



                
          
                }

            }
        }


       

        protected virtual void SetIKTarget(AvatarIKGoal goal)
        {
            RaycastHit hit;
            Vector3 start = GetIKPosition(goal);
            FallingVars vars = actionCharacter.MovementRuntime.Movement.Standard.Locomotion.Fall.Falling;
            InputMoveVars move = actionCharacter.MovementRuntime.Movement.Standard.Locomotion.Locomotion.Movement;

            hit = Detection.SimpleRaycastHit(start + -vars.GravityDirection, vars.GravityDirection, actionCharacter.GetCenter().y, vars.GroundLayer);
            if (hit.collider != null)
            {
                Vector3 footpos = Vector3.ProjectOnPlane(hit.point, move.GroundPlane);
                footpos.y += distanceToGround;
                SetIKPosition(goal, footpos);
            }
            else
            {
                Collider[] colls = Detection.SphereOverlapAll(start, footRadius, vars.GroundLayer);
                if (colls.Length > 0)
                {
                    int max = 10;
                    int run = 0;
                    while (colls.Length > 0)
                    {
                        colls = Detection.SphereOverlapAll(GetIKPosition(goal), footRadius, vars.GroundLayer);
                        Vector3 footpos = GetIKPosition(goal) + (-vars.GravityDirection * footRadius);
                        SetIKPosition(goal, footpos);

                        run++;
                        if (run > max)
                        {
                            break;
                        }
                    }

                }
            }
        }

     

        public void SetControllerObject(Transform controller)
        {
            this.controller = controller;
            actionCharacter = this.controller.GetComponent<ActionCharacter>();
        }
    }
}
