using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{

    /// <summary>
    /// example for lock on behavior, not production ready script.
    /// </summary>
    public class FauxLockOn : MonoBehaviour
    {
        public Transform Target;
        protected ActionCharacter ac;

        protected  void Awake()
        { 
            ac = GetComponent<ActionCharacter>();
        }
      
        // Update is called once per frame
        void Update()
        {
            ac.SetLockOnTarget(Target);
        }
    }
}