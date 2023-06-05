using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{
    [RequireComponent(typeof(AudioSource))]
    //might not need anymore
    public class AudioReturn : ReturnToPool
    {
        protected AudioSource audiosource;


        protected override void Awake()
        {
            audiosource = GetComponent<AudioSource>();
        }

        


    }
}
