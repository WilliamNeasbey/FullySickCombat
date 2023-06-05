using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace GWLPXL.ActionCharacter
{

    /// <summary>
    /// testing, place on object that gets hit
    /// </summary>
    public class FreezeFrameOnHit : MonoBehaviour
    {
        public float hitrate = .25f;
        public float FreezeRate = .2f;
        bool iframe = true;
        // Start is called before the first frame update
        void Start()
        {
            ActorHitBoxes boxes = GetComponent<ActorHitBoxes>();
            boxes.HitBoxes.HitTakers.HitReceiver.HitTakerEvents.OnHitCollision += CollisionEvent;
            iframe = false;
        }

        void CollisionEvent(HitContextCollision collision)
        {
            if (Time.timeScale == 1 && iframe == false)
            {
                Debug.Log(collision.AttackingPartName);
                Time.timeScale = 0;
                StartCoroutine(ResetTime());

                StartCoroutine(HitRate());
                iframe = true;
            }

        }

        IEnumerator HitRate()
        {
            iframe = true;
            yield return new WaitForSecondsRealtime(hitrate);
            iframe = false;
        }
        IEnumerator ResetTime()
        {
            yield return new WaitForSecondsRealtime(FreezeRate);
            Time.timeScale = 1;

        }


    }
}