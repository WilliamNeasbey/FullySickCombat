using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{

    /// <summary>
    /// abstract mono for subbing to hit givers enabled/disabled
    /// </summary>
    public abstract class HitBoxHitGiverSub : MonoBehaviour
    {
        [SerializeField]
        protected ActorHitBoxes boxes;

       
        protected virtual void Awake()
        {
            if (boxes == null)
            {
                boxes = GetComponent<ActorHitBoxes>();
            }
   

        }

        protected virtual void OnEnable()
        {
            SubManager.SubHitGiverEnabled(boxes, HitGiverEnabled);
            SubManager.SubHitGiverDisabled(boxes, HitGiverDisabled);

        }

        protected virtual void OnDisable()
        {
            SubManager.UnSubHitGiverEnabled(boxes, HitGiverEnabled);
            SubManager.UnSubHitGiverDisabled(boxes, HitGiverDisabled);
        }

        protected virtual void HitGiverEnabled(HitBoxArgs area)
        {
          
        }

        protected virtual void HitGiverDisabled(HitBoxArgs area)
        {
           

        }

      
    }


}