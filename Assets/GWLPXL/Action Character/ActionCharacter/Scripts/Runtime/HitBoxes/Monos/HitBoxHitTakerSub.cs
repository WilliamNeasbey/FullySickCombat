using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{
    public abstract class HitBoxHitTakerSub : MonoBehaviour
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
            SubManager.SubHitTakerEnabled(boxes, HitTakerEnabled);
            SubManager.SubHitTakerDisabled(boxes, HitTakerDisabled);

        }

        protected virtual void OnDisable()
        {
            SubManager.UnSubHitTakerEnabled(boxes, HitTakerEnabled);
            SubManager.UnSubHitTakerDisabled(boxes, HitTakerDisabled);
        }

        protected virtual void HitTakerEnabled(HitBoxArgs area)
        {

        }

        protected virtual void HitTakerDisabled(HitBoxArgs area)
        {


        }
    }
}
