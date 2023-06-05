using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{
    public class Follow_NPCStrategy : MonoBehaviour
    {
        public Transform FollowTarget;
        public Vector2 MinMaxMove = new Vector2(3, 5);
        public float MoveRate;
        public bool Moving;
        protected float timer = 0;
        protected INPCControl control;

        private void Awake()
        {

            control = GetComponent<INPCControl>();
        }
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            timer += Time.deltaTime;
            if (timer >= MoveRate)
            {
                timer = 0;
                if (Vector3.Distance(FollowTarget.transform.position, this.transform.position) > MinMaxMove.y)
                {
                    Vector3 dir = FollowTarget.transform.position - this.transform.position;
                    dir.y = 0;
                    dir.Normalize();
                    control.SetInput(dir.x, dir.z);
                    Moving = true;
                }
               
            }

            if (Moving)
            {
                if (Vector3.Distance(FollowTarget.transform.position, this.transform.position) > MinMaxMove.y)
                {
                    Vector3 dir = FollowTarget.transform.position - this.transform.position;
                    dir.y = 0;
                    dir.Normalize();
                    control.SetInput(dir.x, dir.z);
                    Moving = true;
                }
                if (Vector3.Distance(FollowTarget.transform.position, this.transform.position) < MinMaxMove.x)
                {
                    control.SetInput(0, 0);
                    Moving = false;
                }
          
  
            }
            
            
        }
    }
}
