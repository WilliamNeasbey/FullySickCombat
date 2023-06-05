using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{

    [CreateAssetMenu(fileName = "Test Custom Code", menuName = "GWLPXL/ActionCharacter/Action/Test Custom Code", order = 250)]
    public class TestCustomCodeSO : CustomActionCodeSO
    {
        public override bool HasRequirements(ActionCharacter instance)
        {
            return true;
        }

        public override void Tick(ActionTickerCC fromaction)
        {
            
        }
    }
}