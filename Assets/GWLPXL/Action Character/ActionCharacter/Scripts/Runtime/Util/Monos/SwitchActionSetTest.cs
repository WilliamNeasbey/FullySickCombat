using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{

    /// <summary>
    /// deprecate, just test
    /// </summary>
    public class SwitchActionSetTest : MonoBehaviour
    {
        public PlayerCharacter Player;
        public List<CharacterActionLoadoutSO> SetsToCycle = new List<CharacterActionLoadoutSO>();
        public KeyCode CycleKey = KeyCode.Alpha1;
        protected int cycle = 0;
        

        // Update is called once per frame
        void Update()
        {
            if (UnityEngine.Input.GetKeyDown(CycleKey))
            {
                cycle++;
                if (cycle > SetsToCycle.Count - 1) cycle = 0;
                Player.AddActionSet(SetsToCycle[cycle].ActionSets);

            }
        }
    }
}