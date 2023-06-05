
using UnityEngine;

using System.Collections.Generic;

namespace GWLPXL.ActionCharacter
{

    /// <summary>
    /// requirements to perform an action
    /// </summary>
    [System.Serializable]
    public class CharacterRequirements
    {
        public List<FreeFormState> RequiredStates = new List<FreeFormState>(1) { FreeFormState.Ground };
        public bool RequiresTarget = false;

       

    }
}

