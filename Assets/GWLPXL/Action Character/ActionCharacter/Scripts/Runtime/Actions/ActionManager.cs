using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.Linq;

namespace GWLPXL.ActionCharacter
{
    
    /// <summary>
    /// starts, ends, continues, and knows about actions
    /// </summary>
    public static class ActionManager
    {
#if UNITY_2019_3_OR_NEWER && UNITY_EDITOR // Introduced in 2019.3. Also can cause problems in builds so only for editor.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init()
        {
            inputBuffer = new Dictionary<int, InputBufferMap>();
            currentAction = new Dictionary<int, string>();//id, action name
            actionCharacters = new Dictionary<int, ActionTracker>();//id, tracker
             registered = new Dictionary<int, ActionCharacter>();
            inActionList = new List<int>();//id
           actionsos = new List<ActionSO>();

             action = string.Empty;
             sceneRunner = null;
            hasSceneRunner = false;
            runtimeLoadout = new Dictionary<ActionCharacter, CharacterActionLoadoutSO>();

          names = new List<string>();
           filtered2 = new Dictionary<string, int>();
           filteredSet = new Dictionary<string, int>();
            // Initialize your stuff here.
        }
#endif


        public static bool SendDebugMessage = true;
        public static event Action<CombosUpdated> OnCombosUpdated;
        public static event Action<ActionContext> OnActionComplete;
        public static event Action<ActionContext> OnActionStart;
        public static event Action<ActionContext> OnNextSequence;

        //public static Dictionary<int, InputBufferMap> InputBuffer => inputBuffer;
        static Dictionary<int, InputBufferMap> inputBuffer = new Dictionary<int, InputBufferMap>();//id, index

        static readonly InputBufferMap empty = new InputBufferMap(-1, "Empty");

        static Dictionary<int, string> currentAction = new Dictionary<int, string>();//id, action name
        static Dictionary<int, ActionTracker> actionCharacters = new Dictionary<int, ActionTracker>();//id, tracker
        static Dictionary<int, ActionCharacter> registered = new Dictionary<int, ActionCharacter>();
        static List<int> inActionList = new List<int>();//id
        static List<ActionSO> actionsos = new List<ActionSO>();

        static List<int> preventList = new List<int>();
        static string action = string.Empty;
        static ActionSceneRunner sceneRunner = null;
        static bool hasSceneRunner = false;
        static Dictionary<ActionCharacter, CharacterActionLoadoutSO> runtimeLoadout = new Dictionary<ActionCharacter, CharacterActionLoadoutSO>();

        static List<string> names = new List<string>();
        static Dictionary<string, int> filtered2 = new Dictionary<string, int>();
        static Dictionary<string, int> filteredSet = new Dictionary<string, int>();
        public static void Register(ActionCharacter instance)
        {
            if (hasSceneRunner == false)
            {
                sceneRunner = GameObject.FindObjectOfType<ActionSceneRunner>();
                if (sceneRunner == null)
                {
                    GameObject obj = new GameObject();
                    sceneRunner = obj.AddComponent<ActionSceneRunner>();
                    obj.name = "Action Scene Runner";
                }
                hasSceneRunner = sceneRunner != null;
            }
            registered[instance.ID] = instance;
            

        }
        public static void UnRegister(int id)
        {
            if (registered.ContainsKey(id))
            {
                if (InActionSequence(id))
                {
                    EndAction(registered[id], GetActionTicker(id), false);
                }
                registered.Remove(id);
            }
        }

        public static List<ActionSO> GetActionSOs(List<string> names, ActionDatabase database)
        {
            actionsos.Clear();
            for (int i = 0; i < names.Count; i++)
            {
                actionsos.Add(database.GetAction(names[i]));
            }
            return actionsos;
        }
        public static void AddCombos(ComboArgs args, ComboDynamicType type, List<string> toReplace = null)
        {
            CharacterActionLoadoutSO loadout = ScriptableObject.CreateInstance<CharacterActionLoadoutSO>();
            loadout.Combos = args.Combos;
            ActionManager.AddActionSet(args.Instance, loadout, type, toReplace);
        }
        public static void RemoveCombos(ComboArgs args)
        {
            CharacterActionLoadoutSO loadout = ScriptableObject.CreateInstance<CharacterActionLoadoutSO>();
            loadout.Combos = args.Combos;
            ActionManager.RemoveActionSet(args.Instance, loadout);
        }

