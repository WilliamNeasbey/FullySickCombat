


using System.Collections.Generic;

namespace GWLPXL.ActionCharacter
{
    /// <summary>
    ///base Character action sequence
    /// </summary>
    [System.Serializable]
    public class CharacterActionSequence
    {
        public List<ActionCCVars> MovementSequence = new List<ActionCCVars>(1) { new ActionCCVars() };
    }
}

