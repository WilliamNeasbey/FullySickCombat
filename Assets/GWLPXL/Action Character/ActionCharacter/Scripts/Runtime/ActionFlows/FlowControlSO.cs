
using System.Collections.Generic;

using UnityEngine;

namespace GWLPXL.ActionCharacter
{
    public class FlowForgiveness : ITick
    {
        public string ActionName;
        public float Duration;
        FlowControlSO so;
        public FlowForgiveness(FlowControlSO so, string name, float duration)
        {
            this.so = so;
            ActionName = name;
            Duration = duration;
        }

        public void AddTicker()
        {
            TickManager.AddTicker(this);
        }

        public float GetTickDuration()
        {
            return Duration;
        }

        public void RemoveTicker()
        {
            TickManager.RemoveTicker(this);
        }

        public void Tick()
        {
            so.RemoveForgivness(null);
        }
    }
    /// <summary>
    /// scriptable object container for the combo flows data
    /// TO DO: handle any
    /// </summary>

    [CreateAssetMenu(fileName = "New Flow", menuName = "GWLPXL/ActionCharacter/Character/Flow", order = 130)]
    [System.Serializable]
    public class FlowControlSO : ScriptableObject
    {
        public Dictionary<string, List<string>> RequiredStates => requiredActionStateskeys;
        public Dictionary<string, List<string>> NextStates => currentToNextkeys;
        public List<string> Starters => starterActions;
        public string Current => current;


        [SerializeField]
        protected List<Flow> registered = new List<Flow>();
        [SerializeField]
        protected bool debug = false;

        protected List<string> empty = new List<string>();

        [System.NonSerialized]
        protected string current;
        [System.NonSerialized]
        protected List<string> starterActions = new List<string>();
        [System.NonSerialized]
        protected List<string> nexts = new List<string>();
        [System.NonSerialized]
        protected List<string> required = new List<string>();
        [System.NonSerialized]
        protected List<string> earlyExits = new List<string>();
        [System.NonSerialized]
        protected Dictionary<string, List<string>> currentToNextkeys = new Dictionary<string, List<string>>();
        [System.NonSerialized]
        protected Dictionary<string, List<string>> requiredActionStateskeys = new Dictionary<string, List<string>>();
        [System.NonSerialized]
        protected Dictionary<string, float> forgivenesskeys = new Dictionary<string, float>();
        [System.NonSerialized]
        protected Dictionary<string, List<string>> earlyExitsKeys = new Dictionary<string, List<string>>();

        [System.NonSerialized]
        protected int owner;
        [System.NonSerialized]
        protected ActionCharacter ownerInstance;
        [System.NonSerialized]
        protected FlowForgiveness forgiveness;
        [System.NonSerialized]
        protected bool hasForgiveness = false;

        [System.NonSerialized]
        protected bool allowEarlyExits = false;

        public virtual List<string> GetEarlyExits(string actionName)
        {
            return FindEarlyExits(actionName);
        }
        public virtual void EnableEarlyExits(bool isEnabled)
        {
            allowEarlyExits = isEnabled;
        }
        /// <summary>
        /// clears runtime data
        /// </summary>
        public virtual void Setup(ActionCharacter instance)
        {
            int id = instance.ID;
            ownerInstance = instance;
            if (id != owner)
            {
                instance.Events.OnActionStarted -= StartForgiveness;
                instance.Events.OnActionStarted += StartForgiveness;
                instance.Events.OnActionEnded -= RemoveForgivness;
                instance.Events.OnActionEnded += RemoveForgivness;
            }
            owner = id;
            starterActions.Clear();
            nexts.Clear();
            required.Clear();
            currentToNextkeys.Clear();
            requiredActionStateskeys.Clear();
            registered.Clear();
            earlyExitsKeys.Clear();

            current = string.Empty;
            if (hasForgiveness)
            {
                forgiveness.RemoveTicker();
                hasForgiveness = false;
            }

            

        }

