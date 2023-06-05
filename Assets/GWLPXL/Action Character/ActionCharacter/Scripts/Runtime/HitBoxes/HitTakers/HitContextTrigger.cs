using UnityEngine;

namespace GWLPXL.ActionCharacter
{
    /// <summary>
    /// information sent at a hurt trigger
    /// </summary>

    [System.Serializable]
    public class HitContextTrigger : System.EventArgs
    {
        public GameObject Attacker;
        public Collider AttackingCollider;
        public string AttackAction;
        public string AttackingPartName;
        public string HitPartName;
        public Collider Other;

        public HitContextTrigger(GameObject owner, Collider attacker, string partname, string actionName, Collider other)
        {
    
            Attacker = owner;
            AttackingCollider = attacker;
            AttackingPartName = partname;
            Other = other;
            AttackAction = actionName;
        }
    }
}