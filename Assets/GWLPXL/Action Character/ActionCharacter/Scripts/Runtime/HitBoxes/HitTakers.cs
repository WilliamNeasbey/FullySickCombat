
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{

    /// <summary>
    /// controls the hitboxes that take hits/hurt
    /// </summary>
    [System.Serializable]
    public class HitTakers
    {
        public int OwnerID => id;
        public List<HitCollider> Registered => registered;
        [Tooltip("Name of the layer. Will convert all hitboxes to this physics layer.")]
        public string HitBoxLayer = "Hitbox";
        public HitBoxBase HitBoxesBase = new HitBoxBase();
        public HitReceiver HitReceiver = new HitReceiver();
        public event System.Action<HitCollider> OnColliderAdded;
        public event System.Action<HitCollider> OnColliderRemoved;
        public event System.Action<HitBoxArgs> OnHitTakerEnabled;
        public event System.Action<HitBoxArgs> OnHitTakerDisabled;

        [SerializeField]
        protected bool UseDebug = false;


        protected Dictionary<string, List<HitCollider>> colliderdefaults = new Dictionary<string, List<HitCollider>>();
        protected Dictionary<Collider, HitCollider> hurtDic = new Dictionary<Collider, HitCollider>();
        protected Dictionary<Collider, string> hurtdicName = new Dictionary<Collider, string>();
        protected Dictionary<string, List<HitCollider>> namehurtdic = new Dictionary<string, List<HitCollider>>();
        protected HitTakerEventCaller manager = new HitTakerEventCaller();
        protected List<HitCollider> registered = new List<HitCollider>();

        protected int id = 0;
        protected int teamtag = 0;

        public virtual string GetHitTakerName(Collider key)
        {
            if (hurtdicName.ContainsKey(key))
            {
                return hurtdicName[key];
            }
            return string.Empty;

        }
        public virtual bool IsOwner(Collider collider)
        {
            for (int i = 0; i < registered.Count; i++)
            {
                if (collider == registered[i].Collider)
                {
                    return true;
                }
            }
            return false;
        }


        public virtual void ReplaceHitCollider(HitCollider hitcollider)
        {
            string key = CommonFunctions.StringKey(hitcollider.Name);
            if (namehurtdic.ContainsKey(key))
            {
                List<HitCollider> _ = namehurtdic[key];
                colliderdefaults.Add(key, _);
                for (int i = 0; i < _.Count; i++)
                {
                    RemoveHitTaker(_[i]);
                }
            }
            AddHitTaker(hitcollider);
        }

        /// <summary>
        /// reset everyone back to default starting state
        /// </summary>
        public virtual void ReturnToDefaultAll()
        {
            for (int i = 0; i < HitBoxesBase.HitBoxes.Count; i++)
            {
                ReturnToDefault(HitBoxesBase.HitBoxes[i].Name);
            }
        }
        public virtual void ReturnToDefault(string hitbox)
        {
            string key = CommonFunctions.StringKey(hitbox);
            if (namehurtdic.ContainsKey(key))
            {
                List<HitCollider> _ = namehurtdic[key];
                for (int i = 0; i < _.Count; i++)
                {
                    RemoveHitTaker(_[i]);
                }
            }

            if (colliderdefaults.ContainsKey(key))
            {
                List<HitCollider> _ = colliderdefaults[key];
                for (int i = 0; i < _.Count; i++)
                {
                    AddHitTaker(_[i]);
                }

                colliderdefaults.Remove(key);
            }



        }

        /// <summary>
        /// disable hit taker by name
        /// </summary>
        /// <param name="name"></param>
        public virtual void DisableHitTaker(string name)
        {
            string key = CommonFunctions.StringKey(name);
            if (namehurtdic.ContainsKey(key))
            {
                List<HitCollider> _ = namehurtdic[key];
                for (int i = 0; i < _.Count; i++)
                {
                    _[i].PhysicsEvents.enabled = false;
                    _[i].Collider.enabled = false;
                }
                namehurtdic[key] = _;
                OnHitTakerDisabled?.Invoke(new HitBoxArgs(name));
            }
        }

        /// <summary>
        /// enable hit taker by name
        /// </summary>
        /// <param name="name"></param>
        public virtual void EnableHitTaker(string name)
        {
            string key = CommonFunctions.StringKey(name);
            if (namehurtdic.ContainsKey(key))
            {
                List<HitCollider> _ = namehurtdic[key];
                for (int i = 0; i < _.Count; i++)
                {
                    _[i].PhysicsEvents.enabled = true;
                    _[i].Collider.enabled = true;
                }

                OnHitTakerEnabled?.Invoke(new HitBoxArgs(name));
            }
        }

        public virtual List<HitCollider> GetHitTaker(string area)
        {
            string key = CommonFunctions.StringKey(area);
            if (namehurtdic.ContainsKey(key))
            {
                return namehurtdic[key];
            }
            return null;
        }
        public virtual HitCollider GetHitTaker(Collider area)
        {

            if (hurtDic.ContainsKey(area))
            {
                return hurtDic[area];
            }
            return null;
        }
        protected virtual void DebugMessage(string message)
        {
            if (UseDebug)
            {
                Debug.Log(message);
            }
        }
        /// <summary>
        /// sub, register, add default hit takers
        /// </summary>
        /// <param name="ownerID"></param>
        /// <param name="teamTag"></param>
        public virtual void Setup(int ownerID, int teamTag)
        {

            id = ownerID;
            this.teamtag = teamTag;
            HitTakerManager.UnRegister(id);
            manager.OnCollisionEnter += HitTakerCollisionEnter;
            manager.OnCollisionExit += HitTakerCollisionExit;
            manager.OnCollisionStay += HitTakerCollisionStay;

            manager.OnTriggerEnter += HitTakerTriggerEnter;
            manager.OnTriggerExit += HitTakerTriggerExit;
            manager.OnTriggerStay += HitTakerTriggerStay;

            for (int i = 0; i < HitBoxesBase.HitBoxes.Count; i++)
            {
                HitCollider hitbox = HitBoxesBase.HitBoxes[i];
                AddHitTaker(hitbox);
            }

            for (int i = 0; i < HitBoxesBase.DefaultHitBoxes.Count; i++)
            {
                EnableHitTaker(HitBoxesBase.DefaultHitBoxes[i]);
            }

            HitReceiver.Setup(id);
            HitTakerManager.Register(id, this);
        }

        /// <summary>
        /// add a new hit taker
        /// </summary>
        /// <param name="hitbox"></param>
        public virtual void AddHitTaker(HitCollider hitbox)
        {
            hitbox.Setup();

            int layer = LayerMask.NameToLayer(HitBoxLayer);
            hitbox.Collider.gameObject.layer = layer;
            manager.RegisterEvents(hitbox.PhysicsEvents);

            string key = CommonFunctions.StringKey(hitbox.Name);
            List<HitCollider> colls;
            if (namehurtdic.ContainsKey(key))
            {
                colls = namehurtdic[key];
                colls.Add(hitbox);
                namehurtdic[key] = colls;
            }
            else
            {
                colls = new List<HitCollider>();
                colls.Add(hitbox);
                namehurtdic.Add(key, colls);
            }
            hurtDic.Add(hitbox.Collider, hitbox);
            hurtdicName.Add(hitbox.Collider, hitbox.Name);

            HitTakerManager.AddNewCollider(hitbox.Collider, this);
            HitBoxTeamManager.AddHitTakersToTeam(hitbox, teamtag);

            IgnoreCollision(hitbox);
            registered.Add(hitbox);
            OnColliderAdded?.Invoke(hitbox);

            DisableHitTaker(hitbox.Name);//starts disabled
        }

        /// <summary>
        /// ignore collision with hitcollider
        /// </summary>
        /// <param name="hitbox"></param>
        public virtual void IgnoreCollision(HitCollider hitbox)
        {
            for (int i = 0; i < registered.Count; i++)
            {
                Physics.IgnoreCollision(hitbox.Collider, registered[i].Collider);
            }
        }
        /// <summary>
        /// enable collision with hitcollider
        /// </summary>
        /// <param name="coll"></param>
        public virtual void EnableCollision(HitCollider coll)
        {
            for (int i = 0; i < registered.Count; i++)
            {
                Physics.IgnoreCollision(coll.Collider, registered[i].Collider, false);
            }
        }

        /// <summary>
        /// remove hit taker
        /// </summary>
        /// <param name="hitbox"></param>
        public virtual void RemoveHitTaker(HitCollider hitbox)
        {
            if (registered.Contains(hitbox))
            {
                manager.UnRegisterEvents(hitbox.PhysicsEvents);
                hurtDic.Remove(hitbox.Collider);
                hurtdicName.Remove(hitbox.Collider);
                namehurtdic.Remove(CommonFunctions.StringKey(hitbox.Name));
                registered.Remove(hitbox);
                HitTakerManager.RemoveCollider(hitbox.Collider);
                OnColliderRemoved?.Invoke(hitbox);
                HitBoxTeamManager.RemoveHitTakersFromTeam(hitbox, teamtag);
            }

        }
        /// <summary>
        /// close down hit taker system
        /// </summary>
        public virtual void CloseDown()
        {
            HitGiverManager.UnRegister(id);
            for (int i = 0; i < registered.Count; i++)
            {
                RemoveHitTaker(registered[i]);
            }
            manager.OnCollisionEnter -= HitTakerCollisionEnter;
            manager.OnCollisionExit -= HitTakerCollisionExit;
            manager.OnCollisionStay -= HitTakerCollisionStay;

            manager.OnTriggerEnter -= HitTakerTriggerEnter;
            manager.OnTriggerExit -= HitTakerTriggerExit;
            manager.OnTriggerStay -= HitTakerTriggerStay;

            hurtDic.Clear();
            hurtdicName.Clear();
            namehurtdic.Clear();

        }



        protected virtual void HitTakerCollisionEnter(CollisionEventArgs args)
        {
            DebugMessage("Hurt Collision ENTER " + args.S.name + " with " + args.C.collider.name);

            HitBoxesBase.UnityPhysicsEvents.FireCollisionEnter(this, args);
            HitBoxesBase.PhysicsCallbacks.FireCollisionEnter(this, args);


        }
        protected virtual void HitTakerCollisionStay(CollisionEventArgs args)
        {

            DebugMessage("Damage Collision STAY " + args.S.name + " with " + args.C.collider.name);
            HitBoxesBase.UnityPhysicsEvents.FireCollisionStay(this, args);
            HitBoxesBase.PhysicsCallbacks.FireCollisionStay(this, args);

        }
        protected virtual void HitTakerCollisionExit(CollisionEventArgs args)
        {
            DebugMessage("Hurt Trigger EXIT " + args.S.name + " with " + args.C.collider.name);
            HitBoxesBase.UnityPhysicsEvents.FireCollisionExit(this, args);
            HitBoxesBase.PhysicsCallbacks.FireCollisionExit(this, args);

        }

        protected virtual void HitTakerTriggerEnter(TriggerEventArgs args)
        {
            DebugMessage("Hurt Trigger ENTER " + args.S.name + " with " + args.O.name);
            HitBoxesBase.UnityPhysicsEvents.FireTriggerEnter(this, args);
            HitBoxesBase.PhysicsCallbacks.FireTriggerEnter(this, args);



        }
        protected virtual void HitTakerTriggerStay(TriggerEventArgs args)
        {
            DebugMessage("Hurt Trigger STAY " + args.S.name + " with " + args.O);
            HitBoxesBase.UnityPhysicsEvents.FireTriggerStay(this, args);
            HitBoxesBase.PhysicsCallbacks.FireTriggerStay(this, args);


        }
        protected virtual void HitTakerTriggerExit(TriggerEventArgs args)
        {
            DebugMessage("Hurt Trigger EXIT " + args.S.name + " with " + args.O);
            HitBoxesBase.UnityPhysicsEvents.FireTriggerExit(this, args);
            HitBoxesBase.PhysicsCallbacks.FireTriggerExit(this, args);


        }


    }
}