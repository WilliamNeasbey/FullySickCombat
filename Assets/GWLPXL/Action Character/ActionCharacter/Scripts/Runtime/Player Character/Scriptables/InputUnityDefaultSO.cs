using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{

    /// <summary>
    /// SO wrapper for unity's working input
    /// </summary>

    [CreateAssetMenu(fileName = "New Unity Default Input Wrapper", menuName = "GWLPXL/ActionCharacter/Input/Unity Default Input Wrapper", order = 310)]
    public class InputUnityDefaultSO : InputWrapperSO
    {
        public InputWrapperUnityDefault Wrapper = new InputWrapperUnityDefault();

        public override InputWrapper GetWrapper()
        {
            return Wrapper;
        }
    }

   
    /// <summary>
    /// input wrapper for unity's working input system
    /// </summary>
    [System.Serializable]
    public class InputWrapperUnityDefault : InputWrapper
    {
        public InputAxis XAxis = new InputAxis("Horizontal");
        public InputAxis ZAxis = new InputAxis("Vertical");

        public override float GetAxis(InputAxis axis)
        {
            axis.Value = Input.GetAxis(axis.AxisName);
            return axis.Value;
        }
        public override float GetXAxis()
        {
            XAxis.Value = Input.GetAxis(XAxis.AxisName);
            return XAxis.Value;
        }

        public override float GetZAxis()
        {
            ZAxis.Value = Input.GetAxis(ZAxis.AxisName);
            return ZAxis.Value;
        }

      
        public override bool GetButton(InputButton inputbutton)
        {
            
            switch (inputbutton.Type)
            {
                case ButtonType.Click:
                    inputbutton.Value = Input.GetButtonDown(inputbutton.ButtonName);
                    return inputbutton.Value;
                case ButtonType.Held:
                    inputbutton.Value = Input.GetButton(inputbutton.ButtonName);
                    return inputbutton.Value;   
                case ButtonType.Released:
                    inputbutton.Value = Input.GetButtonUp(inputbutton.ButtonName);
                    return inputbutton.Value;
                default:
                    break;
            }

            return false;

        }

       

       
    }


    
}