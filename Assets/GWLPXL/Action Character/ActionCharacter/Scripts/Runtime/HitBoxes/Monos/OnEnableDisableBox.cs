using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{
    /// <summary>
    /// on enable, assign hitbox. On disable, remove hitbox. Doesn't work due to order of operations if set as default loadout...
    /// </summary>
    public class OnEnableDisableBox : HitBoxDynamic
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            AddHitBox();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            RemoveHitBox();
        }
    }
}
