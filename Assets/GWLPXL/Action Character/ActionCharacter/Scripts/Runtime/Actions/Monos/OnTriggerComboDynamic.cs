using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{


    /// <summary>
    /// performs the listed Combo action type to the character's loadout that hits the trigger
    /// </summary>
    /// 
    [RequireComponent(typeof(ActorHitBoxes))]
    public class OnTriggerComboDynamic : Environment
    {
        [Tooltip("New Combos")]
        public List<Combos> Combos = new List<Combos>();
        [Tooltip("Which combos to replace (if any)?")]
        public List<string> ToReplace = new List<string>();
        [Tooltip("What type of action?")]
        public ComboDynamicType Type = ComboDynamicType.Add;
        [Tooltip("Should only the player be affected? Or any action character?")]
        public bool OnlyPlayer = false;

        protected override void OnEnable()
        {
            base.OnEnable();
            boxes.HitBoxes.HitGivers.HitBoxesBase.PhysicsCallbacks.OnTriggerEnter += TriggerCombo;

        }

        protected override void OnDisable()
        {
            base.OnDisable();
            boxes.HitBoxes.HitGivers.HitBoxesBase.PhysicsCallbacks.OnTriggerEnter -= TriggerCombo;

        }

        public virtual void TriggerCombo(TriggerEventArgs args)
        {
            Debug.Log("Triggered");
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
                        ActionManager.AddCombos(new ComboArgs(character, Combos), Type, ToReplace);
                    }
                }
                else
                {
                    ActionManager.AddCombos(new ComboArgs(character, Combos), Type, ToReplace);
                }

            }
        }
       
 
    }
}
