using UnityEngine;

namespace GWLPXL.ActionCharacter
{
    /// <summary>
    /// mono abstract class to extend action enabled/disabled events. See DamageActionFX for example.
    /// </summary>
    public abstract class ActionSub : MonoBehaviour
    {
        [SerializeField]
        protected ActionCharacter charactercc;

       
        protected virtual void Awake()
        {
            if (charactercc == null)
            {
                charactercc = GetComponent<ActionCharacter>();
            }
        }


        protected virtual void OnEnable()
        {
            Subs();

        }

        protected virtual void Subs()
        {
            SubManager.SubActionStart(charactercc, ActionEnabled);
            SubManager.SubActionEnd(charactercc, ActionDisabled);
        }

        protected virtual void OnDisable()
        {
            Unsubs();

        }

        protected virtual void Unsubs()
        {
            SubManager.UnSubActionStart(charactercc, ActionEnabled);
            SubManager.UnSubActionEnd(charactercc, ActionDisabled);
        }

        /// <summary>
        /// override
        /// </summary>
        /// <param name="actionName"></param>
        protected virtual void ActionEnabled(CharacterActionArgs ctx)
        {


        }
        /// <summary>
        /// override
        /// </summary>
        /// <param name="actionName"></param>
        protected virtual void ActionDisabled(CharacterActionArgs ctx)
        {
    


        }
    }


}