        public virtual void StartForgiveness(CharacterActionArgs ctx)
        {

            string key = CommonFunctions.StringKey(ctx.ActionName);
            if (forgivenesskeys.ContainsKey(key) && forgivenesskeys[key] > 0)
            {
                if (hasForgiveness)
                {
                    forgiveness.RemoveTicker();
                }
                forgiveness = new FlowForgiveness(this, key, forgivenesskeys[key]);
                hasForgiveness = true;
            }

        }

        public virtual void RemoveForgivness(CharacterActionArgs ctx)
        {
            if (hasForgiveness)
            {
                hasForgiveness = false;
                forgiveness.RemoveTicker();
                forgiveness = null;
            }

        }
       

        /// <summary>
        /// sets the current action state of the character
        /// </summary>
        /// <param name="newcurrent"></param>
        public virtual void SetCurrent(string newcurrent)
        {
            current = newcurrent;
        }

        /// <summary>
        /// add new flow
        /// </summary>
        /// <param name="flow"></param>
        public virtual void AddFlow(Flow flow)
        {
            string key = flow.StartingAction.ToLowerInvariant();

            bool add = true;
            for (int i = 0; i < registered.Count; i++)
            {
                string current = registered[i].StartingAction.ToLowerInvariant();
                if (CommonFunctions.WordEquals(current, key) == true)
                {
  
                    Debug.LogWarning("Trying to add flow that already exists " + " " + current + " " + key);
                    add = false;
                    break;

                }
            }
            if (add)
            {
                forgivenesskeys[key] = flow.ForgivenessDuration;
                registered.Add(flow);
                SetupEarlyExits(flow);
                SetupCurrentToNexts(flow);
                SetupRequiredActions(flow);
                DebugMessage(DebugHelpers.FormatFlowDebug("Flow added " + key), this);
            }



           

        }

        /// <summary>
        /// Remove Flow
        /// </summary>
        /// <param name="flow"></param>
        public virtual void RemoveFlow(Flow flow)
        {
            RemoveFlow(flow.StartingAction);
            
        }
        /// <summary>
        /// Remove flow by action name
        /// </summary>
        /// <param name="flowname"></param>
        public virtual void RemoveFlow(string flowname)
        {
            string key = flowname.ToLowerInvariant();
            if (starterActions.Contains(key))
            {
                starterActions.Remove(key);
            }
            if (requiredActionStateskeys.ContainsKey(key))
            {
                requiredActionStateskeys.Remove(key);
            }
            if (currentToNextkeys.ContainsKey(key))
            {
                requiredActionStateskeys.Remove(key);
            }

            if (forgivenesskeys.ContainsKey(key))
            {
                forgivenesskeys.Remove(key);
            }

            if (earlyExitsKeys.ContainsKey(key))
            {
                earlyExitsKeys.Remove(key);
            }

            for (int i = 0; i < registered.Count; i++)
            {
                string current = registered[i].StartingAction.ToLowerInvariant();
                if (CommonFunctions.WordEquals(current, key))
                {
                    registered.RemoveAt(i);
                }
            }

        }
        

        /// <summary>
        /// initializes flow into runtime dictionaries for required actions
        /// </summary>
        /// <param name="flow"></param>
        protected virtual void SetupRequiredActions(Flow flow)
        {
            string startkey = flow.StartingAction.ToLowerInvariant();
            ValidSequenceType type = flow.RequiredType;

            if (flow.RequiredActionStates.Count == 0 || type == ValidSequenceType.Any || type == ValidSequenceType.None)
            {
                if (starterActions.Contains(startkey) == false)
                {
                    starterActions.Add(startkey);
                }
            }
            else
            {

                if (type == ValidSequenceType.SelectedANDNone)
                {
                    if (starterActions.Contains(startkey) == false)
                    {
                        starterActions.Add(startkey);
                    }
                }

                if (requiredActionStateskeys.ContainsKey(startkey) == false)
                {
                    List<string> newreq = new List<string>();
                    for (int j = 0; j < flow.RequiredActionStates.Count; j++)
                    {
                        string reqkey = flow.RequiredActionStates[j].ToLowerInvariant();
                        newreq.Add(reqkey);
                    }
                    requiredActionStateskeys[startkey] = newreq;

                }
                else
                {
                    List<string> required = requiredActionStateskeys[startkey];
                    for (int j = 0; j < flow.RequiredActionStates.Count; j++)
                    {
                        string reqkey = flow.RequiredActionStates[j].ToLowerInvariant();
                        if (required.Contains(reqkey) == false)
                        {
                            required.Add(reqkey);
                        }
                    }

                    requiredActionStateskeys[startkey] = required;

                }
            }
        }

