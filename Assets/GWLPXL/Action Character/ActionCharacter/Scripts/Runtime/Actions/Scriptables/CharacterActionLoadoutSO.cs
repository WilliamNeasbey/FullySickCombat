using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{

    //[CreateAssetMenu(fileName = "New NPC Character Action Loadout", menuName = "GWLPXL/ActionCharacter/Action/NPC Character Action Loadout", order = 220)]

    //public class NPCCharacterActionSetsSO : CharacterActionLoadoutSO
    //{
    //    public NPCCharacterActionSets ActionSets = new NPCCharacterActionSets();

    //    public override CharacterActionSets GetActionSets()
    //    {
    //        return ActionSets;
    //    }
    //}

    [System.Serializable]
    public class AnimOverride
    {
        public AnimationClip OriginalClip;
        public AnimationClip Override;

        public AnimOverride(AnimationClip original, AnimationClip overrider)
        {
            OriginalClip = original;
            Override = overrider;
        }
           
    }

    [System.Serializable]
    public class InstantiatedObjects
    {
        public string Parent;
        public GameObject Prefab;
    }
   
    [System.Serializable]
    public class LocomotionOverrides 
    {
        public List<AnimOverride> ClipOverrides = new List<AnimOverride>();
    }

    [System.Serializable]
    public class SimpleComboPiece
    {
        public ActionSO Action;
        public int InputSlotIndex = 0;
        public List<ActionSO> Cancels = new List<ActionSO>();
        [Tooltip("Duration of combo forgiveness.")]
        public float Forgiveness = .25f;

        public SimpleComboPiece(ActionSO action, int inputSlotIndex, List<ActionSO> cancels, float forgive)
        {
            Action = action;
            InputSlotIndex = inputSlotIndex;
            Cancels = cancels;
            Forgiveness = forgive;
        }
    }

    [System.Serializable]
    public class Combos
    {
        public string Name = string.Empty;
        public List<SimpleComboPiece> Pieces;
        public Combos(string name, List<SimpleComboPiece> pieces)
        {
         
            Name = name;
            Pieces = new List<SimpleComboPiece>();
            for (int i = 0; i < pieces.Count; i++)
            {
                Pieces.Add(new SimpleComboPiece(
                    pieces[i].Action,
                    pieces[i].InputSlotIndex,
                    pieces[i].Cancels,
                    pieces[i].Forgiveness
                    ));

            }
            
        }
    }

    [CreateAssetMenu(fileName = "New Character Action Loadout", menuName = "GWLPXL/ActionCharacter/Action/Character Action Loadout", order = 220)]
    public class CharacterActionLoadoutSO : ScriptableObject
    {
        public string LoadoutName = string.Empty;
        [HideInInspector]//not in effect at the moment
        public InstantiatedObjects WeaponOverrides = new InstantiatedObjects();
        [Tooltip("Non-action oriented overrides. For example, walking.")]
        public LocomotionOverrides Overrides = new LocomotionOverrides();
        public List<Combos> Combos = new List<Combos>();
        public List<ActionSet> ActionSets = new List<ActionSet>();
        protected string[] comboNames = new string[0];
        protected List<int> empty = new List<int>();
        public virtual List<int> GetInputSequence(string comboName)
        {
            string combokey = CommonFunctions.StringKey(comboName);
            for (int i = 0; i < Combos.Count; i++)
            {
                string key = CommonFunctions.StringKey(Combos[i].Name);
                if (CommonFunctions.WordEquals(key, combokey))
                {
                    return GetInputSequence(i);
                }
             
            }
            return empty;
        }
        public virtual List<int> GetInputSequence(int comboindex)
        {
            List<int> _ = new List<int>();
            for (int i = 0; i < Combos[comboindex].Pieces.Count; i++)
            {
                _.Add(Combos[comboindex].Pieces[i].InputSlotIndex);
            }
            return _;
        }
        public virtual LocomotionOverrides GetLocoOverrides()
        {
            return Overrides;
        }
       
        public virtual List<ActionSet> GetActionSets()
        {
            return ActionSets;
        }

        public virtual string[] GetComboNames()
        {
            comboNames = new string[Combos.Count];
            for (int i = 0; i < Combos.Count; i++)
            {
                comboNames[i] = Combos[i].Name;
            }
            return comboNames;
        }
    }
}
