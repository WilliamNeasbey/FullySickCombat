using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{
    [System.Serializable]
    public class InputActionSet
    {
        public List<InputActionMap> Maps = new List<InputActionMap>();
        public InputActionSet(List<InputActionMap> maps)
        {
            Maps = maps;
        }
    }

    /// <summary>
    /// delete, no longer in use
    /// </summary>
    [System.Serializable]
    public class InputActionMap
    {
        public int ActionMapSlot = 0;
        public string ActionName = null;//move to string

        public InputActionMap(string actionName, int slot)
        {
            ActionName = actionName;

            ActionMapSlot = slot;
        }
    }

    [System.Serializable]
    public class InputSlot
    {
       // public int SlotID;
        public InputActionSlotSO Requirements;
        public List<string> ActionList { get; set; }
        public Dictionary<string, int> Actions { get; set; }
        public InputSlot()
        {
            //SlotID = id;
            ActionList = new List<string>();
            Actions = new Dictionary<string, int>();
        }
    }

    [CreateAssetMenu(fileName = "New Input Action Map", menuName = "GWLPXL/ActionCharacter/Input/Action Map", order = 300)]
    public class InputActionMapSO : ScriptableObject
    {

        public List<InputSlot> InputSlots = new List<InputSlot>();

        public Dictionary<int, InputSlot> SlotIDs = new Dictionary<int, InputSlot>();

        [System.NonSerialized]
        protected Dictionary<string, int> inputDic = new Dictionary<string, int>();
        protected InputRequirements empty = new InputRequirements();
  
        public virtual List<InputActionSlotSO> GetSlots()
        {
            List<InputActionSlotSO> _ = new List<InputActionSlotSO>(InputSlots.Count);
            for (int i = 0; i < InputSlots.Count; i++)
            {
                _.Add(InputSlots[i].Requirements);
            }
            return _;
        }
        public virtual void SetInputs(List<InputActionSlotSO> slotSO)
        {
            for (int i = 0; i < slotSO.Count; i++)
            {
                InputSlots[i].Requirements.Key = i;
                InputSlots[i].Requirements = slotSO[i];
            }
        }
        public virtual List<InputRequirements> GetAllRequirementsCopy()
        {
            List<InputRequirements> temp = new List<InputRequirements>();
            for (int i = 0; i < InputSlots.Count; i++)
            {
                InputRequirements newreq = new InputRequirements();
                newreq.InputButtons = new List<InputButton>();
                for (int j = 0; j < InputSlots[i].Requirements.InputRequirements.InputButtons.Count; j++)
                {
                    InputButton t = InputSlots[i].Requirements.InputRequirements.InputButtons[j];
                    InputButton cloneb = new InputButton(t.ButtonName, t.Type);
                    newreq.InputButtons.Add(cloneb);
                }
                newreq.MovementAxisRequirement = new InputAxisFreeForm(InputSlots[i].Requirements.InputRequirements.MovementAxisRequirement.RequirementType);
                temp.Add(InputSlots[i].Requirements.InputRequirements);
            }
            return temp;
        }
        public virtual void ConsumeInput(int index)
        {
            if (SlotIDs.ContainsKey(index))
            {
                InputSlot slot = SlotIDs[index];
                for (int i = 0; i < slot.Requirements.InputRequirements.InputButtons.Count; i++)
                {
                    slot.Requirements.InputRequirements.InputButtons[i].Value = false;
                  
                }

                
            }
        }
        public virtual void Setup()
        {
            foreach (var kvp in inputDic)
            {
                InputSlot slot = SlotIDs[kvp.Value];
                slot.ActionList.Clear();
                slot.Actions.Clear();
            }
            inputDic.Clear();

            for (int i = 0; i < InputSlots.Count; i++)
            {
                InputSlot slot = InputSlots[i];
                
                InputSlots[i].ActionList.Clear();
                InputSlots[i].Actions.Clear();
                int key = i;
                InputSlots[i].Requirements.Key = key;
                SlotIDs[key] = InputSlots[i];
                ConsumeInput(key);
            }
        }

        protected virtual void UnRegisterMap(InputActionMap map)
        {

            int index = map.ActionMapSlot;
            
            string key = CommonFunctions.StringKey(map.ActionName);

            if (SlotIDs.ContainsKey(index))
            {
                InputSlot slot = SlotIDs[index];
                if (slot.Actions.ContainsKey(key))
                {
                    slot.Actions.Remove(key);
                    if (slot.ActionList.Contains(key) == true)
                    {
                        slot.ActionList.Remove(key);
                    }
                }

            }



        }

        //currently does one, we need one that does them in groups.
        protected virtual void RegisterMap(InputActionMap map)
        {

            int index = map.ActionMapSlot;
            string key = CommonFunctions.StringKey(map.ActionName);

            if (SlotIDs.ContainsKey(index))
            {
                InputSlot slot = SlotIDs[index];
                slot.Actions[key] = 1;
                SlotIDs[index] = slot;
                if (slot.ActionList.Contains(key) == false)
                {
                    slot.ActionList.Add(key);
                }
            }
           


        }

        

        
        public virtual InputRequirements GetInputRequirements(int index, string actionName)
        {


            if (SlotIDs.ContainsKey(index))
            {
                InputSlot slot = SlotIDs[index];
                string key = CommonFunctions.StringKey(actionName);
                if (slot.Actions.ContainsKey(key))
                {
                    return slot.Requirements.InputRequirements;
                }
   
            }
            return empty;
        }

        public virtual void AddInputActionSet(InputActionSet set)
        {

            List<InputActionMap> initialList = set.Maps;
            for (int i = 0; i < InputSlots.Count; i++)
            {
                int index = InputSlots[i].Requirements.Key;
                List<InputActionMap> inputset = initialList.Where(item => item.ActionMapSlot == index).ToList();

                for (int j = 0; j < inputset.Count; j++)
                {
                    InputSlot slot = SlotIDs[index];
                    string key = CommonFunctions.StringKey(inputset[j].ActionName);

                    slot.Actions[key] = 1;
                    SlotIDs[index] = slot;
                    if (slot.ActionList.Contains(key) == false)
                    {
                        slot.ActionList.Add(key);
                    }
                }
            }

        }
        public virtual void RemoveInputActionSet(InputActionSet set)
        {
     
            List<InputActionMap> initialList = set.Maps;
            for (int i = 0; i < InputSlots.Count; i++)
            {
                int index = InputSlots[i].Requirements.Key;

                List<InputActionMap> inputset = initialList.Where(item => item.ActionMapSlot == index).ToList();
                if (inputset.Count > 0)
                {
                    if (SlotIDs.ContainsKey(index))
                    {
                        SlotIDs[index].ActionList.Clear();
                        SlotIDs[index].Actions.Clear();
                    }

                }
               
                
            }
        }
        public virtual void AddInputActionMap(InputActionMap newmap)
        {
            RegisterMap(newmap);
        }

        public virtual void RemoveInputActionMap(InputActionMap map)
        {
            UnRegisterMap(map);
        }
       
    }
}