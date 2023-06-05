
using System.Collections;
using UnityEngine;
using UnityEngine.Events;


namespace GWLPXL.ActionCharacter
{

   


    public class HitGiverEventCaller
    {

        public event System.Action<CollisionEventArgs> OnCollisionEnter;
        public event System.Action<CollisionEventArgs> OnCollisionExit;
        public event System.Action<CollisionEventArgs> OnCollisionStay;

        public System.Action<TriggerEventArgs> OnTriggerEnter;
        public System.Action<TriggerEventArgs> OnTriggerExit;
        public System.Action<TriggerEventArgs> OnTriggerStay;


        public void RegisterEvents(PhysicsEvents events)
        {
            events.Callbacks.OnCollisionEnter += OnCollisionEnter;
            events.Callbacks.OnCollisionExit += OnCollisionExit;
            events.Callbacks.OnCollisionStay += OnCollisionStay;

            events.Callbacks.OnTriggerEnter += OnTriggerEnter;
            events.Callbacks.OnTriggerStay += OnTriggerStay;
            events.Callbacks.OnTriggerExit += OnTriggerExit;
        }

        public void UnRegisterEvents(PhysicsEvents events)
        {
            events.Callbacks.OnCollisionEnter -= OnCollisionEnter;
            events.Callbacks.OnCollisionExit -= OnCollisionExit;
            events.Callbacks.OnCollisionStay -= OnCollisionStay;

            events.Callbacks.OnTriggerEnter -= OnTriggerEnter;
            events.Callbacks.OnTriggerStay -= OnTriggerStay;
            events.Callbacks.OnTriggerExit -= OnTriggerExit;
        }
    }
}