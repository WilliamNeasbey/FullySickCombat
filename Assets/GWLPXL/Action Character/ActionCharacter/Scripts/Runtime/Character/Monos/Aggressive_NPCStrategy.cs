using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{
 
    public class Aggressive_NPCStrategy : MonoBehaviour
    {
        public SearchType SearchType = SearchType.Closest;
        public bool AutoAcquire;
        public GameObject Target;
        public float AttackRate;
        public float AttackDistance;
        public float LockOnDistance;
        public float LockOnRate;
        public bool Moving;
        public List<int> EnemyTeams = new List<int>();
        public int[] AttackSequence = new int[1] { 0 };
        protected int index;
        protected INPCControl control;
        protected float timer2 = 0;
        protected float timer = 0;
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
            timer2 += Time.deltaTime;

            if (timer2 >= LockOnRate)
            {
                timer2 = 0;
                if (AutoAcquire)
                {
                    Target = HitBoxTeamManager.FindNearest(EnemyTeams[0], transform.position);
                }

                if (Vector3.Distance(Target.transform.position, this.transform.position) < LockOnDistance)
                {
                    Vector3 dir = Target.transform.position - this.transform.position;
                    control.SetTarget(Target.transform);
                    control.SetInput(dir.x, dir.z);
                }
                else
                {
                    control.SetInput(0, 0);
                    control.SetTarget(null);
                }
                Moving = Target != null;
            }
            
            if (Moving)
            {
                if (Vector3.Distance(Target.transform.position, this.transform.position) < LockOnDistance)
                {
                    Vector3 dir = Target.transform.position - this.transform.position;
                    control.SetTarget(Target.transform);
                    control.SetInput(dir.x, dir.z);
                }
                else
                {
                    control.SetInput(0, 0);
                    control.SetTarget(null);
                }
            }

            if (timer >= AttackRate)
            {
                if (Target == null) return;

                if (Vector3.Distance(Target.transform.position, this.transform.position) < AttackDistance)
                {
                    timer = 0;
                    Moving = false;
                    control.SetInput(0, 0);
                    control.TriggerActionSlot(AttackSequence[index]);
                    index++;
                    if (index > AttackSequence.Length - 1)
                    {
                        index = 0;
                    }
                }
            }
           
            
        }
    }
}