        public static void ClearActions(ActionCharacter instance)
        {
            instance.RemoveAllActions();
            instance.RemoveAllLoadoutOverrides();
            instance.AnimatorController.RemoveAllOverrides();
          
        }

        /// <summary>
        /// public call into adding/removing/replacing combos
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="addActionSet"></param>
        /// <param name="type"></param>
        /// <param name="selected"></param>
        public static void AddActionSet(ActionCharacter instance, CharacterActionLoadoutSO addActionSet, ComboDynamicType type, List<string> selected = null)
        {
            switch (type)
            {
                case ComboDynamicType.ReplaceAll:
                    AddActionSet(instance, addActionSet, true);
                    break;
                case ComboDynamicType.Add:
                    AddActionSet(instance, addActionSet, false);
                    break;
                case ComboDynamicType.Remove:
                    RemoveActionSet(instance, addActionSet);
                    break;
                case ComboDynamicType.ReplaceSelected:
                    if (selected != null && selected.Count > 0)
                    {
                        CharacterActionLoadoutSO runtime = GetRuntimeLoadout(instance);
                        for (int i = 0; i < selected.Count; i++)
                        {
                            string c = CommonFunctions.StringKey(selected[i]);
                            for (int j = 0; j < runtime.Combos.Count; j++)
                            {
                                string now = CommonFunctions.StringKey(runtime.Combos[i].Name);
                                if (CommonFunctions.WordEquals(c, now))
                                {
                                    runtime.Combos.RemoveAt(j);
                                }
                            }
                        }

                        AddActionSet(instance, addActionSet, false);
                    }
                    else
                    {
                        Debug.LogWarning("Trying to replace selected actions on loadout, but selected is null or empty.");
                    }
                    break;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="addActionSet"></param>
        /// <param name="removeExisting"></param>
        static void AddActionSet(ActionCharacter instance, CharacterActionLoadoutSO addActionSet, bool removeExisting = true)
        {

            if (ActionManager.InActionSequence(instance.ID))
            {
          //keep an eye, this isn't ideal
                ActionManager.EndAction(instance, ActionManager.GetActionTicker(instance.ID));
            }
            CharacterActionLoadoutSO runtime = GetRuntimeLoadout(instance);

            if (removeExisting)
            {
                runtime.Combos.Clear();
                runtime.Overrides.ClipOverrides.Clear();
                ClearActions(instance);
            }

            #region animoverrides
            List<AnimOverride> _ = new List<AnimOverride>();
            for (int i = 0; i < addActionSet.Overrides.ClipOverrides.Count; i++)
            {
                AnimOverride aoverride = addActionSet.Overrides.ClipOverrides[i];
                AnimationClip original = aoverride.OriginalClip;
                AnimationClip clipoverride = aoverride.Override;
                bool add = true;
                for (int j = 0; j < runtime.Overrides.ClipOverrides.Count; j++)
                {
                    AnimOverride current = runtime.Overrides.ClipOverrides[j];
                    AnimationClip currentclip = current.OriginalClip;
                    if (original == currentclip)
                    {
                        add = false;
                        current.Override = clipoverride;
                        break;
                    }
                }

                if (add)
                {
                    _.Add(new AnimOverride(original, clipoverride));
                }
            }

            for (int i = 0; i < _.Count; i++)
            {
                runtime.Overrides.ClipOverrides.Add(_[i]);
            }

            #endregion

            #region combos
            List<Combos> newadds = new List<Combos>();
            for (int i = 0; i < addActionSet.Combos.Count; i++)
            {
                string newone = addActionSet.Combos[i].Name;
                bool add = true;
                for (int j = 0; j < runtime.Combos.Count; j++)
                {
                    string existing = runtime.Combos[j].Name;
                    if (CommonFunctions.WordEquals(newone, existing))
                    {
                        add = false;
                        break;
                    }
                }

                if (add)
                {
                    newadds.Add(addActionSet.Combos[i]);
                }
            }

            for (int i = 0; i < newadds.Count; i++)
            {
                runtime.Combos.Add(newadds[i]);
            }
            #endregion

            ComboManager.VerifyCombos(runtime);

           
            instance.AddActionSet(runtime.GetActionSets());
            instance.AddLoadoutOverride(addActionSet.LoadoutName);
            instance.AnimatorController.SetOverrides(runtime.Overrides.ClipOverrides);
            instance.Loadout = runtime;

            OnCombosUpdated?.Invoke(new CombosUpdated(instance));
        }

        public static void LoadoutRefresh(ActionCharacter instance)
        {
            CharacterActionLoadoutSO runtime = GetRuntimeLoadout(instance);
            instance.AddActionSet(runtime.GetActionSets());
            instance.AnimatorController.SetOverrides(runtime.Overrides.ClipOverrides);//redo
            instance.Loadout = runtime;
        }
        public static CharacterActionLoadoutSO GetRuntimeLoadout(ActionCharacter instance)
        {
            if (runtimeLoadout.ContainsKey(instance) == false)
            {
                CharacterActionLoadoutSO first = ScriptableObject.CreateInstance<CharacterActionLoadoutSO>();
                first.ActionSets = new List<ActionSet>();
                first.Combos = new List<Combos>();
                first.LoadoutName = instance.name;
                first.name = instance.name;
                runtimeLoadout[instance] = first;
            }
            return runtimeLoadout[instance];
        }
        public static List<Combos> GetMyCombos(ActionCharacter instance)
        {
            return GetRuntimeLoadout(instance).Combos;

        }
        public static void RemoveActionSet(ActionCharacter instance, CharacterActionLoadoutSO removeSet)
        {
            if (ActionManager.InActionSequence(instance.ID))
            {
                ActionManager.EndAction(instance, ActionManager.GetActionTicker(instance.ID));
            }
            CharacterActionLoadoutSO runtime = GetRuntimeLoadout(instance);

            ClearActions(instance);//clear because we rebuild

            #region animoverrides
            List<AnimOverride> _ = new List<AnimOverride>();
            for (int i = 0; i < removeSet.Overrides.ClipOverrides.Count; i++)
            {
                AnimOverride aoverride = removeSet.Overrides.ClipOverrides[i];
                AnimationClip original = aoverride.OriginalClip;
                AnimationClip clipoverride = aoverride.Override;
                bool remove = false;
                for (int j = 0; j < runtime.Overrides.ClipOverrides.Count; j++)
                {
                    AnimOverride current = runtime.Overrides.ClipOverrides[j];
                    AnimationClip currentclip = current.OriginalClip;
                    if (original == currentclip)
                    {
                        remove = true;
                        break;
                    }
                }

                if (remove)
                {
                    _.Add(aoverride);
                }
            }

            for (int i = 0; i < _.Count; i++)
            {
                runtime.Overrides.ClipOverrides.Remove(_[i]);
            }

            #endregion

            #region combos
            List<Combos> newadds = new List<Combos>();
            for (int i = 0; i < removeSet.Combos.Count; i++)
            {
                string newone = removeSet.Combos[i].Name;
                bool remove = false;
                for (int j = 0; j < runtime.Combos.Count; j++)
                {
                    string existing = runtime.Combos[j].Name;
                    if (CommonFunctions.WordEquals(newone, existing))
                    {
                        remove = true;
                        break;
                    }
                }

                if (remove)
                {
                    newadds.Add(removeSet.Combos[i]);
                }
            }

            for (int i = 0; i < newadds.Count; i++)
            {
                runtime.Combos.Remove(newadds[i]);
            }
            #endregion

            ComboManager.VerifyCombos(runtime);

            instance.AddActionSet(runtime.GetActionSets());
            instance.RemoveLoadoutOverride(removeSet.LoadoutName);//redo
            instance.AnimatorController.SetOverrides(runtime.Overrides.ClipOverrides);//redo
            instance.Loadout = runtime;

            OnCombosUpdated?.Invoke(new CombosUpdated(instance));
        }
        /// <summary>
        /// clears dictionaries, resets characters and action tickers
        /// </summary>
        public static void Clear()
        {
            inputBuffer.Clear();
            currentAction.Clear();
            inActionList.Clear();
            List<ActionTracker> remove = new List<ActionTracker>();
            foreach (var kvp in actionCharacters)
            {
                remove.Add(kvp.Value);

            }

           
            for (int i = 0; i < remove.Count; i++)
            {
                remove[i].CurrentMove.RemoveTicker();
                //EndAction(remove[i].ActionObject, remove[i].CurrentMove);//this gives us null.
            }
            remove.Clear();
            actionCharacters.Clear();
            registered.Clear();
            hasSceneRunner = false;
            sceneRunner = null;
        }

        public static void UpdateInputBuffer(int id, InputBufferMap map)
        {
            inputBuffer[id] = map;
        }

        public static InputBufferMap GetInputBufferMap(int id)
        {
            if (inputBuffer.ContainsKey(id))
            {
                return inputBuffer[id];
            }
            return new InputBufferMap(-1, "NULL");
        }
        public static int GetInputSlotIndex(int id)
        {
            if (inputBuffer.ContainsKey(id))
            {
                return inputBuffer[id].InputSlotIndex;
            }
            return -1;
        }

        /// <summary>
        /// get input, no callback stores to inputbuffer
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="checkinput"></param>
        /// <param name="checktransition"></param>
        /// <returns></returns>
        /// 


        public static void ResetBufferMap(ActionCharacter instance)
        {
            InputBufferMap map = GetBufferMap(instance);
            map.ActionName = "EMPTY";
            map.InputSlotIndex = -1;
            inputBuffer[instance.ID] = map;
        }
        public static InputBufferMap GetBufferMap(ActionCharacter instance)
        {
            if (inputBuffer.ContainsKey(instance.ID) == false)
            {
                inputBuffer[instance.ID] = new InputBufferMap(-1, "EMPTY");
            }
            return inputBuffer[instance.ID];
        }
        public static InputBufferMap GetActionInputRequirements(ActionCharacter instance, bool checktransition = true)
        {
            if (inputBuffer.ContainsKey(instance.ID) == false)
            {
                inputBuffer[instance.ID] = new InputBufferMap(-1, "EMPTY");
            }

            List<InputSlot> slots = instance.InputRequirements.InputSlots;
            filteredSet.Clear();
            for (int i = 0; i < slots.Count; i++)
            {
                List<string> actionlist = slots[i].ActionList;
                int slotkey = slots[i].Requirements.Key;
                for (int j = 0; j < actionlist.Count; j++)
                {
                    string name = actionlist[j];
                    ActionSO so = instance.ActionsDatabase.Database.GetAction(name);
                    if (CharacterActionRequirementsSuccess(instance, so))
                    {
                        filteredSet[name] = slotkey;
                    }
                }
            }


            names.Clear();
            filtered2.Clear();
            foreach (var kvp in filteredSet)
            {
                if (InputSuccess(instance, kvp.Value, kvp.Key))
                {
                    filtered2[kvp.Key] = kvp.Value;
                    names.Add(kvp.Key);
                }
            }


            action = string.Empty;
            int finalslot = -1;
            foreach (var kvp in filtered2)
            {
                string actionname = kvp.Key;
                ActionSO so = instance.GetAction(actionname);
                if (checktransition)
                {
                    if (instance.Flow.CanTransition(actionname))
                    {
                        action = actionname;
                        finalslot = kvp.Value;
                    }
                }
                else
                {
                    action = actionname;
                    finalslot = kvp.Value;
                }



            }


            if (finalslot > -1)
            {
                //we got something
                InputBufferMap map = new InputBufferMap(finalslot, action);
                inputBuffer[instance.ID] = map;
                DebugMessage(DebugHelpers.FormatInputDebug("Input ") + "Input Buffer " + action, instance);
                return inputBuffer[instance.ID];
            }



            return empty;
        }

        public static bool GetInputRequirements(ActionCharacter instance, int inputSlotKey, string actionName)
        {
            if (instance.InputRequirements.SlotIDs.ContainsKey(inputSlotKey))
            {
                InputSlot slot = instance.InputRequirements.SlotIDs[inputSlotKey];
                string key = CommonFunctions.StringKey(actionName);
                if (slot.Actions.ContainsKey(key))
                {

                    ActionSO so = instance.ActionsDatabase.Database.GetAction(actionName);
                    if (CharacterActionRequirementsSuccess(instance, so))
                    {
                        return InputSuccess(instance, inputSlotKey, actionName);
                    }
                }
            }

            return false;
        }
        /// <summary>
        /// checks axis and button requirements, return true if success
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="requirements"></param>
        /// <returns></returns>
        public static bool InputSuccess(ActionCharacter instance, int slot, string actionname)
        {
            InputActionMapSO actionmap = instance.InputRequirements;
            InputRequirements requirements = actionmap.GetInputRequirements(slot, actionname);

            List<InputButton> buttons = requirements.InputButtons;
            InputAxisFreeForm moveAxis = requirements.MovementAxisRequirement;
            int count = 0;
            bool pressed = false;
            for (int i = 0; i < buttons.Count; i++)//this button logic seems wrong?
            {
                pressed = instance.InputWrapper.GetButton(buttons[i]);
                if (pressed)
                {
                    count += 1;
                }


            }

            bool buttonsuccess = count == buttons.Count;
            bool moveaxissuccess = instance.InputWrapper.GetMoveAxis(instance, moveAxis);

            if (SendDebugMessage)
            {
                if (buttonsuccess)
                {
                    DebugMessage(DebugHelpers.FormatInputDebug("Button ") + DebugHelpers.FormatSuccessResponse("Input Success ") + actionname, instance);
                }
                else
                {
                    DebugMessage(DebugHelpers.FormatInputDebug("Button ") + DebugHelpers.FormatFailedResponse("Input Failed ") + actionname, instance);
                }

                if (moveaxissuccess)
                {
                    DebugMessage(DebugHelpers.FormatInputDebug("Move Axis ") + DebugHelpers.FormatSuccessResponse("Input Success ") + actionname, instance);
                }
                else
                {
                    DebugMessage(DebugHelpers.FormatInputDebug("Move Axis ") + DebugHelpers.FormatFailedResponse("Input Failed ") + actionname, instance);

                }
            }


            return buttonsuccess == true && moveaxissuccess == true;

        }

        /// <summary>
        /// for external checking
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="requirements"></param>
        /// <returns></returns>
        public static bool InputCheck(ActionCharacter instance, InputRequirements requirements)
        {
            List<InputButton> buttons = requirements.InputButtons;
            InputAxisFreeForm moveAxis = requirements.MovementAxisRequirement;
            int count = 0;
            bool pressed = false;
            for (int i = 0; i < buttons.Count; i++)//this button logic seems wrong?
            {
                pressed = instance.InputWrapper.GetButton(buttons[i]);
                if (pressed)
                {
                    count += 1;
                }


            }

            bool buttonsuccess = count == buttons.Count;
            bool moveaxissuccess = instance.InputWrapper.GetMoveAxis(instance, moveAxis);

            if (SendDebugMessage)
            {
                if (buttonsuccess)
                {
                    DebugMessage(DebugHelpers.FormatInputDebug("Button ") + DebugHelpers.FormatSuccessResponse("Input Success ") + instance.name, instance);
                }
                else
                {
                    DebugMessage(DebugHelpers.FormatInputDebug("Button ") + DebugHelpers.FormatFailedResponse("Input Failed ") + instance.name, instance);
                }

                if (moveaxissuccess)
                {
                    DebugMessage(DebugHelpers.FormatInputDebug("Move Axis ") + DebugHelpers.FormatSuccessResponse("Input Success ") + instance.name, instance);
                }
                else
                {
                    DebugMessage(DebugHelpers.FormatInputDebug("Move Axis ") + DebugHelpers.FormatFailedResponse("Input Failed ") + instance.name, instance);

                }
            }


            return buttonsuccess == true && moveaxissuccess == true;
        }
        
        public static void CheckAnimatorState(Animator animator, ActionSO action)
        {
            List<ActionVars> vars = action.GetSequenceVars();
            string name = action.GetActionName();
            for (int i = 0; i < vars.Count; i++)
            {
                CheckAnimatorState(animator, vars[i], name);
            }
        }
        public static void CheckAnimatorState(Animator animator, ActionVars forAction, string actionname)
        {
            int stateid = Animator.StringToHash(forAction.AnimatorVars.AnimatorStateName);
            bool has = animator.HasState(forAction.AnimatorVars.Layer, stateid);
            if (has == false)
            {
                Debug.LogWarning("No State named " + forAction.AnimatorVars.AnimatorStateName + " exists on layer " + forAction.AnimatorVars.Layer +
                    ". No Animation will play with Action " + actionname + " until a state is set in the Animator");
            }
        }
        public static void CheckAnimatorStates(Animator animator, ActionsDatabaseSO actionsdatabaseSO)
        {
            if (animator == null)
            {
                Debug.LogError("Trying to check animator but entry is null");
                return;
            }
            else if (actionsdatabaseSO == null)
            {
                Debug.LogError("Trying to check animator but action database is null");
                return;
            }
            for (int i = 0; i < actionsdatabaseSO.Database.Actions.Count; i++)
            {
                CheckAnimatorState(animator, actionsdatabaseSO.Database.Actions[i]);

            }
        }

        /// <summary>
        /// checks character state against required states, returns true if requirements met
        /// TO DO: coyote time
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="requirements"></param>
        /// <returns></returns>
        public static bool CharacterActionRequirementsSuccess(ActionCharacter instance, ActionSO requirements)
        {
            return requirements.HasRequirements(instance);
            
        }


        public static InputBufferMap GetActionMap(int characterID)
        {
            if (actionCharacters.ContainsKey(characterID))
            {
                return actionCharacters[characterID].Map;
            }
            return empty;
        }
        /// <summary>
        /// get index of current action
        /// </summary>
        /// <param name="characterID"></param>
        /// <returns></returns>
        public static int GetActionIndex(int characterID)
        {
            if (actionCharacters.ContainsKey(characterID))
            {
                return actionCharacters[characterID].Index;
            }
            return -1;
        }
        /// <summary>
        /// is character in action?
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public static bool InActionSequence(int character)
        {
            return inActionList.Contains(character);

        }
        public static ActionTickerCC GetActionTickerCC(int ownderID)
        {
            if (actionCharacters.ContainsKey(ownderID))
            {
                return actionCharacters[ownderID].CurrentMove as ActionTickerCC;
            }
            return null;
        }
        public static ActionTicker GetActionTicker(int ownderID)
        {
            if (actionCharacters.ContainsKey(ownderID))
            {
                return actionCharacters[ownderID].CurrentMove;
            }
            return null;
        }
        /// <summary>
        /// get name of current action
        /// </summary>
        /// <param name="ownderID"></param>
        /// <returns></returns>
        public static string GetActionName(int ownderID)
        {
            if (actionCharacters.ContainsKey(ownderID))
            {
                return actionCharacters[ownderID].ActionName;
            }
            return string.Empty;
        }
        /// <summary>
        /// get characterinstance in action
        /// </summary>
        /// <param name="ownderID"></param>
        /// <returns></returns>
        public static ActionCharacter GetActionObject(int ownderID)
        {
            if (actionCharacters.ContainsKey(ownderID))
            {
                return actionCharacters[ownderID].ActionObject;
            }
            return null;
        }

       /// <summary>
       /// big hauncho that starts the action sequence, override forcestart to force the start.
       /// </summary>
       /// <param name="instance"></param>
       /// <param name="extraMoveIndex"></param>
       /// <param name="forceStart"></param>
        public static bool TryStartAction(ActionCharacter instance, string actionName, bool forceStart = false)
        {
            if (preventList.Contains(instance.ID))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(actionName))
            {
                return false;
            }
            ActionSO so = instance.ActionsDatabase.Database.GetAction(actionName);
            if (so == null)
            {
                return false;
            }

            int id = instance.ID;
            InputBufferMap map = new InputBufferMap(-1, "NULL");
            if (instance is PlayerCharacter)
            {
                if (inputBuffer.ContainsKey(id))
                {
                    map = new InputBufferMap(inputBuffer[id].InputSlotIndex, inputBuffer[id].ActionName);
                }

            }


            if (forceStart == false)
            {
                if (InActionSequence(id)) return false;
            }
            else
            {
                if (InActionSequence(id))
                {
                    ActionTracker tracker = actionCharacters[id];
                    EndAction(instance, tracker.CurrentMove, false);
                }
            }



            int startindex = 0;
            ActionTicker dashCC = so.GetAction(instance, startindex);
            dashCC.AddTicker();

            string current = actionName;
            if (instance.Flow != null)
            {
                instance.Flow.SetCurrent(current);
            }
            
            instance.ModifyAirborneActions(so.GetAirborneCost());
            actionCharacters[id] = new ActionTracker(current, startindex, so, map, dashCC, instance);
            inActionList.Add(id);
            currentAction[id] = current;
            OnActionStart?.Invoke(new ActionContext(current, instance, dashCC));

            DebugMessage(DebugHelpers.FormatActionDebug("Action ") + "current " + current, instance);
            return true;


        }

/// <summary>
/// checks tracker, if another sequence in the action continues with that, otherwise ends
/// </summary>
/// <param name="instance"></param>
/// <param name="old"></param>
        public static void ContinueActionSequence(ActionCharacter instance, ActionTicker old)
        {
            int id = instance.ID;
            if (actionCharacters.ContainsKey(id) == false) return;//not found

            if (preventList.Contains(instance.ID))
            {
                EndAction(instance, old);
                return;
            }

            ActionTracker tracker = actionCharacters[id];

            tracker.Index++;
            if (tracker.Index > tracker.Action.GetSequenceVars().Count - 1)
            {
                //complete
                EndAction(instance, old);
            }
            else
            {
                inputBuffer[id] = new InputBufferMap(-1, "EMPTY");
                old.RemoveTicker();
                ActionTicker ticker = tracker.Action.GetAction(instance, tracker.Index);
                tracker.CurrentMove = ticker;
                tracker.CurrentMove.AddTicker();
                instance.Events.FireNextSequenceStarted(new CharacterActionArgs(instance, tracker.ActionName));
                OnNextSequence?.Invoke(new ActionContext(tracker.ActionName, tracker.ActionObject, ticker));
                DebugMessage(DebugHelpers.FormatActionDebug("Action ") + "continue  " + tracker.ActionName, instance);
            }
        }

        /// <summary>
        /// transition shortcut, calls endaction and startactoin
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="old"></param>
        /// <param name="newMove"></param>
        public static void TransitionAction(ActionCharacter controller, ActionTicker old, string newMove)
        {
            old.ResetActionObject();
            old.RemoveTicker();
            bool started = TryStartAction(controller,  newMove, true);
            if (started)
            {
                CharacterActionArgs args = new CharacterActionArgs(controller, newMove);
                controller.Events.UnityEvents.OnActionSequenceStarted?.Invoke(args);
                controller.Events.FireActionStarted(args);
            }
        }
    /// <summary>
    /// will stop any current actions and prevent any further ones from initiating
    /// </summary>
    /// <param name="instance"></param>
       public static void PreventActions(ActionCharacter instance)
        {
            if (preventList.Contains(instance.ID) == false)
            {
                if (InActionSequence(instance.ID))
                {
                    EndAction(instance, GetActionTicker(instance.ID));
                }
                preventList.Add(instance.ID);
            }
        }

        /// <summary>
        /// will allow actions on the character
        /// </summary>
        /// <param name="instance"></param>
        public static void AllowActions(ActionCharacter instance)
        {
            if (preventList.Contains(instance.ID))
            {
                preventList.Remove(instance.ID);
            }
        }
        /// <summary>
        /// ends current action sequence, override to stop the default transition back to locomotion
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="old"></param>
        /// <param name="transbacktoloco"></param>
        public static void EndAction(ActionCharacter instance, ActionTicker old, bool transbacktoloco = true)
        {

            int key = instance.ID;

            if (actionCharacters.ContainsKey(key))
            {

                old.ResetActionObject();
                old.RemoveTicker();

                inputBuffer[key] = new InputBufferMap(-1, "EMPTY");
                inputBuffer.Remove(key);
                inActionList.Remove(key);
                ActionTracker tracker = actionCharacters[key];
                tracker.CurrentMove.RemoveTicker();
                actionCharacters.Remove(key);
                currentAction[key] = string.Empty;
               

                instance.Flow.SetCurrent(string.Empty);
                instance.ActionSequenceEnded(tracker.ActionName);

                OnActionComplete?.Invoke(new ActionContext(tracker.ActionName, instance, tracker.CurrentMove));


                if (transbacktoloco)
                {

                    FreeFormState state = instance.GetCharacterStateAC();
                    switch (state)
                    {
                        case FreeFormState.Airborne:
                            instance.AnimatorController.PlayCrossFadeFixed(instance.Config.Defaults.Airborne, instance.Config.Defaults.AirborneTrans);
                            break;
                        case FreeFormState.Coyote:
                            instance.AnimatorController.PlayCrossFadeFixed(instance.Config.Defaults.Airborne, instance.Config.Defaults.AirborneTrans);
                            break;
                        case FreeFormState.Ground:
                            instance.AnimatorController.PlayCrossFadeFixed(instance.Config.Defaults.Locomotion, instance.Config.Defaults.LocomotionTrans);
                            break;
                    }
                }
                
            }
                

        }

        public static Vector3 GetStep(Vector3 start, Transform t1, float x, float y, float z)
        {
            Vector3 step = start + t1.right * x + t1.up * y + t1.forward * z;
            return step;
        }
        public static float Calculate(Distances distance, float percent, float goal, bool hasCurve = false)
        {
            float value = 0;
            if (hasCurve)
            {
                percent = distance.Curve.Evaluate(percent);
            }
            value = Mathf.Lerp(0, goal, percent);
            if (percent >= 1)
            {
                value = goal;
            }

          
            return value;
        }

        /// <summary>
        /// debug shortcut
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ctx"></param>
        static void DebugMessage(string message, UnityEngine.Object ctx)
        {
            if (SendDebugMessage)
            {
                DebugHelpers.DebugMessage(message, ctx, DebugMessageType.Log, 1);
            }

        }

        /// <summary>
        /// tracks action in progress
        /// </summary>
        [System.Serializable]
        public class ActionTracker
        {
            public string ActionName;
            public ActionSO Action;
            public InputBufferMap Map;
            public ActionCharacter ActionObject;
            public int Index;
            public ActionTicker CurrentMove;

            public ActionTracker(string actionName, int index, ActionSO action, InputBufferMap map, ActionTicker currentMove, ActionCharacter actionObject)
            {
                Action = action;
                Map = map;
                ActionObject = actionObject;
                ActionName = actionName;
                Index = index;
                CurrentMove = currentMove;

            }
        }


    }

