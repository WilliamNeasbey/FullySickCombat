using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

namespace GWLPXL.ActionCharacter
{

    [System.Serializable]
    public class TriggerCallback : UnityEvent<Collider, Collider>
    {
    }
    [System.Serializable]
    public class CollisionCallback : UnityEvent<Collider, Collision>
    {

    }

    /// <summary>
    /// self collider, collision
    /// </summary>
    public class CollisionEventArgs : EventArgs
    {
        public Collider S;
        public Collision C;
        public CollisionEventArgs(Collider s, Collision c)
        {
            S = s;
            C = c;
        }
    }
    public class TriggerEventArgs : EventArgs
    {
        public Collider S;
        public Collider O;
        public TriggerEventArgs(Collider s, Collider o)
        {
            S = s;
            O = o;
        }
    }
    [System.Serializable]
    public class PhysicsCallbacks
    {
        public event System.Action<TriggerEventArgs> OnTriggerEnter;
        public event System.Action<TriggerEventArgs> OnTriggerExit;
        public event System.Action<TriggerEventArgs> OnTriggerStay;
        public event System.Action<CollisionEventArgs> OnCollisionEnter;
        public event System.Action<CollisionEventArgs> OnCollisionExit;
        public event System.Action<CollisionEventArgs> OnCollisionStay;

        public void FireTriggerEnter(object o, TriggerEventArgs arg)
        {
            OnTriggerEnter?.Invoke(arg);
        }
        public void FireTriggerExit(object o, TriggerEventArgs arg)
        {
            OnTriggerExit?.Invoke(arg);
        }
        public void FireTriggerStay(object o, TriggerEventArgs arg)
        {
            OnTriggerStay?.Invoke(arg);
        }
        public void FireCollisionEnter(object o, CollisionEventArgs arg)
        {
            OnCollisionEnter?.Invoke(arg);
        }
        public void FireCollisionStay(object o, CollisionEventArgs arg)
        {
            OnCollisionStay?.Invoke(arg);
        }
        public void FireCollisionExit(object o, CollisionEventArgs arg)
        {
            OnCollisionExit?.Invoke(arg);
        }

    }

    [System.Serializable]
    public class UnityPhysicsCallbacks
    {
        public void FireTriggerEnter(object o, TriggerEventArgs arg)
        {
            OnTriggerEnter?.Invoke(arg.S, arg.O);
        }
        public void FireTriggerExit(object o, TriggerEventArgs arg)
        {
            OnTriggerExit?.Invoke(arg.S, arg.O);
        }
        public void FireTriggerStay(object o, TriggerEventArgs arg)
        {
            OnTriggerStay?.Invoke(arg.S, arg.O);
        }
        public void FireCollisionEnter(object o, CollisionEventArgs arg)
        {
            OnCollisionEnter?.Invoke(arg.S, arg.C);
        }
        public void FireCollisionStay(object o, CollisionEventArgs arg)
        {
            OnCollisionStay?.Invoke(arg.S, arg.C);
        }
        public void FireCollisionExit(object o, CollisionEventArgs arg)
        {
            OnCollisionExit?.Invoke(arg.S, arg.C);
        }

        public TriggerCallback OnTriggerEnter;
        public TriggerCallback OnTriggerExit;
        public TriggerCallback OnTriggerStay;
        public CollisionCallback OnCollisionEnter;
        public CollisionCallback OnCollisionExit;
        public CollisionCallback OnCollisionStay;
    }

    /// <summary>
    /// mono that subs to unity physics events and passes them on. Required on each hit box to pass along their results.
    /// </summary>
    public class PhysicsEvents : MonoBehaviour
    {
        public UnityPhysicsCallbacks UnityEvents = new UnityPhysicsCallbacks();
        public PhysicsCallbacks Callbacks = new PhysicsCallbacks();
        public bool EnableDebug = false;
        protected Collider selfColl;

        protected virtual void Awake()
        {
            selfColl = GetComponent<Collider>();//self
        }
        protected virtual void DebugMessage(string message)
        {
            if (EnableDebug)
            {
                Debug.Log(message);
            }
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            TriggerEventArgs args = new TriggerEventArgs(selfColl, other);
            Callbacks.FireTriggerEnter(this, args);
            UnityEvents.FireTriggerEnter(this, args);
            DebugMessage(gameObject.name + " Trigger enter with " + other.gameObject);
        }

        protected virtual void OnTriggerStay(Collider other)
        {
            TriggerEventArgs args = new TriggerEventArgs(selfColl, other);
            Callbacks.FireTriggerStay(this, args);
            UnityEvents.FireTriggerStay(this, args);
            DebugMessage(gameObject.name + " Trigger stay with " + other.gameObject);
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            TriggerEventArgs args = new TriggerEventArgs(selfColl, other);
            Callbacks.FireTriggerExit(this, args);
            UnityEvents.FireTriggerExit(this, args);
            DebugMessage(gameObject.name + " Trigger exit with " + other.gameObject);
        }

        protected virtual void OnCollisionEnter(Collision collision)
        {
            Callbacks.FireCollisionEnter(this, new CollisionEventArgs(selfColl, collision));
            UnityEvents.FireCollisionEnter(this, new CollisionEventArgs(selfColl, collision));
            DebugMessage(gameObject.name + " Collision enter with " + collision.collider.gameObject);
        }

        protected virtual void OnCollisionStay(Collision collision)
        {
            Callbacks.FireCollisionStay(this, new CollisionEventArgs(selfColl, collision));
            UnityEvents.FireCollisionStay(this, new CollisionEventArgs(selfColl, collision));
            DebugMessage(gameObject.name + " Collision stay with " + collision.collider.gameObject);

        }

        protected virtual void OnCollisionExit(Collision collision)
        {
            Callbacks.FireCollisionExit(this, new CollisionEventArgs(selfColl, collision));
            UnityEvents.FireCollisionExit(this, new CollisionEventArgs(selfColl, collision));
            DebugMessage(gameObject.name + " Collision exit with " + collision.collider.gameObject);

        }
    }
}