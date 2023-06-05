using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{
    [System.Serializable]
    public class ActionSFX
    {
        public string ActionName;
        public List<AudioClip> SFXClips = new List<AudioClip>();
        public Transform Parent;
        public float Delay = .25f;
    }

    public class ActionSFXSub : ActionSub
    {
        public List<ActionSFX> ActionFx = new List<ActionSFX>();
        public AudioSource Audio;
        protected Dictionary<string, ActionSFX> runtimedic = new Dictionary<string, ActionSFX>();
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
                ActionSFX fx = runtimedic[key];
                StartCoroutine(Delay(fx));
            }
        }

        IEnumerator Delay(ActionSFX fx)
        {
            yield return new WaitForSeconds(fx.Delay);
            if (fx.SFXClips.Count > 0)
            {
                int rando = Random.Range(0, fx.SFXClips.Count);
                Audio.PlayOneShot(fx.SFXClips[rando]);
            }

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

