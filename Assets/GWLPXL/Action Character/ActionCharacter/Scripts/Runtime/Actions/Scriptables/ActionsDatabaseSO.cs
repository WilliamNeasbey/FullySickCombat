using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{
    

    /// <summary>
    /// wrapper for the action database
    /// </summary>
    [CreateAssetMenu(fileName = "New Action Database", menuName = "GWLPXL/ActionCharacter/Action/Database", order = 210)]
    public class ActionsDatabaseSO : ScriptableObject
    {
        string[] actionames = new string[0];
        public ActionDatabase Database = new ActionDatabase();
        public string[] GetActionNames()
        {
            actionames = new string[Database.Actions.Count];
            for (int i = 0; i < Database.Actions.Count; i++)
            {
                actionames[i] = Database.Actions[i].GetActionName();
            }
            return actionames;
        }

    }


    /// <summary>
    /// base action database which holds references to all available actions
    /// </summary>
    [System.Serializable]
    public class ActionDatabase
    {
        public List<ActionSO> Actions = new List<ActionSO>();
        [System.NonSerialized]
        protected Dictionary<string, ActionSO> preloadDic = new Dictionary<string, ActionSO>();
        [System.NonSerialized]
        protected bool dirty = true;

        public virtual void Preload()
        {
            if (dirty == false) return;
            for (int i = 0; i < Actions.Count; i++)
            {
                string current = CommonFunctions.StringKey(Actions[i].GetActionName());
                if (preloadDic.ContainsKey(current) == false)
                {
                    preloadDic.Add(current, Actions[i]);
                }
               
              
            }


            dirty = false;
        }

        public virtual ActionSO GetAction(string name)//eventually move to dictionary
        {
            Preload();
            string key = CommonFunctions.StringKey(name);
            if (preloadDic.ContainsKey(key))
            {
                return preloadDic[key];
            }

            Debug.Log("Entry is null " + name);
            return null;
        }


    }

}