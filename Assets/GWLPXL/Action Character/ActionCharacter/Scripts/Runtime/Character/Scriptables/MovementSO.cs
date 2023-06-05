using System.Collections;
using System.Collections.Generic;
using UnityEngine;




namespace GWLPXL.ActionCharacter
{

    /// <summary>
    /// Movement Wrapper
    /// </summary>
    /// 
    [CreateAssetMenu(fileName = "New Movement", menuName = "GWLPXL/ActionCharacter/Character/Movement", order = 100)]

    public class MovementSO : ScriptableObject
    {
        public Movement Movement = new Movement();
    }


    /// <summary>
    /// base movement
    /// </summary>
    [System.Serializable]
    public class Movement
    {

        public StandardMovements Standard = new StandardMovements();
    }

    /// <summary>
    /// base standard movements
    /// </summary>
    [System.Serializable]
    public class StandardMovements
    {
        public StandardLocomotion Locomotion = new StandardLocomotion();


    }

    /// <summary>
    /// base locomotion
    /// </summary>
    [System.Serializable]
    public class StandardLocomotion
    {
        public CharacterRotate Rotate = new CharacterRotate();
        public CharacterLocomotion Locomotion = new CharacterLocomotion();
        public CharacterFalling Fall = new CharacterFalling();

    }


}

