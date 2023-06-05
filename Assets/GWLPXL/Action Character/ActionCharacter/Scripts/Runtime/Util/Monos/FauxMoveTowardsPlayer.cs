using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace GWLPXL.ActionCharacter
{

    /// <summary>
    /// example of a movement controller, not production quality.
    /// </summary>
    public class FauxMoveTowardsPlayer : MonoBehaviour, IHaveHitBoxes
    {
        public event Action<FauxMoveTowardsPlayer> OnDeathComplete;

        public bool Move = true;
        public float MoveForce;
        public float MaxSpeed;
        public float Radius;
        public LayerMask Ground;
        public int TeamTarget = 0;
        public float GroundCheckRadius = 1;
        public bool AutoTarget = true;
        public float CheckTargetRate = 10;
        public Transform OverrideTarget = null;
        public GameObject DeathEffectPrefab = null;
        public Vector2 DespawnMinMax = new Vector2(3, 5);
        public List<string> HitGivers = new List<string>();
        public List<string> HitTakers = new List<string>();
        bool grounded;
        float targettimer = 0;
        Rigidbody rb;
        [SerializeField]
        Transform target;
        Transform self;
        FauxKnockback knockback;
        Color original;
        private void Awake()
        {
            if (DeathEffectPrefab != null)
            {
                SimplePool.Preload(DeathEffectPrefab);
            }
            knockback = GetComponent<FauxKnockback>();
            self = GetComponent<Transform>();
            rb = GetComponent<Rigidbody>();
            original = GetComponentInChildren<MeshRenderer>().material.color;

        }

        void OnEnable()
        {
            Move = true;
            knockback.OnDoKill += Death;
        }
    
        void Death()
        {
            Move = false;
            Invoke(nameof(DeathFX), UnityEngine.Random.Range(DespawnMinMax.x, DespawnMinMax.y));
            GetComponentInChildren<MeshRenderer>().material.color = Color.red;
        }

        void DeathFX()
        {
            GetComponentInChildren<MeshRenderer>().material.color = original;
            if (DeathEffectPrefab != null)
            {
                SimplePool.Spawn(DeathEffectPrefab, transform.position, Quaternion.identity);
            }
 
            OnDeathComplete?.Invoke(this);
        }
        void OnDisable()
        {
            knockback.OnDoKill -= Death;
        }
        // Start is called before the first frame update
        void Start()
        {
            if (OverrideTarget == null)
            {
                GameObject obj = HitBoxTeamManager.FindNearest(TeamTarget, transform.position);
                if (obj != null)
                {
                    target = obj.transform;
                }
                else
                {
                    target = this.transform;
                }
            }
            else
            {
                target = OverrideTarget;
            }
            Move = true;
            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<Rigidbody>().useGravity = true;
        }

        private void FixedUpdate()
        {
            if (Move == false) return;

            targettimer += Time.deltaTime;
            if (targettimer >= CheckTargetRate)
            {
                targettimer = 0;
                if (OverrideTarget == null)
                {
                    GameObject obj = HitBoxTeamManager.FindNearest(TeamTarget, transform.position);
                    if (obj != null)
                    {
                        target = obj.transform;
                    }
                    else
                    {
                        target = this.transform;
                    }
                }
                else
                {
                    target = OverrideTarget;
                }
            }
            


            grounded = Physics.CheckSphere(self.position, GroundCheckRadius, Ground);
            if (grounded)
            { 

                Vector3 dir = target.position - self.position + UnityEngine.Random.insideUnitSphere * Radius;
                dir.y = 0;
                rb.AddForce(dir.normalized * MoveForce);
                rb.velocity = Vector3.ClampMagnitude(rb.velocity, MaxSpeed);
            }

        }

        public List<string> GetHitGivers()
        {
            return HitGivers;
        }

        public List<string> GetHitTakers()
        {
            return HitTakers;
        }

        public void SetHitTakers(List<string> takers)
        {
            HitTakers = takers;
        }

        public void SetHitGivers(List<string> givers)
        {
            HitGivers = givers;
        }
    }
}