using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{
    [RequireComponent(typeof(ActorHitBoxes))]
    public abstract class Environment : MonoBehaviour, IHaveHitBoxes
    {
        protected List<string> givers = new List<string>();
        protected List<string> takers = new List<string>();
        protected ActorHitBoxes boxes = null;

        protected virtual void Awake()
        {
            boxes = GetComponent<ActorHitBoxes>();
            boxes.Setup(null);
        }

        protected virtual void Start()
        {

        }

        protected virtual void OnEnable()
        {


        }

        protected   virtual void OnDisable()
        {

        }
        
        public List<string> GetHitGivers()
        {
            return givers;
        }

        public List<string> GetHitTakers()
        {
            return takers;
        }

        public void SetHitGivers(List<string> givers)
        {
            this.givers = givers;
        }

        public void SetHitTakers(List<string> takers)
        {
            this.takers = takers;
        }

       
    }
}
