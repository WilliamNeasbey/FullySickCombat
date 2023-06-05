using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{
    public class LockOnTrigger : MonoBehaviour
    {
        public Transform Target;

        protected virtual void OnTriggerEnter(Collider other)
        {
            ActionCharacter action = other.GetComponent<ActionCharacter>();
     
            if (action != null)
            {
                action.SetLockOnTarget(Target);
            }
        }


        protected virtual void OnTriggerExit(Collider other)
        {
            ActionCharacter action = other.GetComponent<ActionCharacter>();
            if (action != null)
            {
                action.SetLockOnTarget(null);
            }
        }
       
    }
}
