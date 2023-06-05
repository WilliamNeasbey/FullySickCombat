
using UnityEngine;

namespace GWLPXL.ActionCharacter
{
    public class PlayerCharacterCCLoco : LocomotionBase
    {




        protected InputAxis axisx = new InputAxis("Mouse X");
        protected InputAxis axisz = new InputAxis("Mouse Y");
        protected PlayerCharacterCC player;
        public PlayerCharacterCCLoco(PlayerCharacterCC instance, Movement movement) : base(instance, movement)
        {
            this.player = instance;
            this.movement = movement;
        }


        protected override void AssignInputs()
        {
            //test for now
            secondaryz = player.InputWrapper.GetAxis(axisz);
            secondaryx = player.InputWrapper.GetAxis(axisx);

            inputx = player.InputWrapper.GetXAxis();
            inputz = player.InputWrapper.GetZAxis();
            base.AssignInputs();
            
        }

       


    }
}



