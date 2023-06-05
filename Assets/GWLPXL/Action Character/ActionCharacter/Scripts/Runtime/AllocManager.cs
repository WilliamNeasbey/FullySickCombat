using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{
    public static class AllocManager
    {
        static List<string> ListString = new List<string>();

        public static List<string> GetListString()
        {
            ListString.Clear();
            return ListString;
        }
    }
}
