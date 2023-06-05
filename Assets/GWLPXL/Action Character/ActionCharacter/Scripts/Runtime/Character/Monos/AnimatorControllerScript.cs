using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{
    public class AnimationClipOverrides : List<KeyValuePair<AnimationClip, AnimationClip>>
    {
        public AnimationClipOverrides(int capacity) : base(capacity) { }

        public AnimationClip this[string name]
        {
            get { return this.Find(x => x.Key.name.Equals(name)).Value; }
            set
            {
                int index = this.FindIndex(x => x.Key.name.Equals(name));
                if (index != -1)
                    this[index] = new KeyValuePair<AnimationClip, AnimationClip>(this[index].Key, value);
            }
        }
    }

    public interface IAnimatorController
    {
        void RemoveAllOverrides();
        void SetOverrides(List<AnimOverride> locooverrides );
        void RemoveLocoOverrides(List<AnimOverride> locooverrides );
        Animator GetAnimator();
        void SetAnimatorSpeed(float value);
        AnimatorStateInfo GetStateInfo(int layer = 0);
        void PlayCrossFadeFixed(string name, float crossfade, int layer = 0, float start = 0);

        void SetFloatParam(string name, float value);
        float GetFloatParam(string name);

        void SetIntParam(string name, int value);
        int GetIntParam(string name);

        void SetBoolParam(string name, bool value);
        bool GetBoolParam(string name);

        void SetTrigger(string name);
    }

    /// <summary>
    /// wrapper for all the anim calls the action character system makes
    /// using unity's animator override is a potential for performance concerns, may need to cache all possible overrides but currently not necessary
    /// </summary>
    public class AnimatorControllerScript : MonoBehaviour, IAnimatorController
    {

        protected Animator anim;
        protected RuntimeAnimatorController animCache;
        protected AnimatorOverrideController overrider;

        protected virtual void Awake()
        {
            anim = GetComponent<Animator>();
            animCache = anim.runtimeAnimatorController;
          
        }

   

        public virtual void RemoveLocoOverrides(List<AnimOverride> locooverrides)
        {
            if (overrider == null) return;

            if (locooverrides.Count > 0)
            {

                for (int i = 0; i < locooverrides.Count; i++)
                {
                    AnimOverride locooverride = locooverrides[i];
                    overrider[locooverride.OriginalClip.name] = locooverride.OriginalClip;
                }
            }
        }


        /// <summary>
        /// wow, performance issues with this either way
        /// </summary>
        /// <param name="locooverrides"></param>
        public virtual void SetOverrides(List<AnimOverride> locooverrides)
        {
            if (locooverrides.Count > 0)
            {
                if (overrider == null)
                {
                    string name = anim.runtimeAnimatorController.name;
                    overrider = new AnimatorOverrideController(anim.runtimeAnimatorController);
                    anim.runtimeAnimatorController = overrider;
                    overrider.name = name + "_Override";
                }

    

                for (int i = 0; i < locooverrides.Count; i++)
                {
                    AnimOverride locooverride = locooverrides[i];
                    overrider[locooverride.OriginalClip.name] = locooverride.Override;

                }
            }

        }

        public virtual AnimatorStateInfo GetStateInfo(int layer = 0)
        {
            return anim.GetCurrentAnimatorStateInfo(layer);
        }
        public virtual void PlayCrossFadeFixed(string name, float crossfade, int layer = 0, float start = 0)
        {
            if (string.IsNullOrWhiteSpace(name)) return;
          //  Debug.Log(name);

            if (anim != null)
            {
                anim.CrossFadeInFixedTime(name, crossfade, layer, start);
            }

        }
        public virtual void SetBoolParam(string name, bool value)
        {
            anim.SetBool(name, value);
        }

        public virtual void SetFloatParam(string name, float value)
        {
            anim.SetFloat(name, value);
        }

        public virtual void SetIntParam(string name, int value)
        {
            anim.SetInteger(name, value);
        }

        public virtual void SetTrigger(string name)
        {
            anim.SetTrigger(name);
        }

        public virtual float GetFloatParam(string name)
        {
            return anim.GetFloat(name);
        }

        public virtual int GetIntParam(string name)
        {
            return anim.GetInteger(name);
        }

        public virtual bool GetBoolParam(string name)
        {
            return anim.GetBool(name);
        }

        public void SetAnimatorSpeed(float value)
        {
            anim.speed = value;
        }

        public Animator GetAnimator()
        {
            return anim;
        }

        public void RemoveAllOverrides()
        {
            if (overrider == null) return;

            anim.runtimeAnimatorController = animCache;
            Destroy(overrider);
            overrider = null;
        }
    }
}