using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{
    /// <summary>
    /// example of damage fx, inherits hitactions and instantiates the prefab
    /// </summary>
    public class DamageFX : HitDamage
    {

        public GameObject CollisionFXPrefab;
        public AudioSource Audio;
        public AudioClip Clip;

        bool hashprefab;
        protected override void Awake()
        {
            base.Awake();
            hashprefab = CollisionFXPrefab != null;

        }
        protected override void DamageCollision(HitContextCollision ctx)
        {
            print("ATTACKER" + ctx.Attacker);
            CollisionPrefab(ctx);

        }
        protected override void DamageTrigger(HitContextTrigger ctx)
        {
            print("ATTACKER" + ctx.Attacker);
            TriggerPrefab(ctx);

        }

        private void CollisionPrefab(HitContextCollision ctx)
        {
            if (hashprefab == false) return;
            Vector3 pos = ctx.Collision.GetContact(0).point;
            GameObject instance = Instantiate(CollisionFXPrefab, pos, Quaternion.identity);
            Audio.PlayOneShot(Clip);
        }
        private void TriggerPrefab(HitContextTrigger ctx)
        {
            if (hashprefab == false) return;
            Vector3 pos = ctx.Other.transform.position;
            GameObject instance = Instantiate(CollisionFXPrefab, pos, Quaternion.identity);
            Audio.PlayOneShot(Clip);
        }

    }
}