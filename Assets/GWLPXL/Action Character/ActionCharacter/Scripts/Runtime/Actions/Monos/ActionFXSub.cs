using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{
    [System.Serializable]
    public class ActionFX
    {
        public string ActionName;
        public GameObject VFXPrefab;
        public Transform Parent;
        public float Delay = .25f;
    }

    /// <summary>
    /// example of extending ActionSub for the actions to create FX on action start
    /// probably want to use animation events or custom action code for more precision
    /// </summary>
    public class ActionFXSub : ActionSub
    {
        public List<ActionFX> ActionFx = new List<ActionFX>();

        protected Dictionary<string, ActionFX> runtimedic = new Dictionary<string, ActionFX>();
        protected override void Awake()
        {
            base.Awake();
            for (int i = 0; i < ActionFx.Count; i++)
            {
                string key = CommonFunctions.StringKey(ActionFx[i].ActionName);
                runtimedic.Add(key, ActionFx[i]);
            }

        }

        protected override void ActionEnabled(CharacterActionArgs area)
        {
            string key = CommonFunctions.StringKey(area.ActionName);
            Debug.Log("Action Name: " + key);
            if (runtimedic.ContainsKey(key))
            {
                ActionFX fx = runtimedic[key];
                StartCoroutine(Delay(fx));
            }
        }

        IEnumerator Delay(ActionFX fx)
        {
            yield return new WaitForSeconds(fx.Delay);
            InstantiateVFX(fx);
        }
        protected virtual void InstantiateVFX(ActionFX fx)
        {
            GameObject instance = Instantiate(fx.VFXPrefab, fx.Parent.position, fx.Parent.transform.rotation * fx.VFXPrefab.transform.rotation);
           
  
        }

        protected override void ActionDisabled(CharacterActionArgs area)
        {

            string key = CommonFunctions.StringKey(area.ActionName);
            
            if (runtimedic.ContainsKey(key))
            {
                print(key);
            }
        }
    }
}