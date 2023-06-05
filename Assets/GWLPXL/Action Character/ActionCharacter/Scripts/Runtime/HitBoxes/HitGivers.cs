
using System.Collections.Generic;
using UnityEngine;


namespace GWLPXL.ActionCharacter
{
    [System.Serializable]
    public class HitBoxBase
    {
        [Tooltip("First collider is self, second is other.")]
        public UnityPhysicsCallbacks UnityPhysicsEvents = new UnityPhysicsCallbacks();
        public PhysicsCallbacks PhysicsCallbacks = new PhysicsCallbacks();
        public List<HitCollider> HitBoxes = new List<HitCollider>();
        public List<string> DefaultHitBoxes = new List<string>();//used when no actions or cant find actions.
        public GroupHitBoxController GroupHitboxEnabler = new GroupHitBoxController();
    }

    [System.Serializable]
    public class HitBoxArgs : System.EventArgs
    {
        public string Name;
        public HitBoxArgs(string name)
        {
            Name = name;
        }
    }
    /// <summary>
    /// Controls the hitboxes that give hits/damage
    /// </summary>
    [System.Serializable]
    public class HitGivers
    {
        public int OwnerID => id;
        [Tooltip("Name of the layer. Will convert all hitboxes to this physics layer.")]
        public string HitBoxLayer = HitBoxDefaults.HitboxLayerName;
        public event System.Action<HitBoxArgs> OnHitGiverEnabled;
        public event System.Action<HitBoxArgs> OnHitGiverDisabled;

        public HitBoxBase HitBoxesBase = new HitBoxBase();
        public HitSender HitSender = new HitSender();
        public event System.Action<HitCollider> OnColliderAdded;
        public event System.Action<HitCollider> OnColliderRemoved;
        public List<HitCollider> Registered => registered;
        [SerializeField]
        protected bool useDebug = false;

        protected Dictionary<string, List<HitCollider>> colliderdefaults = new Dictionary<string, List<HitCollider>>();
        protected Dictionary<Collider, HitCollider> damagedic = new Dictionary<Collider, HitCollider>();
        protected Dictionary<Collider, string> damagedicName = new Dictionary<Collider, string>();

        protected Dictionary<string, List<HitCollider>> namedamagedic = new Dictionary<string, List<HitCollider>>();

        protected HitGiverEventCaller manager = new HitGiverEventCaller();
        protected List<HitCollider> registered = new List<HitCollider>();
        protected List<HitCollider> empty = new List<HitCollider>(0);

        protected int id = 0;
        protected int teamTag = 0;

       
        /// <summary>
        /// enables hit givers based on name
        /// </summary>
        /// <param name="name"></param>
        public virtual void EnableHitGiver(string name)
        {
            string key = CommonFunctions.StringKey(name);
            if (namedamagedic.ContainsKey(key))
            {
                List<HitCollider> colls = namedamagedic[key];
                for (int i = 0; i < colls.Count; i++)
                {
                    colls[i].PhysicsEvents.enabled = true;
                    colls[i].Collider.enabled = true;

               

                }
                namedamagedic[key] = colls;
                OnHitGiverEnabled?.Invoke(new HitBoxArgs(name));
            }
           
        }

        /// <summary>
        /// disable hit givers based on name
        /// </summary>
        /// <param name="name"></param>
        public virtual void DisableHitGiver(string name)
        {
            string key = CommonFunctions.StringKey(name);
            if (namedamagedic.ContainsKey(key))
            {
                List<HitCollider> colls = namedamagedic[key];
                for (int i = 0; i < colls.Count; i++)
                {
                    colls[i].PhysicsEvents.enabled = false;
                    colls[i].Collider.enabled = false;



                }
                namedamagedic[key] = colls;
                OnHitGiverDisabled?.Invoke(new HitBoxArgs(name));

            }
        }
        /// <summary>
        /// do we own this collider
        /// </summary>
        /// <param name="collider"></param>
        /// <returns></returns>
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
       
