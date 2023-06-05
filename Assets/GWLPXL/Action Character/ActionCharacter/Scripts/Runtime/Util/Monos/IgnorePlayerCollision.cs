using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{


    public class IgnorePlayerCollision : MonoBehaviour
    {
   
        
        protected virtual void Start()
        {
            PlayerPartyController party = FindObjectOfType<PlayerPartyController>();
            if (party != null)
            {
                for (int i = 0; i < party.PlayerPartyMembers.Count; i++)
                {
                    Physics.IgnoreCollision(GetComponent<Collider>(), party.PlayerPartyMembers[i].GetComponent<Collider>(), true);
                }
            }
        }

    }
}