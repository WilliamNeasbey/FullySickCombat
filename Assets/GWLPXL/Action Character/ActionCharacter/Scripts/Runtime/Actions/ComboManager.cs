using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Text;

namespace GWLPXL.ActionCharacter
{
   
    public class SimpleCombo
    {
        public string Name = string.Empty;
        public List<SimpleComboPiece> Pieces = new List<SimpleComboPiece>();
    }
    public class SimpleComboSO : ScriptableObject
    {
        public string Name = string.Empty;
        public List<SimpleComboPiece> Pieces = new List<SimpleComboPiece>();
    }
    public static class ComboManager 
    {
        static Dictionary<int, List<string>> _ = new Dictionary<int, List<string>>();
        static StringBuilder sb = new StringBuilder();

        /// <summary>
        /// will also rename combos without names
        /// </summary>
        /// <param name="temp"></param>
        public static void VerifyCombos(CharacterActionLoadoutSO temp)
        {
            temp.GetActionSets().Clear();
            List<SimpleCombo> tempcombos = new List<SimpleCombo>();
            for (int i = 0; i < temp.Combos.Count; i++)
            {
                SimpleCombo newinstance = new SimpleCombo();
                string name = temp.Combos[i].Name;
                newinstance.Name = name;
                bool empty = false;
                if (string.IsNullOrWhiteSpace(name))
                {
                    Debug.LogWarning("Combo doesn't have a name, will set default naming.");
                    empty = true;
                    sb.Clear();

                }
                newinstance.Pieces = new List<SimpleComboPiece>();
                for (int j = 0; j < temp.Combos[i].Pieces.Count; j++)
                {

                    SimpleComboPiece c = temp.Combos[i].Pieces[j];
                    if (c.Action == null)
                    {
                        Debug.LogWarning("No action combo wont work, skipping " + name);
                    }
                    SimpleComboPiece newcopy = new SimpleComboPiece(c.Action, c.InputSlotIndex, c.Cancels, c.Forgiveness);
                    newinstance.Pieces.Add(newcopy);
                    if (empty)
                    {
                        sb.Append(newcopy.Action.GetActionName());
                        if (j < temp.Combos[i].Pieces.Count - 1)
                        {
                            sb.Append("=>");
                        }
                    }
                }

                if (empty)
                {
                    name = sb.ToString();
                }
                newinstance.Name = name;
       
                tempcombos.Add(newinstance);
            }

            for (int i = 0; i < tempcombos.Count; i++)
            {
                TryBuildCombo(temp, tempcombos[i].Name, tempcombos[i].Pieces);
            }

            CheckForComboStartConflicts(temp);

        }
     
