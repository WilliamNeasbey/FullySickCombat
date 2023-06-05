using UnityEngine;

namespace GWLPXL.ActionCharacter
{
    /// <summary>
    /// information sent at a hurt collision
    /// </summary>

    [System.Serializable]
    public class HitContextCollision : System.EventArgs
    {
        public GameObject Attacker;
        public Collider AttackingCollider;
        public string AttackingPartName;
        public Collision Collision;
        public string DamagedPartName;
        public string AttackerAction;
        public HitContextCollision(GameObject owner, Collider attacker, string partname, string attackerAction, Collision collision)
        {
            Attacker = owner;
            AttackingCollider = attacker;
            AttackingPartName = partname;
            Collision = collision;
            AttackerAction = attackerAction;
        }
    }
}