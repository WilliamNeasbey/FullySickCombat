using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{


    public class AutoEnable : MonoBehaviour
    {
        [SerializeField]
        protected List<GameObject> toEnable = new List<GameObject>();
        // Start is called before the first frame update
        void Start()
        {
            for (int i = 0; i < toEnable.Count; i++)
            {
                toEnable[i].SetActive(true);
            }
        }


    }
}