    /// <summary>
    /// context for starting/ending actions
    /// </summary>
    [System.Serializable]
    public class ActionContext : EventArgs
    {
        public string ActionName;
        public ActionCharacter Character;
        public ActionTicker Action;

        public ActionContext(string actionname, ActionCharacter instance, ActionTicker action)
        {

            Action = action;
            ActionName = actionname;
            Character = instance;
        }
    }

    /// <summary>
    /// when combos are added/removed
    /// </summary>
    public class CombosUpdated : EventArgs
    {
        public ActionCharacter Instance;
        public CombosUpdated(ActionCharacter ac)
        {
            Instance = ac;
        }
    }

    /// <summary>
    /// input buffer
    /// </summary>
    public class InputBufferMap
    {
        public int InputSlotIndex;
        public string ActionName;

        public InputBufferMap(int slot, string action)
        {
            InputSlotIndex = slot;
            ActionName = action;
        }
    }
    [System.Serializable]
    public class ComboArgs : System.EventArgs
    {
        public ActionCharacter Instance;
        public List<Combos> Combos;

        public ComboArgs(ActionCharacter ac, List<Combos> combos)
        {
            Instance = ac;
            Combos = combos;
        }
    }

    public enum ComboDynamicType
    {
        None = 0,
        /// <summary>
        /// Adds to existing loadout
        /// </summary>
        Add = 1,
        /// <summary>
        /// REmoves from existing loadout
        /// </summary>
        Remove = 2,
        /// <summary>
        /// replaces existing loadout
        /// </summary>
        ReplaceAll = 3,
        /// <summary>
        /// replaces only those named in the string parameter
        /// </summary>
        ReplaceSelected = 4
    }
}