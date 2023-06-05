
using UnityEngine;

namespace GWLPXL.ActionCharacter
{
 
    /// <summary>
    /// base class for the character locomotion CC
    /// </summary>
    [System.Serializable]
    public class CharacterLocomotion
    {
        public InputMoveVars Movement = new InputMoveVars(0, 0, 5, InputReference.Camera);

    }

    [System.Serializable]
    public class InputMoveVars
    {
        public float Multiplier = 1;
        public float AirborneMulti = 1;
        public float Speed = 5;
        public float BufferedKeepTime = .25f;
        public float SlopeLimit = 45;

        public InputReference Reference = InputReference.Camera;

        public float X { get; set; }
        public float Z { get; set; }
        public float BufferedX { get; set; }
        public float BufferedZ { get; set; }
        public Vector3 TranslatedMoveDirection { get; set; }
        public Vector3 BufferedTranslatedMoveDirection { get; set; }
        public Vector3 BufferedMoveDir { get; set; }
        public Vector3 LocalInput { get; set; }
        public Vector3 GlobalInput { get; set; }
        public Vector3 CameraInput { get; set; }
        public Vector3 GroundPlane { get; set; }

        public InputMoveVars(float x, float z, float speed, InputReference reference = InputReference.Camera, float muli = 1)
        {
            X = x;
            Z = z;
            Speed = speed;
            Reference = reference;
            Multiplier = muli;
        }
    }

}

