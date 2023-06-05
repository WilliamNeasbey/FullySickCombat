using System.Collections.Generic;
using UnityEngine;
namespace GWLPXL.ActionCharacter
{
    /// <summary>
    /// base actionset class
    /// </summary>
    [System.Serializable]
    public class ActionSet
    {
        public string Action;
        public Flow Flow;
        public int InputIndex = 0;
        protected InputActionMap actionMap;
        public ActionSet(string action, Flow flow, int inputKey)
        {
            Action = action;
            Flow = flow;
            InputIndex = inputKey;
        }

        public virtual InputActionMap GetActionMap()
        {
            actionMap = new InputActionMap(Action, InputIndex);
            return actionMap;
        }
    }

    [System.Serializable]
    public class CharacterActionSet : ActionSet
    {
      
        
        public CharacterActionSet(string action, Flow flow, int inputkey) : base(action, flow, inputkey)
        {

        }
    }

    /// <summary>
    /// player action set, derived from action set
    /// </summary>
    [System.Serializable]
    public class PlayerActionSet : ActionSet
    {

        public PlayerActionSet(string action, Flow flow, int inputkey) : base(action, flow, inputkey)
        {
  
        }

        //public override InputActionMap GetActionMap()
        //{
        //    actionMap = new InputActionMap(Action, InputActionSlot.Key);
        //    return actionMap;
        //}
    }


    [System.Serializable]
    public abstract class CharacterActionSets
    {
 
        public abstract List<ActionSet> GetActions();
    }
    /// <summary>
    /// list of action sets
    /// </summary>
    [System.Serializable]
    public class NPCCharacterActionSets : CharacterActionSets
    {

        public List<ActionSet> ActionSets = new List<ActionSet>();

        public override List<ActionSet> GetActions()
        {
            return ActionSets;
        }
    }

    /// <summary>
    /// list of player action sets
    /// </summary>
    [System.Serializable]
    public class PlayerActionSets : CharacterActionSets
    {
        public List<PlayerActionSet> ActionSets = new List<PlayerActionSet>();
        protected List<ActionSet> sets = new List<ActionSet>();
        public override List<ActionSet> GetActions()
        {
            sets.Clear();
            for (int i = 0; i < ActionSets.Count; i++)
            {
                sets.Add(ActionSets[i]);
            }
            return sets;

        }
    }
}