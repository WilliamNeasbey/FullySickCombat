using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{
    public enum CustomCodeType
    {
        None = 0,
        Once = 100,
        Continuous = 200,

    }
    [System.Serializable]
    public class CustomCode
    {
        public virtual float GetStartPercent()
        {
            return startPercent;
        }
        public virtual float GetEndPercent()
        {
            return endPercent;
        }

        public virtual CustomCodeType GetCodeType()
        {
            return type;
        }
        public virtual void SetCustomCodeType(CustomCodeType newtype)
        {
            type = newtype;
        }
        public virtual void SetStartPercent(float newpercent)
        {
            startPercent = newpercent;
            if (startPercent < 0) startPercent = 0;
            else if (startPercent > 1) startPercent = 1;
        }

        public virtual void SetEndPercent(float newPercent)
        {
            endPercent = newPercent;
            if (endPercent < 0) endPercent = 0;
            else if (endPercent > 1) endPercent = 1;
        }
        [SerializeField]
        [Range(0f, 1f)]
        protected float startPercent = 0;
        [SerializeField]
        [Range(0f, 1f)]
        protected float endPercent = 1;
        [SerializeField]
        protected CustomCodeType type = CustomCodeType.Once;

       
    }

    public abstract class CustomActionCodeSO : ScriptableObject
    {
        [SerializeField]
        protected CustomCode customParameters;
        [System.NonSerialized]
        protected Dictionary<int, ActionTickerCC> trackingdic = new Dictionary<int, ActionTickerCC>();
        public abstract bool HasRequirements(ActionCharacter instance);

        public abstract void Tick(ActionTickerCC fromaction);

        public virtual void SetCustomCode(CustomCode newCustom)
        {
            customParameters = newCustom;
        }
        public virtual CustomCode GetCustomCode()
        {
            return customParameters;
        }


        public virtual void Initialize(ActionTickerCC fromaction)
        {
            trackingdic[fromaction.Character.ID] = fromaction;
            fromaction.Character.Events.OnActionEnded += CleanUp;
            
        }
        public virtual void CleanUp(CharacterActionArgs ctx)
        {
            if (trackingdic.ContainsKey(ctx.Instance.ID))
            {

                ctx.Instance.Events.OnActionEnded -= CleanUp;
                trackingdic.Remove(ctx.Instance.ID);
            }

        }

       
       

    }
}