        protected virtual void SetupEarlyExits(Flow flow)
        {
            string startkey = CommonFunctions.StringKey(flow.StartingAction);
            if (earlyExitsKeys.ContainsKey(startkey) == false)
            {
                List<string> exits = new List<string>();
                for (int i = 0; i < flow.EarlyExits.Count; i++)
                {
                    string key = CommonFunctions.StringKey(flow.EarlyExits[i]);
                    exits.Add(key);
                    
                }

                earlyExitsKeys[startkey] = exits;
            }
            else
            {
                List<string> exits = earlyExitsKeys[startkey];
                for (int i = 0; i < flow.EarlyExits.Count; i++)
                {
                    string key = CommonFunctions.StringKey(flow.EarlyExits[i]);
                    if (exits.Contains(key) == false)
                    {
                        exits.Add(key);
                    }
                }

                earlyExitsKeys[startkey] = exits;
            }
        }
        /// <summary>
        /// initializes flow into runtime dictionaries for next actions
        /// </summary>
        /// <param name="flow"></param>
        protected virtual void SetupCurrentToNexts(Flow flow)
        {
            string startkey = CommonFunctions.StringKey(flow.StartingAction);
            ValidSequenceType type = flow.NextType;

            if (currentToNextkeys.ContainsKey(startkey) == false)
            {

                if (type == ValidSequenceType.Any && starterActions.Contains(startkey) == false)
                {
                    starterActions.Add(startkey);
                }

                List<string> newnexts = new List<string>();
                for (int j = 0; j < flow.Nexts.Count; j++)
                {
                    string nextkey = flow.Nexts[j].ToLowerInvariant();
                    newnexts.Add(nextkey);

                }
                currentToNextkeys[startkey] = newnexts;

                

            }
            else
            {
                if (type == ValidSequenceType.Any && starterActions.Contains(startkey) == false)
                {
                    starterActions.Add(startkey);
                }

                List<string> newnexts = currentToNextkeys[startkey];
                for (int j = 0; j < flow.Nexts.Count; j++)
                {
                    string nextkey = flow.Nexts[j].ToLowerInvariant();
                    if (newnexts.Contains(nextkey) == false)
                    {
                        newnexts.Add(nextkey);
                    }


                }
                currentToNextkeys[startkey] = newnexts;

               



            }
        }

        protected virtual List<string> FindEarlyExits(string key)
        {
            if (allowEarlyExits == false) return empty;

            if (earlyExitsKeys.ContainsKey(key))
            {
                return earlyExitsKeys[key];
            }
            else
            {
                return empty;
            }
        }
        /// <summary>
        /// returns possible nexts from runtime dictionary
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected virtual List<string> FindPossibleNexts(string key)
        {
            if (currentToNextkeys.ContainsKey(key))
            {
                return currentToNextkeys[key];
            }
            else
            {
                return empty;
            }


        }
        /// <summary>
        /// returns required from runtime dictionary
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected virtual List<string> FindRequiredStates(string key)
        {
            DebugMessage(DebugHelpers.FormatFlowDebug("Looking for key " + key), this);
            if (requiredActionStateskeys.ContainsKey(key))
            {
                DebugMessage(DebugHelpers.FormatFlowDebug("Looking for key " + key) + DebugHelpers.FormatSuccessResponse(" Success"), this);
                return requiredActionStateskeys[key];
            }
            else
            {
                DebugMessage(DebugHelpers.FormatFlowDebug("Looking for key " + key) + DebugHelpers.FormatFailedResponse(" Failed"), this);
                return empty;
            }


        }

