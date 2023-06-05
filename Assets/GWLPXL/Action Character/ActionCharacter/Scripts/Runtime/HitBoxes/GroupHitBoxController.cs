
using System.Collections.Generic;
using UnityEngine;


namespace GWLPXL.ActionCharacter
{
    [System.Serializable]
    public class GroupBoxes
    {
        public string GroupName;
        [Tooltip("Enables on Start, Disables on End")]
        public List<string> HitBoxes = new List<string>();

    }

    /// <summary>
    /// Enables / Disables Hitbox groups at runtime based on group names
    /// </summary>
    [System.Serializable]
    public class GroupHitBoxController
    {

        public List<GroupBoxes> Groups = new List<GroupBoxes>();
        [SerializeField]
        protected bool UseDebug = false;

        protected int owner;
        protected Dictionary<string, GroupBoxes> runtime = new Dictionary<string, GroupBoxes>();

        /// <summary>
        /// assigns owner and loads dictionary
        /// </summary>
        /// <param name="owner"></param>
        public void AssignToDictionary(int owner)
        {
            this.owner = owner;
            for (int i = 0; i < Groups.Count; i++)
            {
                runtime[Groups[i].GroupName.ToLowerInvariant()] = Groups[i];

            }
        }

        /// <summary>
        /// disables all
        /// </summary>
        public virtual void DisableAll()
        {
            for (int i = 0; i < Groups.Count; i++)
            {
                DisableBoxes(Groups[i].GroupName);

            }
        }

        /// <summary>
        /// enables boxes based on action name
        /// </summary>
        /// <param name="groupname"></param>
        public virtual void EnableBoxes(string groupname)
        {
            string key = CommonFunctions.StringKey(groupname);
            if (runtime.ContainsKey(key))
            {
                HitGiverManager.EnableHitGivers(owner, runtime[key].HitBoxes);
                DebugMessage("Enabled Areas for " + groupname);
            }
        }
       

        /// <summary>
        /// disables boxes based on action name
        /// </summary>
        /// <param name="groupname"></param>
        public virtual void DisableBoxes(string groupname)
        {
            string key = CommonFunctions.StringKey(groupname);
            if (runtime.ContainsKey(key))
            {
                HitGiverManager.DisableHitGivers(owner, runtime[key].HitBoxes);
                DebugMessage("Disabled Areas for " + groupname);

            }
        }
        protected virtual void DebugMessage(string message)
        {
            if (UseDebug)
            {
                Debug.Log(message);
            }
        }
    }
}