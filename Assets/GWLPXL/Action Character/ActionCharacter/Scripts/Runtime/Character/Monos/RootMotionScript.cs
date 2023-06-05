using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{


    public interface IRootMotion
    {
        void SetControllerObject(Transform transform);
        void SetRootMotionActive(bool isActive);
        bool GetRootMotionActive();
        Vector3 GetRootMotionDirection();
        Vector3 GetTargetPosition();
    }

    /// <summary>
    /// base class for root motion control during actions
    /// </summary>
    public class RootMotionScript : MonoBehaviour, IRootMotion
    {
        public event System.Action OnRootEnabled;
        public event System.Action OnRootDisabled;

        [SerializeField]
        protected bool apply;
        protected Transform controller;
        protected ActionCharacter actionCharacter;
        protected bool hasController;
        protected Animator animator;
        protected Vector3 lastpos;
        protected Vector3 rootMotionDir;
  
        protected virtual void Awake()
        {
            animator = GetComponent<Animator>();
        }


     
        protected virtual void OnAnimatorMove()
        {
            if (hasController == false) return;

            if (animator && apply)
            {
                rootMotionDir = (animator.rootPosition - animator.deltaPosition);
                actionCharacter.AddStep((animator.rootPosition + animator.deltaPosition) - actionCharacter.transform.position);


            }
            else if (apply == false)
            {
                rootMotionDir.x = 0;
                rootMotionDir.y = 0;
                rootMotionDir.z = 0;

                Vector3 step = (animator.rootPosition + animator.deltaPosition) - actionCharacter.transform.position;
                step = Vector3.ProjectOnPlane(step, Vector3.up);
                actionCharacter.AddStep(step);
            }



        }


       
      


        public virtual void SetRootMotionActive(bool isActive)
        {
            apply = isActive;
            if (apply)
            {
                OnRootEnabled?.Invoke();
            }
            else
            {
                OnRootDisabled?.Invoke();
            }
        }

        public Vector3 GetRootMotionDirection()
        {

            return (rootMotionDir);
        }

        public Vector3 GetTargetPosition()
        {
            return animator.targetPosition;
        }

        public void SetControllerObject(Transform transform)
        {
            controller = transform;
            actionCharacter = controller.GetComponent<ActionCharacter>();
            hasController = controller != null;
        }

        public bool GetRootMotionActive()
        {
            return apply;
        }
    }
}