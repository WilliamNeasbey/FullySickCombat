

using UnityEngine;

namespace GWLPXL.ActionCharacter
{
    [System.Serializable]
    public enum RotateType
    {
        None = 0,
        Free = 100,
        Strafe = 101,
        Locked = 102

    }
    /// <summary>
    /// animator rotate and input vars for the character CC
    /// </summary>
    [System.Serializable]
    public class CharacterRotate
    {

        public InputRotateVars Rotation = new InputRotateVars(0, 0, 0, 5, RotateSmoothType.Slerp, InputReference.Camera);

    }


    [System.Serializable]
    public class InputRotateVars
    {
        public float Multiplier = 1;
        public float Speed = 15;
        public float BufferedKeepTime = .25f;

        public RotateSmoothType Smooth = RotateSmoothType.Slerp;
        public InputReference Reference = InputReference.Camera;
        public RotateType RotateType = RotateType.Free;
        public Vector3 TranslatedRotate { get; set; }
        public float X { get; set; }
        public float Z { get; set; }
        public float BufferedX { get; set; }
        public float BufferedZ { get; set; }
        public Quaternion FaceDirection { get; set; }


        public InputRotateVars(float x, float y, float z, float speed, RotateSmoothType smooth = RotateSmoothType.Slerp, InputReference reference = InputReference.Camera, float multiplier = 1)
        {
            X = x;
            Z = z;
            Speed = speed;
            Smooth = smooth;
            Reference = reference;
            Multiplier = multiplier;
        }
    }
}

