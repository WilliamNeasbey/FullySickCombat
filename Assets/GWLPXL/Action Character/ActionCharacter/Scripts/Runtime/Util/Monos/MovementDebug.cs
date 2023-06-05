
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{


    public class MovementDebug : MonoBehaviour
    {
        public MovementSO So;
        public Color MoveD = Color.black;
        public float LineLength = 1;

        private void OnDrawGizmos()
        {
            if (So == null) return;
            Vector3 moveD = transform.position + So.Movement.Standard.Locomotion.Locomotion.Movement.TranslatedMoveDirection;
            Gizmos.color = MoveD;
            Gizmos.DrawSphere(moveD, .1f);
            Gizmos.DrawLine(transform.position, transform.position + (So.Movement.Standard.Locomotion.Locomotion.Movement.TranslatedMoveDirection * LineLength));
        }
    }
}