using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{
    /// <summary>
    /// base action so, override and implement to create your own custom action types
    /// </summary>
    public abstract class ActionSO : ScriptableObject
    {
        public event System.Action OnValuesChanged;
        protected virtual void OnValidate()
        {
            OnValuesChanged?.Invoke();
        }
        /// <summary>
        /// the state or ticker for the action, seqindex is the index since some actions might have multiple parts
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="seqindex"></param>
        /// <returns></returns>
        public abstract ActionTicker GetAction(ActionCharacter instance, int seqindex);
        /// <summary>
        /// the sequence and respective vars
        /// </summary>
        /// <returns></returns>
        public abstract List<ActionVars> GetSequenceVars();
        /// <summary>
        /// name used to identify and key the action, must be unique
        /// </summary>
        /// <returns></returns>
        public abstract string GetActionName();
        /// <summary>
        /// can the action be performed?
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public abstract bool HasRequirements(ActionCharacter instance);
        /// <summary>
        /// sets the name
        /// </summary>
        /// <param name="newName"></param>
        public abstract void SetActionName(string newName);
        /// <summary>
        /// returns how much this costs
        /// </summary>
        /// <returns></returns>
        public abstract int GetAirborneCost();
    }
    /// <summary>
    /// Scriptable Object Container for the included character actions. Extends ActionSO
    /// </summary>
    [CreateAssetMenu(fileName = "New Action", menuName = "GWLPXL/ActionCharacter/Action/Action", order = 200)]
    public class CharacterActionsSO : ActionSO
    {
        public CharacterActions Movement = new CharacterActions();
        [HideInInspector]
        public bool AutoName;

        public override ActionTicker GetAction(ActionCharacter instance, int seqindex)
        {
            return new ActionTickerCC(instance, Movement.Movements.ScriptedMovement.MovementBehavior.MovementSequence[seqindex]);
        }


        public override string GetActionName()
        {
            return Movement.Movements.ScriptedMovement.Name;
        }

        public override void SetActionName(string newName)
        {
            Movement.Movements.ScriptedMovement.Name = newName;
        }

        public override List<ActionVars> GetSequenceVars()
        {
            List<ActionVars> vars = Movement.Movements.ScriptedMovement.MovementBehavior.MovementSequence.ToList<ActionVars>();
            return vars;
        }

        public override bool HasRequirements(ActionCharacter instance)
        {
            //check custom requirements
            List<ActionCCVars> vars = Movement.Movements.ScriptedMovement.MovementBehavior.MovementSequence;
            for (int i = 0; i < vars.Count; i++)
            {
                ActionCCVars v = vars[i];
                for (int j = 0; j < v.CustomCodeAssets.CustomCodes.Count; j++)
                {
                    CustomActionCodeSO c = v.CustomCodeAssets.CustomCodes[j];
                    if (c.HasRequirements(instance) == false)
                    {
                        return false;
                    }
                }
            }

            //check state requirements
            FreeFormState current = instance.GetCharacterStateAC();
            int requiredStatesLength = Movement.Movements.ScriptedMovement.CharacterRequirements.RequiredStates.Count;
            if (Movement.Movements.ScriptedMovement.CharacterRequirements.RequiresTarget)
            {
                if (instance.HasLockOnTarget == false)
                {
                    //no target
                    return false;
                }
            }
            int cost = Movement.Movements.ScriptedMovement.AirborneCost;
            int max = instance.MovementRuntime.Movement.Standard.Locomotion.Fall.Falling.MaxAirborneActions;
            int airbornecurrent = instance.GetCurrentAirborneActions();
            if (cost + airbornecurrent > max)
            {
                return false;
            }

            if (requiredStatesLength == 0)
            {
                return true;//no requirements
            }
            for (int j = 0; j < requiredStatesLength; j++)
            {
                FreeFormState required = Movement.Movements.ScriptedMovement.CharacterRequirements.RequiredStates[j];
                if (current == required)
                {
                    return true;
                }
            }

            return false;
           
        }

        public override int GetAirborneCost()
        {
            return Movement.Movements.ScriptedMovement.AirborneCost;
        }
    }
    /// <summary>
    /// base character actions
    /// </summary>
    [System.Serializable]
    public class CharacterActions
    {
        public ExtraMovement Movements = new ExtraMovement();
    }

    /// <summary>
    /// base movement sequence
    /// </summary>
    [System.Serializable]
    public class ExtraMovement
    {
        public MoveSequence ScriptedMovement = new MoveSequence();
    }

    /// <summary>
    /// movement sequence
    /// </summary>
    [System.Serializable]
    public class MoveSequence
    {
        [Tooltip("The key to all the magic")]
        public string Name = string.Empty;//the key to all the magic
        [Tooltip("Should the action require an Airborne cost?")]
        public int AirborneCost = 0;

        public CharacterRequirements CharacterRequirements = new CharacterRequirements();
        public CharacterActionSequence MovementBehavior = new CharacterActionSequence();
    }


   
}
