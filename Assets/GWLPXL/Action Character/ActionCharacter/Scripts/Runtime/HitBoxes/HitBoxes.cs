
using UnityEngine;

namespace GWLPXL.ActionCharacter
{

    /// <summary>
    /// hitboxes, assigns givers and takers
    /// </summary>
    [System.Serializable]
    public class HitBoxes
    {
        public bool Active { get; set; }
        public int Owner => owner;
        public int TeamTag => teamTag;

        public HitGivers HitGivers = new HitGivers();
        public HitTakers HitTakers = new HitTakers();
        protected int owner = 0;
        protected int teamTag = 0;
        /// <summary>
        /// to enable the hitbox system
        /// </summary>
        /// <param name="owner"></param>
        public virtual void Setup(int ownerId, int teamTag, ActorHitBoxes boxes)
        {
            if (Active == true) return;
            owner = ownerId;
            this.teamTag = teamTag;

            //add the ignore collision events
            HitGivers.OnColliderAdded += HitTakers.IgnoreCollision;
            HitTakers.OnColliderAdded += HitGivers.IgnoreCollision;
            HitGivers.OnColliderAdded += HitTakers.EnableCollision;
            HitTakers.OnColliderAdded += HitGivers.EnableCollision;
            HitGivers.Setup(owner, this.teamTag);
            HitTakers.Setup(owner, this.teamTag);

            ActorHitBoxManager.Add(owner, boxes);
            Active = true;
        }



        /// <summary>
        /// to disable the hitbox system
        /// </summary>
        public virtual void CloseDown()
        {
            if (Active == false) return;

            HitGivers.OnColliderAdded -= HitTakers.IgnoreCollision;
            HitTakers.OnColliderAdded -= HitGivers.IgnoreCollision;
            HitGivers.OnColliderAdded -= HitTakers.EnableCollision;
            HitTakers.OnColliderAdded -= HitGivers.EnableCollision;
            HitGivers.CloseDown();
            HitTakers.CloseDown();
            ActorHitBoxManager.Remove(owner);
            Active = false;
        }
    }


}