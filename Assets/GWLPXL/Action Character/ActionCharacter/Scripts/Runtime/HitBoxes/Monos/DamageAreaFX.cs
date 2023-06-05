using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{
    [System.Serializable]
    public class AreaFX
    {
        public string Area;
        public GameObject VFXPrefab;
        public Transform Parent;

    }

    public class DamageAreaFX : HitBoxHitGiverSub
    {
        public List<AreaFX> AreaFX = new List<AreaFX>();
        Dictionary<string, AreaFX> damageAreadic = new Dictionary<string, AreaFX>();


        protected override void Awake()
        {
            base.Awake();
            for (int i = 0; i < AreaFX.Count; i++)
            {
                damageAreadic.Add(AreaFX[i].Area.ToLowerInvariant(), AreaFX[i]);
            }
        }

        protected override void HitGiverEnabled(HitBoxArgs area)
        {
            base.HitGiverEnabled(area);
            string key = area.Name.ToLowerInvariant();
            if (damageAreadic.ContainsKey(key))
            {
                AreaFX fx = damageAreadic[key];
                GameObject instance = Instantiate(fx.VFXPrefab, fx.Parent.position, Quaternion.identity);
            }
    }
}
}