        /// <summary>
        /// setup, subs, add defaults, registers
        /// </summary>
        /// <param name="ownerID"></param>
        /// <param name="teamTag"></param>
        public virtual void Setup(int ownerID, int teamTag)
        {
            this.id = ownerID;
            this.teamTag = teamTag;


            HitGiverManager.UnRegister(id);
            manager.OnCollisionEnter += HitGiverCollisionEnter;
            manager.OnCollisionExit += HitGiverCollisionExit;
            manager.OnCollisionStay += HitGiverCollisionStay;

            manager.OnTriggerEnter += HitGiverTriggerEnter;
            manager.OnTriggerExit += HitGiverTriggerExit;
            manager.OnTriggerStay += HitGiverTriggerStay;

            for (int i = 0; i < HitBoxesBase.HitBoxes.Count; i++)
            {
                HitCollider coll = HitBoxesBase.HitBoxes[i];
                AddHitGiver(coll);
            }


            for (int i = 0; i < HitBoxesBase.DefaultHitBoxes.Count; i++)
            {
                EnableHitGiver(HitBoxesBase.DefaultHitBoxes[i]);
            }


            HitBoxesBase.GroupHitboxEnabler.AssignToDictionary(id);
            HitBoxesBase.GroupHitboxEnabler.DisableAll();
            ActionManager.OnActionComplete += HitSender.ResetDamageList;
            HitSender.Setup(id);

            HitGiverManager.Register(id, this);
        
        }
        /// <summary>
        /// replaces the current hitcolliders with the same key. Use to switch out colliders on dynamic objects, i.e. weapons
        /// </summary>
        /// <param name="hitcollider"></param>
        public virtual void ReplaceHitCollider(HitCollider hitcollider)
        {
            string key = CommonFunctions.StringKey(hitcollider.Name);
            if (namedamagedic.ContainsKey(key))
            {
                List<HitCollider> _ = namedamagedic[key];
                colliderdefaults.Add(key, _);
                for (int i = 0; i < _.Count; i++)
                {
                    RemoveHitGiver(_[i]);
                }
            }
            AddHitGiver(hitcollider);
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
        /// <summary>
        /// reloads the collider back to their default state. Replace existing must be called first to have any effect.
        /// </summary>
        /// <param name="hitbox"></param>
        public virtual void ReturnToDefault(string hitbox)
        {
            string key = CommonFunctions.StringKey(hitbox);
            if (namedamagedic.ContainsKey(key))
            {
                List<HitCollider> _ = namedamagedic[key];
                for (int i = 0; i < _.Count; i++)
                {
                    RemoveHitGiver(_[i]);
                }
            }
                    
            if (colliderdefaults.ContainsKey(key))
            {
                List<HitCollider> _ = colliderdefaults[key];
                for (int i = 0; i < _.Count; i++)
                {
                    AddHitGiver(_[i]);
                }

                colliderdefaults.Remove(key);
            }



        }
        public virtual List<HitCollider> GetExistingHitColliders(HitCollider hitboxname)
        {
            string key = CommonFunctions.StringKey(hitboxname.Name);
            if (namedamagedic.ContainsKey(key))
            {
                return namedamagedic[key];
            }
            return empty;
        }
        /// <summary>
        /// adds a new hit collider
        /// </summary>
        /// <param name="coll"></param>
        public virtual void AddHitGiver(HitCollider coll)
        {
            coll.Setup();

            int layer = LayerMask.NameToLayer(HitBoxLayer);
            coll.Collider.gameObject.layer = layer;
            manager.RegisterEvents(coll.PhysicsEvents);

            string key = CommonFunctions.StringKey(coll.Name);
            List<HitCollider> colls;
            if (namedamagedic.ContainsKey(key))
            {
                colls = namedamagedic[key];
                colls.Add(coll);
                namedamagedic[key] = colls;
            }
            else
            {
                colls = new List<HitCollider>();
                colls.Add(coll);
                namedamagedic.Add(key, colls);
            }

      
            damagedic.Add(coll.Collider, coll);
            damagedicName.Add(coll.Collider, coll.Name);


            HitGiverManager.AddNewCollider(coll.Collider, this);
            HitBoxTeamManager.AddHitGiversToTeam(coll, teamTag);

            IgnoreCollision(coll);
            registered.Add(coll);
            OnColliderAdded?.Invoke(coll);

            DisableHitGiver(coll.Name);//starts disabled
        }

        /// <summary>
        /// shortcut to ignore collision from a hitcollider
        /// </summary>
        /// <param name="coll"></param>
        public virtual void IgnoreCollision(HitCollider coll)
        {
            for (int i = 0; i < registered.Count; i++)
            {
                Physics.IgnoreCollision(coll.Collider, registered[i].Collider, true);
                //Debug.Log("Collision Ignored " + coll.Name + " with " + registered[i].Name);
            }

        }

   
        /// <summary>
        /// to enable collision for a hitcollider
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
        /// removes hit giver 
        /// </summary>
        /// <param name="removed"></param>
        public virtual void RemoveHitGiver(HitCollider removed)
        {
            if (registered.Contains(removed))
            {
                manager.UnRegisterEvents(removed.PhysicsEvents);
                damagedic.Remove(removed.Collider);
                damagedicName.Remove(removed.Collider);
                namedamagedic.Remove(CommonFunctions.StringKey(removed.Name));

                registered.Remove(removed);
                HitGiverManager.RemoveCollider(removed.Collider);
                OnColliderRemoved?.Invoke(removed);
                HitBoxTeamManager.RemoveHitGiversFromTeam(removed, teamTag);

            }
        }

        /// <summary>
        /// disables hit giver system
        /// </summary>
        public virtual void CloseDown()
        {
            for (int i = 0; i < registered.Count; i++)
            {
                RemoveHitGiver(registered[i]);
            }

            manager.OnCollisionEnter -= HitGiverCollisionEnter;
            manager.OnCollisionExit -= HitGiverCollisionExit;
            manager.OnCollisionStay -= HitGiverCollisionStay;

            manager.OnTriggerEnter -= HitGiverTriggerEnter;
            manager.OnTriggerExit -= HitGiverTriggerExit;
            manager.OnTriggerStay -= HitGiverTriggerStay;

            damagedic.Clear();
            damagedicName.Clear();
            namedamagedic.Clear();
            ActionManager.OnActionComplete -= HitSender.ResetDamageList;

            HitGiverManager.UnRegister(id);
        }

        protected virtual void DebugMessage(string message)
        {
            if (useDebug)
            {
                Debug.Log(message);
            }
        }


       /// <summary>
       /// hit giver collision enter
       /// </summary>
       /// <param name="self"></param>
       /// <param name="collision"></param>
        protected virtual void HitGiverCollisionEnter(CollisionEventArgs args)
        {


            if (HitSender.OnEnter)
            {
                DebugMessage("Damage Collision ENTER " + args.S.name + " with " + args.C.collider.name);
                HitBoxesBase.UnityPhysicsEvents.FireCollisionEnter(this, args);
                HitBoxesBase.PhysicsCallbacks.FireCollisionEnter(this, args);
                TrySendHitGiverCollisionResults(args);

            }

        }

       /// <summary>
       /// hit giver collision stay
       /// </summary>
       /// <param name="self"></param>
       /// <param name="collision"></param>
        protected virtual void HitGiverCollisionStay(CollisionEventArgs args)
        {


            if (HitSender.OnStay)
            {
                DebugMessage("Damage Collision STAY " + args.S.name + " with " + args.C.collider.name);
                HitBoxesBase.UnityPhysicsEvents.FireCollisionStay(this, args);
                HitBoxesBase.PhysicsCallbacks.FireCollisionStay(this, args);
                TrySendHitGiverCollisionResults(args);

            }

        }
      
        /// <summary>
        /// hit giver collision exit
        /// </summary>
        /// <param name="self"></param>
        /// <param name="collision"></param>
        protected virtual void HitGiverCollisionExit(CollisionEventArgs args)
        {


            if (HitSender.OnExit)
            {
                DebugMessage("Damage Trigger EXIT " + args.S.name + " with " + args.C.collider.name);
                HitBoxesBase.UnityPhysicsEvents.FireCollisionExit(this, args);
                HitBoxesBase.PhysicsCallbacks.FireCollisionExit(this, args);
                TrySendHitGiverCollisionResults(args);

            }

        }

        /// <summary>
        /// hit giver send collision results
        /// </summary>
        /// <param name="self"></param>
        /// <param name="collision"></param>
        protected virtual void TrySendHitGiverCollisionResults(CollisionEventArgs args)
        {
            if (IsOwnHurtBox(id, args.C.collider) == false)
            {
                string hitpart = "NULL";
                if (damagedicName.ContainsKey(args.S))
                {
                    hitpart = damagedicName[args.S];
                    DebugMessage(damagedicName[args.S] + " damaged " + args.C.collider);
                }

                string actionName = ActionManager.GetActionName(id);
                GameObject actionObject = HitGiverManager.GetOwner(args.S);
                HitContextCollision ctx = new HitContextCollision(actionObject, args.S, hitpart, actionName, args.C);
                HitSender.SendDamage(ctx);
         
            }
        }
      
        /// <summary>
        /// shortcut to ask if we own this hit taker
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="collider"></param>
        /// <returns></returns>
        protected virtual bool IsOwnHurtBox(int owner, Collider collider)
        {
            return HitTakerManager.IsOwner(owner, collider);
        }
        /// <summary>
        /// hit giver on trigger enter
        /// </summary>
        /// <param name="self"></param>
        /// <param name="other"></param>
        protected virtual void HitGiverTriggerEnter(TriggerEventArgs args)
        {
            DebugMessage("Damage Trigger ENTER " + args.S.name + " with " + args.O.name);
            HitBoxesBase.UnityPhysicsEvents.FireTriggerEnter(this, args);
            HitBoxesBase.PhysicsCallbacks.FireTriggerEnter(this, args);

            if (HitSender.OnEnter)
            {
                TrySendHitGiverTriggerResults(args);
                DebugMessage(damagedicName[args.S] + " damaged " + args.O);
            }

        }
        /// <summary>
        /// hit giver on trigger stay
        /// </summary>
        /// <param name="self"></param>
        /// <param name="other"></param>
        protected virtual void HitGiverTriggerStay(TriggerEventArgs args)
        {
            DebugMessage("Damage Trigger STAY " + args.S.name + " with " + args.O);

            HitBoxesBase.UnityPhysicsEvents.FireTriggerStay(this, args);
            HitBoxesBase.PhysicsCallbacks.FireTriggerStay(this, args);

            if (HitSender.OnStay)
            {
                TrySendHitGiverTriggerResults(args);
                DebugMessage(damagedicName[args.S] + " damaged " + args.O);
            }

        }
        /// <summary>
        /// hit giver on trigger exit
        /// </summary>
        /// <param name="self"></param>
        /// <param name="other"></param>
        protected virtual void HitGiverTriggerExit(TriggerEventArgs args)
        {
            DebugMessage("Damage Collision EXIT " + args.S.name + " with " + args.O);
            HitBoxesBase.UnityPhysicsEvents.FireTriggerExit(this, args);
            HitBoxesBase.PhysicsCallbacks.FireTriggerExit(this, args);

            if (HitSender.OnExit)
            {
                TrySendHitGiverTriggerResults(args);
                DebugMessage(damagedicName[args.S] + " damaged " + args.O);
            }

        }
        /// <summary>
        /// hit giver, send trigger results
        /// </summary>
        /// <param name="self"></param>
        /// <param name="collider"></param>
        protected virtual void TrySendHitGiverTriggerResults(TriggerEventArgs args)
        {
            if (IsOwnHurtBox(id, args.O) == false)
            {
                string hitpart = string.Empty;
                if (damagedicName.ContainsKey(args.S))
                {
                    hitpart = damagedicName[args.S];
                }
         
                string actionName = ActionManager.GetActionName(id);
                //GameObject damageOver = HurtBoxManager.GetOwner(self);
                GameObject actionObject = HitGiverManager.GetOwner(args.S);
                HitContextTrigger ctx = new HitContextTrigger(actionObject, args.S, hitpart, actionName, args.O);
                HitSender.SendDamage(ctx);
                DebugMessage(damagedicName[args.S] + " damaged " + args.O);
            }
        }
    }
}