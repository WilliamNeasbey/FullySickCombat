using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{
    /// <summary>
    /// switches loadout when action character enters trigger
    /// </summary>
       [RequireComponent(typeof(ActorHitBoxes))]
    public class OnTriggerLoadoutSwitch : Environment
    {
        [Tooltip("New Loudout to equip")]
        public CharacterActionLoadoutSO NewLoadout;
        [Tooltip("Should any action character be affected? Or just the player controlled?")]
        public bool OnlyPlayer = false;
        [Tooltip("Replace")]
        public ComboDynamicType Type = ComboDynamicType.ReplaceAll;

        protected override void OnEnable()
        {
            base.OnEnable();
            boxes.HitBoxes.HitGivers.HitBoxesBase.PhysicsCallbacks.OnTriggerEnter += TriggerEnter;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            boxes.HitBoxes.HitGivers.HitBoxesBase.PhysicsCallbacks.OnTriggerEnter -= TriggerEnter;

        }
        public virtual void TriggerEnter(TriggerEventArgs args)
        {
            GameObject owner = HitTakerManager.GetOwner(args.O);
            if (owner == null)
            {
                return;
            }
            ActionCharacter character = owner.GetComponent<ActionCharacter>();
            if (character != null)
            {

                if (OnlyPlayer)
                {
                    if (character.IsPlayedControlled)
                    {
                        ActionManager.AddActionSet(character, NewLoadout, Type);
                    }
                }
                else
                {
                    ActionManager.AddActionSet(character, NewLoadout, Type);
                }

            }
        }
        

    }
}
