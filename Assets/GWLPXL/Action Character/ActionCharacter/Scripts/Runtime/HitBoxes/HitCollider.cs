
using UnityEngine;


namespace GWLPXL.ActionCharacter
{

    /// <summary>
    /// defines a hit box
    /// </summary>
    [System.Serializable]
    public class HitCollider
    {
        public string Name;
        public Collider Collider;
        public PhysicsEvents PhysicsEvents { get; private set; }
        public Rigidbody CollisionBody { get; private set; }


        public virtual void Setup()
        {
            PhysicsEvents = Collider.gameObject.GetComponent<PhysicsEvents>();
            if (PhysicsEvents == null)
            {
                PhysicsEvents = Collider.gameObject.AddComponent<PhysicsEvents>();
            }
            CollisionBody = Collider.gameObject.GetComponent<Rigidbody>();
            if (CollisionBody == null)
            {
                CollisionBody = Collider.gameObject.AddComponent<Rigidbody>();
                CollisionBody.useGravity = false;
                CollisionBody.isKinematic = true;

            }

        }

        public HitCollider(string name, Collider coll)
        {
            Collider = coll;
            Name = name;
           
        }
    }
}