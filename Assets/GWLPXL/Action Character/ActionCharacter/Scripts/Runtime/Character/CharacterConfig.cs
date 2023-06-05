using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GWLPXL.ActionCharacter
{
    [System.Serializable]
    public class Defaults
    {
        public RotateType RotateType = RotateType.Free;
        [Header("Multipliers")]
        public float MoveMulti = 1;
        public float AirborneMulti = 1;
        public float RotateMulti = 1;
        public float FallMulti = 1;
        public float JumpMulti = 1;
        [Header("Animator")]
        public bool UseRoot = false;
        public float AnimatorSpeed = 1;
        public string Locomotion = "Locomotion";
        public string Airborne = "Airborne";
        public float AirborneTrans = .25f;
        public float LocomotionTrans = .25f;
        [Tooltip("Used to speed up or slow down actions")]
        public string AnimSpeedParam = "AnimMulti";
        [Tooltip("Linear speed. Used for when not using 8dir movement with Vel X and Z")]
        public string Speed = "Speed";
        public string IsGrounded = "IsGrounded";
        public string VelocityZ = "VelocityZ";
        public string VelocityX = "VelocityX";
        public string Idling = "Idling";
        public string DeadKey = "Death";

    }

    /// <summary>
    /// data container for character configuration, used as a way to share the character data across systems
    /// </summary>

    [CreateAssetMenu(fileName = "New Config", menuName = "GWLPXL/ActionCharacter/Character/Config", order = 110)]
    public class CharacterConfig : ScriptableObject
    {
        public Defaults Defaults = new Defaults();
        public List<string> Actions = new List<string>();
        public List<string> HitGiverBoxes = new List<string>();
        public List<string> HitTakerBoxes = new List<string>();
        public List<string> ParentPoints = new List<string>();


    }
}