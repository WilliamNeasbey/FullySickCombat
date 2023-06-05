using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{

    /// <summary>
    /// excamples creates preview instance of the action
    /// to do, return with the new improvements in mind.
    /// </summary>
    /// 

    public class FauxPlayerInstancePreview : MonoBehaviour
    {
        public ActionClone Instance;
        public string PreviewActionName = string.Empty;
        public KeyCode PreviewKey = KeyCode.F1;
        protected Dictionary<int, GameObject> previewDic = new Dictionary<int, GameObject>();
        protected ActionCharacter ac;
        protected virtual void Awake()
        {
            ac = GetComponent<ActionCharacter>();
        }
      
        protected virtual void Update()
        {
            if (Input.GetKeyDown(PreviewKey))
            {
                CreatePreview(transform.position, transform.rotation);
            }
        }
        protected virtual void CreatePreview(Vector3 pos, Quaternion rot)
        {
            string key = CommonFunctions.StringKey(PreviewActionName);
            ActionClone newPreview = Instantiate(Instance, pos, rot);
            newPreview.Config = ScriptableObject.Instantiate(ac.Config);
            newPreview.ActionsDatabase = ScriptableObject.Instantiate(ac.ActionsDatabase);
            newPreview.MovementTemplate = ScriptableObject.Instantiate(ac.MovementTemplate);
            newPreview.Loadout = ScriptableObject.Instantiate(ac.Loadout);
            newPreview.Initialization();
            newPreview.SetRotateInputVector(ac.GetRotationInputVector());
            newPreview.SetMovementInputVector(ac.GetInputMoveDirection());

            bool action = newPreview.TryStartActionSequence(PreviewActionName);//do a action chain
            if (action)
            {
                newPreview.Transform.rotation = rot;
                newPreview.Events.OnActionEnded += RemovePreview;
            }

        }

        protected virtual void RemovePreview(CharacterActionArgs ctx)
        {
            ctx.Instance.Events.OnActionEnded -= RemovePreview;

            Destroy(ctx.Instance.gameObject);


          
        }
    }
}