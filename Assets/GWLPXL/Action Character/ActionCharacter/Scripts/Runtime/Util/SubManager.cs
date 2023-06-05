using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace GWLPXL.ActionCharacter
{
    
    public static class SubManager
    {
        public static void SubGiveDamage(ActorHitBoxes boxes, Action<HitContextTrigger> callback)
        {
            if (boxes == null)
            {
                Debug.LogWarning("Trying to sub to boxes but boxes is null");
                return;
            }
            boxes.HitBoxes.HitGivers.HitSender.DamageEvents.OnHitTrigger += callback;
        }
        public static void UnSubGiveDamage(ActorHitBoxes boxes, Action<HitContextTrigger> callback)
        {
            if (boxes == null)
            {
                Debug.LogWarning("Trying to unsub from boxes but boxes is null");
                return;
            }
            boxes.HitBoxes.HitGivers.HitSender.DamageEvents.OnHitTrigger -= callback;
        }
        public static void SubGiveDamage(ActorHitBoxes boxes, Action<HitContextCollision> callback)
        {
            if (boxes == null)
            {
                Debug.LogWarning("Trying to sub to boxes but boxes is null");
                return;
            }
            boxes.HitBoxes.HitGivers.HitSender.DamageEvents.OnHitCollision += callback;
        }
        public static void UnSubGiveDamage(ActorHitBoxes boxes, Action<HitContextCollision> callback)
        {
            if (boxes == null)
            {
                Debug.LogWarning("Trying to unsub from boxes but boxes is null");
                return;
            }
            boxes.HitBoxes.HitGivers.HitSender.DamageEvents.OnHitCollision -= callback;
        }
        public static void SubHitTakerDisabled(ActorHitBoxes boxes, Action<HitBoxArgs> callback)
        {
            if (boxes == null)
            {
                Debug.LogWarning("Trying to sub to boxes but boxes is null");
                return;
            }
            boxes.HitBoxes.HitTakers.OnHitTakerDisabled += callback;
        }
        public static void UnSubHitTakerDisabled(ActorHitBoxes boxes, Action<HitBoxArgs> callback)
        {
            if (boxes == null)
            {
                Debug.LogWarning("Trying to unsub from boxes but boxes is null");
                return;
            }
            boxes.HitBoxes.HitTakers.OnHitTakerDisabled -= callback;
        }
        public static void SubHitTakerEnabled(ActorHitBoxes boxes, Action<HitBoxArgs> callback)
        {
            if (boxes == null)
            {
                Debug.LogWarning("Trying to sub to boxes but boxes is null");
                return;
            }
            boxes.HitBoxes.HitTakers.OnHitTakerEnabled += callback;
        }
        public static void UnSubHitTakerEnabled(ActorHitBoxes boxes, Action<HitBoxArgs> callback)
        {
            if (boxes == null)
            {
                Debug.LogWarning("Trying to unsub from boxes but boxes is null");
                return;
            }
            boxes.HitBoxes.HitTakers.OnHitTakerEnabled -= callback;
        }
        public static void SubHitGiverDisabled(ActorHitBoxes boxes, Action<HitBoxArgs> callback)
        {
            if (boxes == null)
            {
                Debug.LogWarning("Trying to sub to boxes but boxes is null");
                return;
            }
            boxes.HitBoxes.HitGivers.OnHitGiverDisabled += callback;
        }
        public static void UnSubHitGiverDisabled(ActorHitBoxes boxes, Action<HitBoxArgs> callback)
        {
            if (boxes == null)
            {
                Debug.LogWarning("Trying to unsub from boxes but boxes is null");
                return;
            }
            boxes.HitBoxes.HitGivers.OnHitGiverDisabled -= callback;
        }
        public static void SubHitGiverEnabled(ActorHitBoxes boxes, Action<HitBoxArgs> callback)
        {
            if (boxes == null)
            {
                Debug.LogWarning("Trying to sub to boxes but boxes is null");
                return;
            }
            boxes.HitBoxes.HitGivers.OnHitGiverEnabled += callback;
        }
        public static void UnSubHitGiverEnabled(ActorHitBoxes boxes, Action<HitBoxArgs> callback)
        {
            if (boxes == null)
            {
                Debug.LogWarning("Trying to unsub from boxes but boxes is null");
                return;
            }
            boxes.HitBoxes.HitGivers.OnHitGiverEnabled -= callback;
        }
        public static void SubTakeHit(ActorHitBoxes boxes, Action<HitContextCollision> callback)
        {
            if (boxes == null)
            {
                Debug.LogWarning("Trying to sub to boxes but boxes is null");
                return;
            }
            boxes.HitBoxes.HitTakers.HitReceiver.HitTakerEvents.OnHitCollision += callback;
        }
        public static void SubTakeHit(ActorHitBoxes boxes, Action<HitContextTrigger> callback)
        {
            if (boxes == null)
            {
                Debug.LogWarning("Trying to sub to boxes but boxes is null");
                return;
            }
            boxes.HitBoxes.HitTakers.HitReceiver.HitTakerEvents.OnHitTrigger += callback;
        }
        public static void UnSubTakeHit(ActorHitBoxes boxes, Action<HitContextTrigger> callback)
        {
            if (boxes == null)
            {
                Debug.LogWarning("Trying to unsub from boxes but boxes is null");
                return;
            }
            boxes.HitBoxes.HitTakers.HitReceiver.HitTakerEvents.OnHitTrigger -= callback;
        }
        public static void UnSubTakeHit(ActorHitBoxes boxes, Action<HitContextCollision> callback)
        {
            if (boxes == null)
            {
                Debug.LogWarning("Trying to unsub to boxes but boxes is null");
                return;
            }
            boxes.HitBoxes.HitTakers.HitReceiver.HitTakerEvents.OnHitCollision -= callback;
        }

        public static void SubActionEnd(ActionCharacter character, Action<CharacterActionArgs> callback)
        {
            if (character == null)
            {
                Debug.LogWarning("Trying to sub to character but character is null");
                return;
            }
            character.Events.OnActionEnded += callback;
        }
        public static void UnSubActionEnd(ActionCharacter character, Action<CharacterActionArgs> callback)
        {
            if (character == null)
            {
                Debug.LogWarning("Trying to unsub from character but character is null");
                return;
            }
            character.Events.OnActionEnded -= callback;
        }
        
        public static void SubActionStart(ActionCharacter character, Action<CharacterActionArgs> callback)
        {
            if (character == null)
            {
                Debug.LogWarning("Trying to sub to character but character is null");
                return;
            }
            character.Events.OnActionStarted += callback;
        }
        public static void UnSubActionStart(ActionCharacter character, Action<CharacterActionArgs> callback)
        {
            if (character == null)
            {
                Debug.LogWarning("Trying to unsub from character but character is null");
                return;
            }
            character.Events.OnActionStarted -= callback;
        }

    }
}
