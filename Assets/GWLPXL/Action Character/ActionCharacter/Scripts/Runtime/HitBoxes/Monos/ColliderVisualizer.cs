using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GWLPXL.ActionCharacter
{


    public class ColliderVisualizer : MonoBehaviour
    {
        public Collider Collider;
        public bool Debug = true;
        private void OnDrawGizmos()
        {
            if (Collider.enabled && Debug)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(transform.position, 1f);
            }
            
        }

    }
}