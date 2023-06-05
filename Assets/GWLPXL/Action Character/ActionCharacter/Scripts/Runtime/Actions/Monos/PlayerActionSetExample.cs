using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{


    /// <summary>
    /// Mono wrapper that demos how to remove/add actions to the player instance
    /// </summary>
    public class PlayerActionSetExample : MonoBehaviour
    {

        public PlayerCharacterCC Player;
        public CharacterActionLoadoutSO PlayerActionSets;

        public ComboDynamicType EraseExisting  = ComboDynamicType.ReplaceAll;
        // Start is called before the first frame update
     
   
       [ContextMenu("Clear Actions")]
       public void ClearActions()
        {
            ActionManager.ClearActions(Player);
        }

        [ContextMenu("Add")]
        public void Add()
        {
            ActionManager.AddActionSet(Player, PlayerActionSets, EraseExisting);
           
        }
        [ContextMenu("Remove")]
        public void Remove()
        {
            ActionManager.RemoveActionSet(Player, PlayerActionSets);
        }
    }

}