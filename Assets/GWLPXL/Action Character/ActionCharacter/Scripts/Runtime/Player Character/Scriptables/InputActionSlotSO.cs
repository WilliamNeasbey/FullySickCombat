using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{

    [CreateAssetMenu(fileName = "New Input Action Slot Key", menuName = "GWLPXL/ActionCharacter/Input/Input Action Slot Key", order = 330)]
    public class InputActionSlotSO : InputActionSlotKey
    {
        public int Key { get; set; }
        public InputRequirements InputRequirements = new InputRequirements();
    }


    public abstract class InputActionSlotKey : ScriptableObject
    {

    }
}