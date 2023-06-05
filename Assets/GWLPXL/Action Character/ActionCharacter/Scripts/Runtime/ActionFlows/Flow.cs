using System.Collections.Generic;
using UnityEngine;
namespace GWLPXL.ActionCharacter
{
   
    /// <summary>
    /// base class that defines a combo flow
    /// </summary>
    [System.Serializable]
    public class Flow
    {

        [Tooltip("Action Name. Key for mostly everything")]
        public string StartingAction = string.Empty;

        public ValidSequenceType RequiredType => rtpe;

        ValidSequenceType rtpe = ValidSequenceType.None;
        [Tooltip("Name of states that are prerequisite. For example, Combo_01 is required for Combo_02, so Combo_01 is a required state for Combo_02.")]
        public List<string> RequiredActionStates = new List<string>();
        public ValidSequenceType NextType => ntype;
        ValidSequenceType ntype = ValidSequenceType.None;

        [Tooltip("Name of states that are allowed to be next. For example, Combo_02 follows Combo_01, so Combo_02 is a next state for Combo_01.")]
        public List<string> Nexts = new List<string>();
        [Tooltip("Time in seconds for flow forgiveness")]
        public float ForgivenessDuration = .25f;
        public List<string> EarlyExits = new List<string>();
        public bool ComboStart = false;
        public Flow(string starting, List<string> nexts, List<string> required, List<string> earlyExits)
        {

            this.StartingAction = starting;

            for (int i = 0; i < nexts.Count; i++)
            {
                AddNext(nexts[i]);
            }
            for (int i = 0; i < required.Count; i++)
            {
                AddReq(required[i]);
            }
            for (int i = 0; i < earlyExits.Count; i++)
            {
                AddCancel(earlyExits[i]);
            }
 
        }

        public void MakeStarting()
        {
            ComboStart = true;
            rtpe = ValidSequenceType.SelectedANDNone;
        }
        public void AddCancel(string cancel)
        {
            for (int i = 0; i < EarlyExits.Count; i++)
            {
                if (CommonFunctions.WordEquals(cancel, EarlyExits[i]))
                {
                    //already added
                    return;
                }
            }

            EarlyExits.Add(cancel);

           
        }

        public void AddNext(string next)
        {
            for (int i = 0; i < Nexts.Count; i++)
            {
                if (CommonFunctions.WordEquals(next, Nexts[i]))
                {
                    return;
                }
            }

            Nexts.Add(next);

            if (Nexts.Count > 0)
            {
                if (NextType == ValidSequenceType.None)
                {
                    ntype = ValidSequenceType.SelectedOnly;
                }
            }
        }
        public void AddReq(string req)
        {
            for (int i = 0; i < RequiredActionStates.Count; i++)
            {
                if (CommonFunctions.WordEquals(req, RequiredActionStates[i]))
                {
                    //already added
                    return;
                }
            }

            RequiredActionStates.Add(req);

            if (RequiredActionStates.Count > 0)
            {
                if (RequiredType == ValidSequenceType.None)
                {
                    rtpe = ValidSequenceType.SelectedOnly;
                }
            }

            if (ComboStart)
            {
                rtpe = ValidSequenceType.SelectedANDNone;
            }
        }



    }
}
