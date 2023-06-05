using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
namespace GWLPXL.ActionCharacter
{

    public class PlayerInput
    {
        public int PlayerNumber;
        public ActionCharacter Character;
        public PlayerInput(int number, ActionCharacter avatar)
        {
            PlayerNumber = number;
            Character = avatar;
        }
    }
    public static class PlayerCharacterManager 
    {
#if UNITY_2019_3_OR_NEWER && UNITY_EDITOR // Introduced in 2019.3. Also can cause problems in builds so only for editor.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init()
        {
            playerNumber = new Dictionary<int, ActionCharacter>();
            npcwrapper = new Dictionary<ActionCharacter, InputNPCWrapperSO>();
           
            // Initialize your stuff here.
        }
#endif

        public static event Action<PlayerInput> OnPlayerAdded;
        public static event Action<PlayerInput> OnPlayerRemoved;
        public static event Action<PlayerInput> OnNewAvatar;
        static Dictionary<int, ActionCharacter> playerNumber = new Dictionary<int, ActionCharacter>();
        static Dictionary<ActionCharacter, InputNPCWrapperSO> npcwrapper = new Dictionary<ActionCharacter, InputNPCWrapperSO>();

        static void SceneUnloaded(Scene scene)
        {
            playerNumber.Clear();
            npcwrapper.Clear();
        }
        public static void MakeNPCCOntrolled(ActionCharacter avatar, CharacterInputs inputs)
        {
            if (avatar == null)
            {
                Debug.Log("Null avatar");
                return;
            }
            INPCControl npccontrols = avatar.gameObject.GetComponent<INPCControl>();
            if (npccontrols == null)
            {
                npccontrols = avatar.gameObject.AddComponent<NPCBlackboard>();
            }
           
            ActionManager.ResetBufferMap(avatar);

            if (npcwrapper.ContainsKey(avatar) == false)
            {
                npcwrapper[avatar] = ScriptableObject.CreateInstance<InputNPCWrapperSO>();
            }

            if (ActionManager.InActionSequence(avatar.ID))
            {
                ActionManager.EndAction(avatar, ActionManager.GetActionTicker(avatar.ID));
            }

            InputWrapperSO wrapperso = npcwrapper[avatar];
            InputNPCWrapper wrapper = wrapperso.GetWrapper() as InputNPCWrapper;
            wrapper.SetNPC(npccontrols);
            npccontrols.SetActionSlotsTemplate(inputs.ActionInputs);
            npccontrols.FireNPCControl();
            avatar.SetNewInputWrapper(wrapperso);

  
 
            

        }
        public static bool IsPlayerControlled(ActionCharacter avatar)
        {
            return playerNumber.ContainsValue(avatar);
        }
        public static void MakeNewPlayerAvatar(CharacterInputs inputs, ActionCharacter newAvatar, int player)
        {
            if (newAvatar == null)
            {
                Debug.Log("Null avatar");
                return;
            }

            if (playerNumber.ContainsKey(player))
            {
                if (newAvatar.ID == playerNumber[player].ID)
                {
                    Debug.Log("Avatar already controlled by player " + player);
                    return;
                }
            }
            else
            {
                Debug.Log("Player number " + playerNumber + " has not been registered.");
                return;
            }

            
            ActionCharacter current = playerNumber[player];
            MakeNPCCOntrolled(current, inputs);
           

            if (npcwrapper.ContainsKey(newAvatar))
            {
                InputWrapperSO wrapperso = npcwrapper[newAvatar];
                InputNPCWrapper wrapper = wrapperso.GetWrapper() as InputNPCWrapper;
                wrapper.SetNPC(null);
                ScriptableObject.Destroy(wrapperso);
                npcwrapper.Remove(newAvatar);
            }

            INPCControl newone = newAvatar.gameObject.GetComponent<INPCControl>();
            if (newone == null)
            {
                newone = newAvatar.gameObject.AddComponent<NPCBlackboard>();
            }
            newone.FirePlayerControl();

            ActionManager.ResetBufferMap(newAvatar);
            if (ActionManager.InActionSequence(newAvatar.ID))
            {
                ActionManager.EndAction(newAvatar, ActionManager.GetActionTicker(newAvatar.ID));
            }
            newAvatar.InputRequirements.SetInputs(inputs.ActionInputs);//inputs must go before wrapper
            newAvatar.SetLockonInput(inputs.LockonInput);
            newAvatar.SetStrafeInput(inputs.StrafeInput);
            newAvatar.SetNewInputWrapper(inputs.InputWrapper);

            playerNumber[player] = newAvatar;
            OnNewAvatar?.Invoke(new PlayerInput(player, newAvatar));

        }
        public static IReadOnlyDictionary<int, ActionCharacter> GetPlayers()
        {
            return playerNumber;
        }
        public static bool HasPlayer(int number)
        {
            return playerNumber.ContainsKey(number);
        }
        public static ActionCharacter GetPlayer(int number)
        {
            if (playerNumber.ContainsKey(number))
            {
                return playerNumber[number];
            }
            return null;
        }
        public static void AddPlayer(int number, ActionCharacter character)
        {
           
         

            if (character != null)
            {
                if (playerNumber.ContainsKey(number))
                {
                    Debug.Log("Player number " + number + " already occupied");
                    int failsafe = 12;
                    while (playerNumber.ContainsKey(number))
                    {
                        failsafe++;
                        number++;
                        if (number > failsafe)
                        {
                            Debug.Log("Somehow there are over " + failsafe + " players registered. No more allowed");
                            return;
                        }
                    }
                }

                if (ActionManager.InActionSequence(character.ID))
                {
                    ActionManager.EndAction(character, ActionManager.GetActionTicker(character.ID));
                }
                playerNumber[number] = character;
                character.SetPlayerNumber(number);
                character.SetPlayerControlled(true);
                OnPlayerAdded?.Invoke(new PlayerInput(number, character));
            }
       
        }
        public static void RemovePlayer(int number)
        {
            if (playerNumber.ContainsKey(number))
            {

                ActionCharacter character = playerNumber[number];
                if (ActionManager.InActionSequence(character.ID))
                {
                    ActionManager.EndAction(character, ActionManager.GetActionTicker(character.ID));
                }
                character.SetPlayerControlled(false);
                OnPlayerRemoved?.Invoke(new PlayerInput(number, playerNumber[number]));
                playerNumber.Remove(number);
               
            }
        }
    }
}