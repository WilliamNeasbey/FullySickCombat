using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{
    public class FauxForceAction : MonoBehaviour
    {
        public string Action;
        public KeyCode Key;
        

        // Update is called once per frame
        void Update()
        {
        if (Input.GetKeyDown(Key))
            {
                GetComponent<ActionCharacter>().ForceAction(Action);
                //ActionManager.TryStartAction(GetComponent<ActionCharacter>(), Action, true);
            }
        }
    }
}
