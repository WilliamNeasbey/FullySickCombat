using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{
    public class AreaDamage : MonoBehaviour, IHaveHitBoxes
    {
        public List<string> HitGivers = new List<string>();
        public List<string> HitTakers = new List<string>();
        public List<string> GetHitGivers()
        {
            return HitGivers;
        }

        public List<string> GetHitTakers()
        {
            return HitTakers;
        }

        public void SetHitGivers(List<string> givers)
        {
            HitGivers = givers;
        }

        public void SetHitTakers(List<string> takers)
        {
            HitTakers = takers;
        }

       
    }
}
