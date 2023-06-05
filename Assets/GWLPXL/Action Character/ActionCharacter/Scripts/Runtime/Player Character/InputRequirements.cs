


using System.Collections.Generic;


namespace GWLPXL.ActionCharacter
{
    public enum FreeFormState
    {
        Ground = 0,
        Airborne = 1,
        Coyote = 100,
        Dead = 101
    }

    public enum ButtonType
    {
        None = 0,
        Click = 10,
        Held = 20,
        Released = 30
    }

    public enum AxisMovementType
    {
        None = 0,
        Neutral = 10,
        Forward = 20,
        Backward = 30,
        Right = 40,
        Left = 50,
        AnythingButNeutral = 100


    }
    /// <summary>
    /// axis requirement for action
    /// </summary>
    [System.Serializable]
    public class InputAxisFreeForm
    {
        public AxisMovementType RequirementType = AxisMovementType.Neutral;
        public AxisMovementType Value { get; set; }
        public InputAxisFreeForm(AxisMovementType type)
        {
            RequirementType = type;
        }
    }
    /// <summary>
    /// input button required for action
    /// </summary>
    [System.Serializable]
    public class InputButton
    {
        public string ButtonName = string.Empty;
        public ButtonType Type = ButtonType.Click;
        public bool Value { get; set; }
        public InputButton(string name, ButtonType type)
        {
            ButtonName = name;
            Type = type;
        }
    }

    /// <summary>
    /// input axis required for action
    /// </summary>
    [System.Serializable]
    public class InputAxis
    {
        public string AxisName = string.Empty;
        public float Value { get; set; }
        public InputAxis(string axis)
        {
            AxisName = axis;
        }
    }
    /// <summary>
    /// input requirements for actions, buttons and axis
    /// </summary>
    [System.Serializable]
    public class InputRequirements
    {
        public List<InputButton> InputButtons = new List<InputButton>();
        public InputAxisFreeForm MovementAxisRequirement = new InputAxisFreeForm(AxisMovementType.None);
        protected int priority = 0;
        /// <summary>
        /// higher number, higher priority, to do - multi input axis e.g. back then forward.
        /// </summary>
        /// <returns></returns>
        public virtual int GetPriority()
        {

            priority = InputButtons.Count;
            if (MovementAxisRequirement.RequirementType != AxisMovementType.None)
            {
                priority += 1;
            }
            return priority;
        }
    }
}

