using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{
    public class FauxChangeRotateType : MonoBehaviour
    {
        public RotateType Type;

        protected ActionCharacter ac;

        private void Awake()
        {
            ac = GetComponent<ActionCharacter>();
        }
      

        // Update is called once per frame
        void Update()
        {
            ac.SetRotateType(Type);
        }
    }
}
