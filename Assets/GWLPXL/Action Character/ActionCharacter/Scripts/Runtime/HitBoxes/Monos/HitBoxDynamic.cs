using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{
    public enum HitBoxType
    {
        None = 0,
        HitGiver = 1,
        HitTaker = 2
    }
    public interface IHitBoxDynamic
    {
        void AssignHitBox(ActorHitBoxes toBoxes);

    }

    [System.Serializable]
    public class HitBoxDynamicVars : System.EventArgs
    {
        public HitCollider HitCollider;
        public HitBoxType Type;
        public bool ReplaceExisting;

        public HitBoxDynamicVars(HitCollider coll, HitBoxType type, bool replaceExisting)
        {
            HitCollider = coll;
            Type = type;
            ReplaceExisting = replaceExisting;
        }
    }
    /// <summary>
    /// base class for dynamic hitbox monos
    /// </summary>
    public abstract class HitBoxDynamic : MonoBehaviour, IHitBoxDynamic
    {
        public HitBoxDynamicVars Vars = new HitBoxDynamicVars(null, HitBoxType.None, false);

        [SerializeField]
        protected ActorHitBoxes toBoxes;

        protected virtual void Awake()
        {
            if (toBoxes == null)
            {
                toBoxes = transform.root.GetComponent<ActorHitBoxes>();
            }
        }

        protected virtual void OnEnable()
        {

        }

        protected virtual void OnDisable()
        {

        }

        public virtual void AssignHitBox(ActorHitBoxes toBoxes)
        {
            this.toBoxes = toBoxes;
        }

        public virtual void AddHitBox()
        {
            if (toBoxes == null) return;
            if (Vars.ReplaceExisting)
            {
                switch (Vars.Type)
                {
                    case HitBoxType.HitGiver:
                        toBoxes.HitBoxes.HitGivers.ReplaceHitCollider(Vars.HitCollider);
                        break;
                    case HitBoxType.HitTaker:
                        toBoxes.HitBoxes.HitTakers.ReplaceHitCollider(Vars.HitCollider);
                        break;

                }
            
            }
            else
            {
                switch (Vars.Type)
                {
                    case HitBoxType.HitGiver:
                        toBoxes.HitBoxes.HitGivers.AddHitGiver(Vars.HitCollider);
                        break;
                    case HitBoxType.HitTaker:
                        toBoxes.HitBoxes.HitTakers.AddHitTaker(Vars.HitCollider);
                        break;
                }

            }


        }
        public virtual void RemoveHitBox()
        {
            if (toBoxes == null) return;


            if (Vars.ReplaceExisting)
            {
                switch (Vars.Type)
                {
                    case HitBoxType.HitGiver:
                        toBoxes.HitBoxes.HitGivers.ReturnToDefault(Vars.HitCollider.Name);
                        break;
                    case HitBoxType.HitTaker:
                        toBoxes.HitBoxes.HitTakers.ReturnToDefault(Vars.HitCollider.Name);
                        break;
                }
            }
            else
            {
                switch (Vars.Type)
                {
                    case HitBoxType.HitGiver:
                        toBoxes.HitBoxes.HitGivers.RemoveHitGiver(Vars.HitCollider);
                        break;
                    case HitBoxType.HitTaker:
                        toBoxes.HitBoxes.HitTakers.RemoveHitTaker(Vars.HitCollider);
                        break;
                }

            }


        }


    }
}
