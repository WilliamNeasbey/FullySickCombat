using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{
    public class FauxHitParticles : HitTakeDamage
    {
        //need particle key for particle pool



        protected override void HurtCollision(HitContextCollision contextCollision)
        {
            base.HurtCollision(contextCollision);
        }


        protected override void HurtTrigger(HitContextTrigger contextCollision)
        {
            base.HurtTrigger(contextCollision);
        }
    }
}
