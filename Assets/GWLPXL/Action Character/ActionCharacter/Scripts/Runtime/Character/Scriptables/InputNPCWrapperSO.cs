using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{
    [CreateAssetMenu(fileName = "New NPC Input Wrapper", menuName = "GWLPXL/ActionCharacter/Input/NPC Input Wrapper", order = 310)]
    public class InputNPCWrapperSO : InputWrapperSO
    {
        protected InputNPCWrapper wrapper = new InputNPCWrapper();
        public override InputWrapper GetWrapper()
        {
            return wrapper;
        }


    }

    [System.Serializable]
    public class InputNPCWrapper : InputWrapper
    {
        protected INPCControl bb;
        protected bool hasbb = false;
        public virtual void SetNPC(INPCControl bb)
        {
            this.bb = bb;
            hasbb = this.bb != null;
        }
        public override float GetAxis(InputAxis axis)
        {

            return axis.Value;
        }

        public override bool GetButton(InputButton inputbutton)
        {
            return inputbutton.Value;
        }

        public override float GetXAxis()
        {
            if (hasbb) return bb.GetXAxisInput();
            return 0;
        }

        public override float GetZAxis()
        {
            if (hasbb) return bb.GetZAxisInputs();
            return 0;
        }

        public override bool GetMovementAxis(ActionCharacter instance, InputAxisFreeForm actionname)
        {

            return base.GetMovementAxis(instance, actionname);
        }
    }
    
}
