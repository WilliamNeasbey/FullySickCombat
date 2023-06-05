using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{

    public class AliveTime : MonoBehaviour
    {
        public float AliveDuration = 1;
        Timer timer;
        // Start is called before the first frame update
        void Start()
        {
            timer = new Timer(AliveDuration, DestroyMe);
        }

        void DestroyMe()
        {
            timer.RemoveTicker();
            timer = null;
            Destroy(this.gameObject);
        }
    }
}
