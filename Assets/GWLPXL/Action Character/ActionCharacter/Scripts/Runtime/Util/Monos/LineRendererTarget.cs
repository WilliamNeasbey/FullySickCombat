using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{
    public class LineRendererTarget : MonoBehaviour
    {
        public List<ActionSO> Actions;
        [SerializeField]
        protected LineRenderer lineRend;
        [SerializeField]
        protected int resolution = 10;
        [SerializeField]
        protected ActionCharacter ac;
        [SerializeField]
        protected Vector3 offset = new Vector3(0, 1, 0);

        protected virtual void Awake()
        {
            if (ac == null)
            {
                ac = transform.root.GetComponent<ActionCharacter>();
            }
       
            if (lineRend == null)
            {
                lineRend = GetComponent<LineRenderer>();
            }
        }
       

        
        protected virtual void OnEnable()
        {
            ActionTickerCC ticker = ActionManager.GetActionTickerCC(ac.ID);
            if (ticker != null && ticker.HasTarget)
            {
                Vector3 original = transform.position;
                Vector3 target = ticker.Target;
                Vector3[] pts = new Vector3[resolution];
                for (int i = 0; i < resolution; i++)
                {
                    if (i == 0)
                    {
                        pts[i] = original;
                        pts[i] += offset;

                    }
                    else if (i == resolution)
                    {
                        pts[i] = target;

                        pts[i] += offset;
                    }
                    else
                    {
                        pts[i] = ticker.GetPositionAt((float)i / (float)resolution);
                        pts[i] += offset;
                    }
                }
                lineRend.positionCount = pts.Length;
                lineRend.SetPositions(pts);
            }
          
        }
      

        protected virtual void OnDisable()
        {

        }

       
    }
}