        public static void CheckForComboStartConflicts(CharacterActionLoadoutSO loadout)
        {
            _.Clear();
            for (int i = 0; i < loadout.ActionSets.Count; i++)
            {
                ActionSet set = loadout.ActionSets[i];
                if (set.Flow.ComboStart)
                {
                    if (_.ContainsKey(set.InputIndex) == false)
                    {
                        _[set.InputIndex] = new List<string>() { set.Action };
                    }
                    else
                    {
                        List<string> current = _[set.InputIndex];
                        current.Add(set.Action);
                    }
                }
            }

            foreach (var kvp in _)
            {
                if (kvp.Value.Count > 1)
                {
                    Debug.LogWarning("You have multiple starts with the same input. This will work if they have different start requirements, i.e. ground, airborne, requires target, etc.");//to do, let i tknow the difference on ground/air
                    List<string> values = kvp.Value;
                    for (int i = 0; i < values.Count; i++)
                    {
                        Debug.LogWarning("Conflicting Actions: " + values[i] + " with input " + kvp.Key);
                    }
                }
            }
        }
        /// <summary>
        /// adds a combo to the loadout
        /// </summary>
        /// <param name="temp"></param>
        /// <param name="comboName"></param>
        /// <param name="actionName"></param>
        /// <param name="pieces"></param>
        public static void TryBuildCombo(CharacterActionLoadoutSO temp, string comboName, List<SimpleComboPiece> pieces)
        {
            //save it to action set
            List<ActionSet> sets = temp.GetActionSets();
            if (pieces.Count <= 0)
            {
                Debug.Log("Can't make combo, no pieces");
                return;
            }
            if (string.IsNullOrWhiteSpace(pieces[0].Action.GetActionName()))
            {
                Debug.Log("Can't make combo, no name for starting action");
                return;
            }
           // string first = pieces[0].Action.GetActionName();
            //string last = pieces[pieces.Count - 1].Action.GetActionName();
            List<Flow> flows = new List<Flow>();
            List<ActionSet> currentactionsets = new List<ActionSet>();
            for (int i = 0; i < sets.Count; i++)
            {
                Flow flow = sets[i].Flow;
                string current = flow.StartingAction;
                for (int j = 0; j < pieces.Count; j++)
                {
                    if (CommonFunctions.WordEquals(current, pieces[j].Action.GetActionName()))
                    {
                        //found it
                        currentactionsets.Add(sets[i]);
                        //flows.Add(flow);
                    }
                }



            }
            for (int i = 0; i < currentactionsets.Count; i++)
            {
                flows.Add(currentactionsets[i].Flow);
            }

            #region deprecate
            //List<string> messages = new List<string>();
            //for (int i = 0; i < flows.Count; i++)
            //{
            //    if (CommonFunctions.WordEquals(flows[i].StartingAction, first))
            //    {
            //        if (flows[i].RequiredType != ValidSequenceType.None || flows[i].RequiredType != ValidSequenceType.SelectedANDNone)
            //        {
            //           // Debug.Log("Not Possible");
            //           // messages.Add(flows[i].StartingAction + " has a starting combo conflict. '\n' The required type must be " + ValidSequenceType.None.ToString() + " or " + ValidSequenceType.SelectedANDNone);

            //        }

            //    }
            //    else if (CommonFunctions.WordEquals(flows[i].StartingAction, last) && flows.Count == 1)
            //    {

            //        //nothing

            //    }
            //    else
            //    {
            //        //not first
            //        if (flows[i].RequiredType != ValidSequenceType.SelectedOnly || flows[i].RequiredType != ValidSequenceType.SelectedANDNone)
            //        {
            //            //messages.Add(flows[i].StartingAction + " has a combo piece conflict.  '\n' The required type must be " + ValidSequenceType.SelectedANDNone.ToString() + " or " + ValidSequenceType.SelectedANDNone.ToString());

            //        }

            //    }
            //}


            //if (messages.Count > 0)
            //{
            //    //error
            //    for (int i = 0; i < messages.Count; i++)
            //    {
            //       // Debug.Log(messages[i]);
            //    }
            //}

            #endregion
            //create new 
            for (int i = 0; i < pieces.Count; i++)
                {
                    string piece = pieces[i].Action.GetActionName();
                    bool has = false;
                    for (int j = 0; j < sets.Count; j++)
                    {
                        if (CommonFunctions.WordEquals(piece, sets[j].Flow.StartingAction))
                        {
                            //already have
                            has = true;
                            break;
                        }
                    }

                    if (has == false)
                    {
                        List<string> nexts = new List<string>();
                        List<string> reqs = new List<string>();
                        List<string> cancels = new List<string>();
                        Flow flow = new Flow(piece, nexts, reqs, cancels);
                        if (i == 0)
                        {
                            flow.MakeStarting();
                        }

                        if (pieces.Count > 1)
                        {
                            if (i < pieces.Count - 1)
                            {
                                flow.AddNext(pieces[i + 1].Action.GetActionName());

                            }
                            if (i > 0)
                            {
                                flow.AddReq(pieces[i - 1].Action.GetActionName());

                            }
                        }



                        if (pieces[i].Cancels.Count > 0)
                        {
                            for (int j = 0; j < pieces[i].Cancels.Count; j++)
                            {
                                flow.AddCancel(pieces[i].Cancels[j].GetActionName());
                            }
                        }


                        sets.Add(new ActionSet(piece, flow, pieces[i].InputSlotIndex));


                    }
                }


                bool newone = true;
                for (int i = 0; i < temp.Combos.Count; i++)
                {
                    if (CommonFunctions.WordEquals(comboName, temp.Combos[i].Name))
                    {
                        temp.Combos[i].Pieces = pieces;
                        newone = false;
                        break;
                    }
                }

                if (newone)
                {
                    temp.Combos.Add(new Combos(comboName, pieces));

                }
                //modify existing

                for (int i = 0; i < flows.Count; i++)
                {
                    string flowkey = flows[i].StartingAction;

                    //find the first, add the piece
                    for (int j = 0; j < pieces.Count; j++)
                    {
 
                        if (CommonFunctions.WordEquals(pieces[j].Action.GetActionName(), flowkey))
                        {
                            //found it
                            if (j < pieces.Count - 1)
                            {

                                string next = pieces[j + 1].Action.GetActionName();
                                flows[i].AddNext(next);


                            }
                            if (j > 0)
                            {
                                string previous = pieces[j - 1].Action.GetActionName();
                                flows[i].AddReq(previous);

                            }


                            SimpleComboPiece p = pieces[j];
                            for (int k = 0; k < p.Cancels.Count; k++)
                            {
                                flows[i].AddCancel(p.Cancels[k].GetActionName());

                            }

                        }
                    }


                }

                if (temp is CharacterActionLoadoutSO)//re-assign input if not null
                {
                    CharacterActionLoadoutSO pload = temp as CharacterActionLoadoutSO;
                    List<ActionSet> psets = pload.ActionSets;
                    for (int i = 0; i < pieces.Count; i++)
                    {
                        string key = pieces[i].Action.GetActionName();
                        int input = pieces[i].InputSlotIndex;

                        for (int j = 0; j < psets.Count; j++)
                        {

                            if (CommonFunctions.WordEquals(key, psets[j].Flow.StartingAction))
                            {
                                psets[j].InputIndex = input;
                            }
                        }
                    }
                }


               
            



        }
    }
}
