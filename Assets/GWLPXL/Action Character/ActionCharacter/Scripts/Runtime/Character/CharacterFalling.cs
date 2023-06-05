
using UnityEngine;

namespace GWLPXL.ActionCharacter
{
    /// <summary>
    /// base class for the character falling
    /// </summary>
    [System.Serializable]
    public class CharacterFalling
    {
        public FallingVars Falling = new FallingVars(Vector3.down, 0, 25, 12, null);
    }



    [System.Serializable]
    public class FallingVars
    {
        public float Multiplier = 1;
        public float FallingSpeed = 12;
        public int MaxAirborneActions = 1;
        public AnimationCurve FallingCurve = null;
        public LayerMask GroundLayer;
        public float TimeToMaxFallSpeed = 12;
        public Vector3 GravityDirection = new Vector3(0, -1, 0);
        [Tooltip("Buffer between choosing isgrounded and !isgrounded.")]
        public float BufferTimer = .02f;
        [Tooltip("Time Coyote state is allowed after going airborne.")]
        public float CoyoteDuration = .5f;
        public bool CoyoteAllowed { get; set; }
        public bool IsGrounded { get; set; }
        public float CurrentFallingSpeed { get; set; }

        public FallingVars(Vector3 gravitydir, LayerMask ground, float fallingspeed, float timetomaxspeed, AnimationCurve curve, float multi = 1)
        {
            GroundLayer = ground;
            GravityDirection = gravitydir;
            FallingSpeed = fallingspeed;
            TimeToMaxFallSpeed = timetomaxspeed;
            FallingCurve = curve;

            Multiplier = multi;
        }

    }
}

