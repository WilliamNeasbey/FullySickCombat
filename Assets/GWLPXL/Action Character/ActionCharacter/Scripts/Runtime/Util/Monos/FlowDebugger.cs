
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{


    public class FlowDebugger : MonoBehaviour
    {
        PlayerCharacterCC cc;
        private void Awake()
        {
            cc = GetComponent<PlayerCharacterCC>();
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            FlowControlSO flows = cc.Flow;
            for (int i = 0; i < flows.Starters.Count; i++)
            {
                Debug.Log("Starter " + flows.Starters[i]);
            }
            foreach (var kvp in flows.RequiredStates)
            {
                string key = kvp.Key;
                string value = "Required: ";
                List<string> values = kvp.Value;
                for (int i = 0; i < values.Count; i++)
                {
                    value += values[i] + " ";
                }
                 Debug.Log("REQUIRED DEBUG " + key + " " + value);
            }

        }
    }
}