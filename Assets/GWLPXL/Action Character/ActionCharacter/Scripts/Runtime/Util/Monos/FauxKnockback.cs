using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace GWLPXL.ActionCharacter
{

    [System.Serializable]
    public class SimpleKnockbackVars
    {

        [Tooltip("None converts Dir to world, Local converts Dir to local")]

        public KnockBackDirectionType DirectionType = KnockBackDirectionType.AttackerLocalDirection;

        public Vector3 Dir;
        public float Force;
        public float Upforce;

    }

    /// <summary>
    /// example class that extends hit reactions to perform a knockback, not intended to be production quality
    /// </summary>
    public class FauxKnockback : HitTakeDamage
    {
        public SimpleKnockbackVars Knockback;
        public event Action OnDoKill;
        Rigidbody rb;

        protected override void Awake()
        {
            base.Awake();
            rb = GetComponent<Rigidbody>();
        }

        protected override void HurtCollision(HitContextCollision contextCollision)
        {
            base.HurtCollision(contextCollision);

            Debug.Log(string.Concat("Collision " + contextCollision.AttackingPartName));

            Vector3 dir = GetDirection(this.transform, contextCollision, Knockback);
            rb.AddForce(dir * Knockback.Force + Vector3.up * Knockback.Upforce, ForceMode.Impulse);

            int kill = UnityEngine.Random.Range(-1, 2);
            if (kill == 0)
            {
                OnDoKill?.Invoke();
            }
   
        }


        protected override void HurtTrigger(HitContextTrigger contextCollision)
        {
            base.HurtTrigger(contextCollision);

            Debug.Log(string.Concat("Trigger " + contextCollision.AttackingPartName));

            Vector3 dir = GetDirection(this.transform, contextCollision, Knockback);
            rb.AddForce(dir * Knockback.Force + Vector3.up * Knockback.Upforce, ForceMode.Impulse);

            int kill = UnityEngine.Random.Range(-1, 2);
            if (kill == 0)
            {
                OnDoKill?.Invoke();
            }
        }

        protected Vector3 GetDirection(Transform self, HitContextTrigger other, SimpleKnockbackVars cv)
        {
            Vector3 dir = new Vector3(0, 0, 0);
            switch (cv.DirectionType)
            {
                case KnockBackDirectionType.None:
                    dir = cv.Dir;
                    break;
                case KnockBackDirectionType.AttackerLocalDirection:
                    dir = other.Attacker.transform.TransformDirection(cv.Dir);
                    break;
                case KnockBackDirectionType.HitDirection:

                    if (other.Attacker != null)
                    {
                        dir = self.position - other.Attacker.transform.position;
                    }
           
                    break;
                case KnockBackDirectionType.HitNormal:
                    dir = self.position - other.Attacker.transform.position;
                    break;

            }

            return dir;
        }
        protected Vector3 GetDirection(Transform self, HitContextCollision other, SimpleKnockbackVars cv)
        {
            Vector3 dir = new Vector3(0, 0, 0);
            switch (cv.DirectionType)
            {
                case KnockBackDirectionType.None:
                    dir = cv.Dir;
                    break;
                case KnockBackDirectionType.AttackerLocalDirection:
                    dir = other.Attacker.transform.TransformDirection(cv.Dir);
                    break;
                case KnockBackDirectionType.HitDirection:
                    dir = self.position - other.Attacker.transform.position;
                    break;
                case KnockBackDirectionType.HitNormal:
                    dir = -other.Collision.GetContact(0).normal;
                    break;

            }

            return dir;
        }
    }
}