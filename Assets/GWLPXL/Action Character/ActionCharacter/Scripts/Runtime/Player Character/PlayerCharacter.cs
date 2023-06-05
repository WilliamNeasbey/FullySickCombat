using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GWLPXL.ActionCharacter
{

    /// <summary>
    /// states for playerinstance
    /// </summary>
    [System.Serializable]
    public class PlayerStates
    {
        public PlayerCharacterCCLoco CCLoco;

    }

    [System.Serializable]
    public class Strafe
    {
        public InputActionSlotSO Input = null;
    }
    [System.Serializable]
    public class LockOn
    {
        public LockStateVars Vars = new LockStateVars();
        public InputActionSlotSO Input = null;
    }
    /// <summary>
    /// abstract base class for player characters, override to extend
    /// </summary>
    public abstract class PlayerCharacter : ActionCharacter
    {

      


    }
}