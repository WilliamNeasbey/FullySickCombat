using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter.Editor
{
    public static class ComboManagerEditor 
    {
        /// <summary>
        /// verification not working at the moment, to do look at it. for now, disable the messages
        /// </summary>
        /// <param name="database"></param>
        /// <param name="temp"></param>
        public static void TryVerifyDatabase(ActionsDatabaseSO database, CharacterActionLoadoutSO temp)
        {
            List<ActionSO> allactions = new List<ActionSO>();
            for (int i = 0; i < temp.Combos.Count; i++)
            {
                for (int j = 0; j < temp.Combos[i].Pieces.Count; j++)
                {
                    if (allactions.Contains(temp.Combos[i].Pieces[j].Action) == false)
                    {
                        allactions.Add(temp.Combos[i].Pieces[j].Action);
                    }
                    for (int k = 0; k < temp.Combos[i].Pieces[j].Cancels.Count; k++)
                    {
                        if (allactions.Contains(temp.Combos[i].Pieces[j].Cancels[k]) == false)
                        {
                            allactions.Add(temp.Combos[i].Pieces[j].Cancels[k]);
                        }
                    }
                }
            }
            for (int i = 0; i < allactions.Count; i++)
            {
                ActionSO _ = database.Database.GetAction(allactions[i].GetActionName());
                if (_ == null)
                {

                    //bool autoadd = UnityEditor.EditorUtility.DisplayDialog("Missing Action", "Action " + allactions[i].GetActionName() + " is missing from the database. Want me to add it?", "Yes.", "No");
                    //if (autoadd == false)
                    //{
                    //    bool secondtry = UnityEditor.EditorUtility.DisplayDialog("ERROR!", "Action " + allactions[i].GetActionName() + " MUST be added to the database for the combo to work", "Fine, add it.", "I said no");
                    //    if (secondtry)
                    //    {
                    //        //add
                    //        database.Database.Actions.Add(allactions[i]);
                    //        UnityEditor.EditorUtility.SetDirty(database);
                    //    }
                    //}
                    //else
                    //{
                    //    //add
                    //    database.Database.Actions.Add(allactions[i]);
                    //    UnityEditor.EditorUtility.SetDirty(database);
                    //}

                }
            }
        }
    }
}
