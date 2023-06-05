using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{
    [System.Serializable]
    public class CharacterInputs
    {
        public List<InputActionSlotSO> ActionInputs = new List<InputActionSlotSO>();
        public InputWrapperSO InputWrapper = null;
        public InputActionSlotSO LockonInput = null;
        public InputActionSlotSO StrafeInput = null;
    }
    [System.Serializable]
    public class PartyController
    {
        public CharacterInputs Inputs = new CharacterInputs();
        public ActionCharacter Avatar;
        public int PlayerNumber { get; set; }
        public int Index { get; set; }
    }




    /// <summary>
    /// allows to set party members and switch between them.
    /// </summary>
    public class PlayerPartyController : MonoBehaviour
    {
        [Header("Player Controllers")]
        [Tooltip("Player Controlled. Controllers and their avatars.")]
        public List<PartyController> Controllers = new List<PartyController>();
        [Header("All Party Members")]
        [Tooltip("All party members. Any action characters that the player(s) can switch to.")]
        public List<ActionCharacter> PlayerPartyMembers = new List<ActionCharacter>();
        [Tooltip("Test key for toggling between")]
        public KeyCode TestKey = KeyCode.F2;
        [Tooltip("Allow this to control player input.")]
        public bool IniOnStart = true;
        // Start is called before the first frame update

        protected virtual void Awake()
        {
            
        }

        protected virtual void OnDestroy()
        {
            for (int i = 0; i < Controllers.Count; i++)
            {
                PlayerCharacterManager.RemovePlayer(Controllers[i].PlayerNumber);
            }
        }
        protected virtual void Start()
        {
            for (int i = 0; i < Controllers.Count; i++)
            {
                Controllers[i].PlayerNumber = i;
                PlayerCharacterManager.AddPlayer(Controllers[i].PlayerNumber, Controllers[i].Avatar);
            }
            if (IniOnStart)
            {
                for (int i = 0; i < Controllers.Count; i++)
                {
                    PlayerCharacterManager.MakeNewPlayerAvatar(Controllers[i].Inputs, Controllers[i].Avatar, Controllers[i].PlayerNumber);
                }

                for (int i = 0; i < PlayerPartyMembers.Count; i++)
                {
                    bool playercontrolled = PlayerPartyMembers[i].IsPlayedControlled;
                    if (playercontrolled == false)
                    {
                        PlayerCharacterManager.MakeNPCCOntrolled(PlayerPartyMembers[i], Controllers[0].Inputs);
                    }
                }

               

            }

        }


        private void Update()
        {
            if (Input.GetKeyDown(TestKey))
            {
                FindNextMember(Controllers[0]);//this example only assumes 1 player, but will work for multiple with different inputs for each player controlled
            }
        }

        public void FindNextMember(int playerNumber)
        {
            for (int i = 0; i < Controllers.Count; i++)
            {
                if (playerNumber == Controllers[i].PlayerNumber)
                {
                    FindNextMember(Controllers[i]);
                    break;
                }
            }
        }
        public void FindNextMember(PartyController controller)
        {
            controller.Index++;
            if (controller.Index > PlayerPartyMembers.Count - 1)
            {
                controller.Index = 0;
            }
            int failed = 0;
            bool fail = false;

            if (PlayerPartyMembers[controller.Index].DamageController != null)//ugly, refactor later
            {
                while (PlayerPartyMembers[controller.Index].IsPlayedControlled || PlayerPartyMembers[controller.Index].DamageController.IsDead)
                {
                    controller.Index++;
                    if (controller.Index > PlayerPartyMembers.Count - 1)
                    {
                        controller.Index = 0;
                    }
                    failed++;
                    if (failed > PlayerPartyMembers.Count)
                    {
                        Debug.Log("Failed to find a free party member");
                        fail = true;
                        break;
                    }
                }
            }
            else
            {
                while (PlayerPartyMembers[controller.Index].IsPlayedControlled)
                {
                    controller.Index++;
                    if (controller.Index > PlayerPartyMembers.Count - 1)
                    {
                        controller.Index = 0;
                    }
                    failed++;
                    if (failed > PlayerPartyMembers.Count)
                    {
                        Debug.Log("Failed to find a free party member");
                        fail = true;
                        break;
                    }
                }
            }
            
           
            if (fail == false)
            {
                PlayerCharacterManager.MakeNewPlayerAvatar(controller.Inputs, PlayerPartyMembers[controller.Index], controller.PlayerNumber);
                controller.Avatar = PlayerPartyMembers[controller.Index];
            }

        }
    }
}