        protected virtual void DebugMessage(string message, UnityEngine.Object ctx)
        {
            if (debug)
            {
                DebugHelpers.DebugMessage(message, ctx, DebugMessageType.Log);
            }

        }
        /// <summary>
        /// searches if it's possible to transition to the next action
        /// </summary>
        /// <param name="next"></param>
        /// <returns></returns>
        public virtual bool CanTransition(string next)
        {
            if (string.IsNullOrWhiteSpace(next)) return false;

            string nextkey = CommonFunctions.StringKey(next);
            string currentKey = Current.ToLowerInvariant();
            #region check for forgiveness
            if (hasForgiveness)//produces double
            {
                currentKey = forgiveness.ActionName;
                DebugMessage(DebugHelpers.FormatFlowDebug("Action Forgivess for " + currentKey) + DebugHelpers.FormatSuccessResponse(" Success"), this);//generates garbage..

            }


            #endregion
            #region check for starter or forgivenessactions
            if (string.IsNullOrEmpty(Current))
            {
                    if (starterActions.Contains(nextkey))
                    {
                        DebugMessage(DebugHelpers.FormatFlowDebug("Action Starter or Any " + nextkey) + DebugHelpers.FormatSuccessResponse(" Success"), this);//generates garbage..
                        return true;
                    }
            }
            #endregion

            #region check for early exits
            earlyExits = FindEarlyExits(currentKey);
            for (int i = 0; i < earlyExits.Count; i++)
            {
                string earlyP = CommonFunctions.StringKey(earlyExits[i]);
                if (CommonFunctions.WordEquals(earlyP, nextkey))
                {
                    DebugMessage(DebugHelpers.FormatFlowDebug("Action Early Exit " + nextkey) + DebugHelpers.FormatSuccessResponse(" Success"), this);//generates garbage..
                    return true;
                }
            }

            #endregion

            #region check for possible nexts
            nexts = FindPossibleNexts(currentKey);

            
            if (nexts.Count == 0)//need a way to transition to self?
            {
                DebugMessage(DebugHelpers.FormatFlowDebug("Action Next Count 0 for " + nextkey) + DebugHelpers.FormatFailedResponse(" Failed"), this);
                return false;
            }

            bool found = false;
            for (int i = 0; i < nexts.Count; i++)
            {
                string nextp = CommonFunctions.StringKey(nexts[i]);

                if (CommonFunctions.WordEquals(nextp, nextkey))//need to check if not self?
                {
                    //found next
                    found = true;
                    DebugMessage(DebugHelpers.FormatFlowDebug("Action Next found for " + nextkey) + DebugHelpers.FormatSuccessResponse(" Success"), this);
                    break;
                }


            }

            if (found == false)
            {
                DebugMessage(DebugHelpers.FormatFlowDebug("Action Next for " + nextkey) + DebugHelpers.FormatFailedResponse(" Failed, 0 nexts found."), this);
                return false;
            }

            #endregion


            #region check for required
            required = FindRequiredStates(nextkey);
            if (required.Count == 0)
            {
                DebugMessage(DebugHelpers.FormatFlowDebug("Action Required for " + nextkey) + DebugHelpers.FormatFailedResponse(" Success, 0 required found."), this);
                return true;//if no required, true
            }

            bool hasreq = false;
            for (int i = 0; i < required.Count; i++)
            {
                string reqkey = required[i].ToLowerInvariant();
                if (CommonFunctions.WordEquals(reqkey, currentKey))
                {
                    hasreq = true;
                    DebugMessage(DebugHelpers.FormatFlowDebug("Action Required for " + current + " found") + DebugHelpers.FormatSuccessResponse(" Success, " + current + " to " + next), this);
                    break;
                }
            }

            if (hasreq == false)
            {
                DebugMessage(DebugHelpers.FormatFlowDebug("Can't Action transition no required " + nextkey) + DebugHelpers.FormatFailedResponse(" Failed, " + current + " to " + next), this);
                return false;
            }

            #endregion

            DebugMessage(DebugHelpers.FormatFlowDebug("Action transition success from " + current + " to " + nextkey) + DebugHelpers.FormatSuccessResponse(" Success, " + current + " to " + next), this);
            return true;


        }

    }


}

