

using System;
using System.Collections.Generic;
using UnityEngine;
namespace GWLPXL.ActionCharacter
{
    [System.Serializable]
    public class MoveLerpVarsRB
    {
        public Vector3 Direction;
        public float Distance;
        public float Duration;
        public float WaitDuration;
        public AnimationCurve MoveCurve;

        public MoveLerpVarsRB(Vector3 dir, float distance, float duration, float waitduration, AnimationCurve curve)
        {
            Direction = dir;
            Distance = distance;
            Duration = duration;
            WaitDuration = waitduration;
            MoveCurve = curve;
        }
    }
    public class PlatformMovement : ITick
    {

        protected List<ActionCharacter> charControllers;

        public Action OnDestinationReached;
        public Action OnNewDestinationStarted;
        public MoveLerpVarsRB Vars;
        protected Rigidbody rb;
        protected float timer = 0;
        protected float waittimer = 0;
        protected float directionMultipler = 1;

        protected Vector3 start;
        protected Vector3 end;
        protected bool pingpong;
        protected bool wait;
        protected Vector3 previouspos;

        public virtual void AddRider(ActionCharacter controller)
        {
            if (charControllers.Contains(controller) == false)
            {
                charControllers.Add(controller);
                controller.SetRidingPlatform(true);
                
            }
        }

        public virtual void RemoveRider(ActionCharacter controller)
        {
            if (charControllers.Contains(controller) == true)
            {
                charControllers.Remove(controller);
                controller.SetRidingPlatform(false);
            }

        }
        public PlatformMovement(Rigidbody rigidbody, MoveLerpVarsRB vars, bool pingPong = false)
        {
            charControllers = new List<ActionCharacter>();
            this.rb = rigidbody;
            this.Vars = vars;
            this.pingpong = pingPong;
            start = rigidbody.position;
            end = start + vars.Direction * directionMultipler * Vars.Distance;
            previouspos = start;


        }
        public virtual void AddTicker()
        {
            TickManager.AddTicker(this as ITick);
        }

        public virtual float GetTickDuration()
        {
            return Time.deltaTime;
        }


        public virtual void RemoveTicker()
        {

            TickManager.RemoveTicker(this as ITick);
        }

        public virtual void Tick()
        {
            
            if (rb == null)
            {
                RemoveTicker();
                return;
            }
           
            timer += GetTickDuration();
            if (timer >= Vars.Duration)
            {
                if (waittimer == 0)
                {
                    OnDestinationReached?.Invoke();
                }
                waittimer += GetTickDuration();
                wait = true;
                if (waittimer >= Vars.WaitDuration)
                {
                    directionMultipler *= -1;
                    timer = 0;
                    waittimer = 0;
                    wait = false;
                    start = rb.position;
                    end = start + Vars.Direction * directionMultipler * Vars.Distance;
                    OnNewDestinationStarted?.Invoke();
                    if (pingpong == false)
                    {
                        RemoveTicker();
                    }

                }

            }
            float curvePosition = Vars.MoveCurve.Evaluate(timer / Vars.Duration);
            Vector3 lerp = Vector3.Lerp(start, end, curvePosition);
            Vector3 deltaPosition = lerp - previouspos;
            for (int i = 0; i < charControllers.Count; i++)
            {
                charControllers[i].AddStep(deltaPosition);
            }

            rb.transform.position = lerp;
            previouspos = lerp;


        }

        

    }


}