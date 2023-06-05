using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{
    public class StrafeTrigger : MonoBehaviour
    {

        protected virtual void OnTriggerEnter(Collider other)
        {
            ActionCharacter action = other.GetComponent<ActionCharacter>();

            if (action != null)
            {
                Debug.Log(action);
                action.SetRotateType(RotateType.Strafe);
            }
        }


        protected virtual void OnTriggerExit(Collider other)
        {
            ActionCharacter action = other.GetComponent<ActionCharacter>();
            if (action != null)
            {
                action.SetRotateType(RotateType.Free);
            }
        }
    }
}
