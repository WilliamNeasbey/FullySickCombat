using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{

    /// <summary>
    /// platform speeds must stay reasonable for built in controller to handle it.
    /// example how to do moving platforms and have the cc ride it
    /// </summary>
    /// 
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class Platform : MonoBehaviour
    {
        public MoveLerpVarsRB Vars;
        protected Rigidbody rb;
        protected PlatformMovement mover;

        protected virtual void Start()
        {
            GetComponent<Collider>().isTrigger = true;
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().isKinematic = true;
            mover = new PlatformMovement(GetComponent<Rigidbody>(), Vars, true);
            mover.AddTicker();
        }

        protected virtual void OnDestroy()
        {
            mover.RemoveTicker();
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            ActionCharacter instance = other.GetComponent<ActionCharacter>();
            if (instance != null)
            {
                mover.AddRider(instance);
                
            }
    
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            ActionCharacter instance = other.GetComponent<ActionCharacter>();
            if (instance != null)
            {
                mover.RemoveRider(instance);
            }
        }
    }
}