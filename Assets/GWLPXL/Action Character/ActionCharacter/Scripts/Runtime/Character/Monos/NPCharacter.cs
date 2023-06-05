

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{


    public abstract class NPCharacter : ActionCharacter
    {
       public string Action { get; set; }

        public override void CheckInputs()
        {
            if (string.IsNullOrWhiteSpace(Action) == false)
            {
                bool success = TryStartActionSequence(Action);
                if (success) Action = string.Empty;
            }


        }
       


    }
}