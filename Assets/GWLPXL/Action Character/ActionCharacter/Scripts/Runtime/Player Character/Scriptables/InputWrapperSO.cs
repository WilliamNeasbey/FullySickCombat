using UnityEngine;
using System.Collections.Generic;
namespace GWLPXL.ActionCharacter
{

    /// <summary>
    /// base abstract class for input wrapper plain, extend for different inputs
    /// </summary>
    public abstract class InputWrapper
    {
        protected Dictionary<string, float> heldTimer = new Dictionary<string, float>();

        protected void ResetHeld(string buttonName)
        {
            if (heldTimer.ContainsKey(buttonName))
            {
                heldTimer[buttonName] = 0;
            }
            else
            {
                heldTimer.Add(buttonName, 0);
            }
        }
        protected float HeldDuration(string buttonName, float dt)
        {
            float duration = 0;
            if (heldTimer.ContainsKey(buttonName))
            {
                duration = heldTimer[buttonName];
            }

            duration += dt;
            heldTimer[buttonName] = duration;
            return duration;
        }
        public abstract float GetXAxis();
        public abstract float GetZAxis();
        public abstract bool GetButton(InputButton inputbutton);
        public abstract float GetAxis(InputAxis axis);
        public virtual bool GetMovementAxis(ActionCharacter instance, InputAxisFreeForm actionname)
        {
            AxisMovementType type = actionname.RequirementType;

            if (type == AxisMovementType.None) return true;//handles none case

            AxisMovementType dir = Detection.DetectInputMoveDirection(instance.Transform, instance.Transform.position + instance.GetBufferedMoveDirection());
            actionname.Value = dir;

            switch (type)
            {
                case AxisMovementType.AnythingButNeutral:
                    return actionname.Value != AxisMovementType.Neutral;

            }

            return type == dir;

        }


    }


    /// <summary>
    /// base abstract class for input wrapper scriptable
    /// </summary>
    public abstract class InputWrapperSO : ScriptableObject
    {
        public abstract InputWrapper GetWrapper();

        
        public virtual float GetAxis(InputAxis axis)
        {
            return GetWrapper().GetAxis(axis);
        }
        public virtual float GetXAxis()
        {
            return GetWrapper().GetXAxis();
        }
        public virtual float GetZAxis()
        {
            return GetWrapper().GetZAxis();
        }
        public virtual bool GetButton(InputButton inputbutton)
        {
            return GetWrapper().GetButton(inputbutton);
        }


        public virtual bool GetMoveAxis(ActionCharacter instance, InputAxisFreeForm axis)
        {
            return GetWrapper().GetMovementAxis(instance, axis);
        }
       

    }
}