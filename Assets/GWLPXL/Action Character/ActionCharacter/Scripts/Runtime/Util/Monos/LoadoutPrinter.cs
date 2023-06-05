using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{
    /// <summary>
    /// prints combos to text.
    /// </summary>
    public class LoadoutPrinter : MonoBehaviour
    {
        public ActionCharacter Character;
        public int PlayerNumber = 0;
        public UnityEngine.UI.Text Text;
        protected StringBuilder sb = new StringBuilder();

        protected virtual void OnEnable()
        {
            PlayerCharacterManager.OnNewAvatar += AssignAvatar;
            PlayerCharacterManager.OnPlayerAdded += AssignAvatar;
            ActionManager.OnCombosUpdated += CombosUpdated;
        }
        protected virtual void OnDisable()
        {
            PlayerCharacterManager.OnNewAvatar -= AssignAvatar;
            PlayerCharacterManager.OnPlayerAdded -= AssignAvatar;
            ActionManager.OnCombosUpdated -= CombosUpdated;
        }
        protected virtual void AssignAvatar(PlayerInput input)
        {
            if (input.PlayerNumber == PlayerNumber)
            {
                Character = input.Character;
                CombosUpdated(new CombosUpdated(Character));
            }
           
        }
        protected virtual void CombosUpdated(CombosUpdated character)
        {
            if (Character == null)
            {
                Debug.Log("Add a character to the loadout printer to see combos", this);
                return;
            }
            sb.Clear();
            List<Combos> combos = Character.GetMyCombos();
            if (combos.Count > 0)
            {
                PrintToText(combos);
            }
            else
            {
                sb.Append("No Combos Equipped");
                Text.text = sb.ToString();
            }
        }

        protected virtual void PrintToText(List<Combos> combos)
        {
            for (int i = 0; i < combos.Count; i++)
            {
                sb.Append(combos[i].Name);
                sb.Append('\n');
                sb.Append("     ");

                List<int> inputs = new List<int>();
                for (int j = 0; j < combos[i].Pieces.Count; j++)
                {
                    inputs.Add(combos[i].Pieces[j].InputSlotIndex);
                }

                InputActionMapSO map = Character.InputRequirements;
                if (map != null)
                {
                    for (int j = 0; j < inputs.Count; j++)
                    {
                        int index = inputs[j];
                        if (index <= map.InputSlots.Count - 1)
                        {
                            InputSlot slot = map.InputSlots[index];
                            InputActionSlotSO so = slot.Requirements;
                            if (so != null)
                            {
                                sb.Append(so.name);

                            }
                            if (j < inputs.Count - 1)
                            {
                                sb.Append(" + ");
                            }
                        }
                        else
                        {
                            sb.Append("INVALID INPUT");
                        }

                    }
                }
                else
                {
                    for (int j = 0; j < inputs.Count; j++)
                    {
                        sb.Append(inputs[j]);
                    }
                }

                sb.Append('\n');
            }

            Text.text = sb.ToString();
        }
    }
}
