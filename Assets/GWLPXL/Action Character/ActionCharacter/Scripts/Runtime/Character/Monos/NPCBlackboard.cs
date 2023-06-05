using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ActionCharacter
{
    [System.Serializable]
    public class NPCInput
    {
        public string Name;
        public int Slot;
        public bool Value;
        public NPCInput(string n, int s, bool v)
        {
            Name = n;
            Slot = s;
            Value = v;
        }
    }

    public interface INPCControl
    {
        float GetXAxisInput();
        float GetZAxisInputs();
        void SetActionSlotsTemplate(List<InputActionSlotSO> templates);
        void SetInput(float x, float z);
        void SetTarget(Transform target);
        void TriggerActionSlot(int slot);
        Transform GetTarget();
        void CollectInputs();
        void ConsumeInputs();
        void FireNPCControl();
        void FirePlayerControl();
    }

    public class NPCBBTIcker : ITick
    {
        INPCControl npc;
        public NPCBBTIcker(INPCControl npc)
        {
            this.npc = npc;
        }
        public void AddTicker()
        {
            TickManager.AddTicker(this);
        }

        public float GetTickDuration()
        {
            return Time.deltaTime;
        }

        public void RemoveTicker()
        {
            TickManager.RemoveTicker(this);
        }

        public void Tick()
        {
            npc.CollectInputs();
            
            
        }
    }
    /// <summary>
    /// example class that implements INPCCOntrol. Holds the data like a blackboard.
    /// </summary>
    public class NPCBlackboard : MonoBehaviour, INPCControl
    {
        public event System.Action<CharacterArgs> OnNPCTakeControl;
        public event System.Action<CharacterArgs> OnPlayerAssumeControl;
        public List<NPCInput> NPCInputs => inputs;
        public float XAxis;
        public float ZAxis;
        public Transform Target;
 

        [SerializeField]
        protected List<NPCInput> inputs = new List<NPCInput>();
        [SerializeField]
        protected NPCInput strafe;
        [SerializeField]
        protected NPCInput lockon;
        [SerializeField]
        protected List<InputRequirements> slots = new List<InputRequirements>();
        [SerializeField]
        protected InputWrapperSO wrapper = null;
        [SerializeField]
        protected InputActionMapSO actionMapSO = null;
        [SerializeField]
        protected ActionCharacter character = null;
        [SerializeField]
        protected bool takeOverOnStart = false;

        protected List<InputActionSlotSO> templates = new List<InputActionSlotSO>();
        protected bool npccontroler = false;
        protected NPCBBTIcker ticker;

        protected virtual void Awake()
        {
            if (character == null) character = GetComponent<ActionCharacter>();

        }
       
        protected virtual void OnDestroy()
        {
            DisableTicker();
        }
        protected virtual void ActionStarted(CharacterActionArgs args)
        {
            ConsumeInputs();
        }    

       protected virtual void Start()
        {
            if (takeOverOnStart)
            {
                CharacterInputs inputs = new CharacterInputs();
                inputs.ActionInputs = character.InputRequirements.GetSlots();
                inputs.InputWrapper = ScriptableObject.CreateInstance<InputNPCWrapperSO>();
                inputs.LockonInput = character.LockOn.Input;
                inputs.StrafeInput = character.Strafe.Input;
                PlayerCharacterManager.MakeNPCCOntrolled(character, inputs);

            }
        }    

        public virtual void EnableTicker()
        {
            DisableTicker();
            ticker = new NPCBBTIcker(this);
        }

        public virtual void DisableTicker()
        {
            if (ticker != null) ticker.RemoveTicker();
            ticker = null;
        }
        /// <summary>
        /// NPC has taken control, perform all the necessary logic
        /// </summary>
        public virtual void FireNPCControl()
        {
            if (ticker != null) ticker.RemoveTicker();
            ticker = null;
            ticker = new NPCBBTIcker(this);
            ticker.AddTicker();
            ConsumeInputs();
            character.SetPlayerControlled(false);
            CreateCopies();
            npccontroler = true;
            character.Events.OnActionStarted += ActionStarted;
            if (character.MovementRuntime != null)//change control to world
            {
                character.MovementRuntime.Movement.Standard.Locomotion.Locomotion.Movement.Reference = InputReference.World;
                character.MovementRuntime.Movement.Standard.Locomotion.Rotate.Rotation.Reference = InputReference.World;
            }

            EnableBrain();
            OnNPCTakeControl?.Invoke(new CharacterArgs(character));
        }

        protected virtual void EnableBrain()
        {
            NPC_Brain brain = GetComponent<NPC_Brain>();
            if (brain == null)
            {
                gameObject.AddComponent<NPC_Brain>();
            }
            brain.enabled = true;
        }

        protected virtual void DisableBrain()
        {
            NPC_Brain brain = GetComponent<NPC_Brain>();
            if (brain != null)
            {
                brain.enabled = false;
            }
           
        }

        /// <summary>
        /// player has taken control, run nec logic
        /// </summary>
        public virtual void FirePlayerControl()
        {
            if (ticker != null) ticker.RemoveTicker();
            ticker = null;
            ConsumeInputs();
            DisableBrain();
            character.SetPlayerControlled(true);
            npccontroler = false;
            character.Events.OnActionStarted -= ActionStarted;
            if (character.MovementRuntime != null)//reset back to templates
            {
                character.MovementRuntime.Movement.Standard.Locomotion.Locomotion.Movement.Reference =
                 character.MovementTemplate.Movement.Standard.Locomotion.Locomotion.Movement.Reference;
                character.MovementRuntime.Movement.Standard.Locomotion.Rotate.Rotation.Reference =
                    character.MovementTemplate.Movement.Standard.Locomotion.Rotate.Rotation.Reference;
            }

            slots.Clear();
            inputs.Clear();
            character.SetPlayerControlled(true);
            OnPlayerAssumeControl?.Invoke(new CharacterArgs(character));
        }
      

        public virtual void CollectInputs()
        {
            for (int i = 0; i < inputs.Count; i++)
            {
                InputSlot slot = character.InputRequirements.InputSlots[i];
                for (int j = 0; j < slot.Requirements.InputRequirements.InputButtons.Count; j++)
                {
                    slot.Requirements.InputRequirements.InputButtons[j].Value = inputs[i].Value;
                }
               
            }

            if (character.LockOn != null && character.LockOn.Input != null)
            {
                for (int i = 0; i < character.LockOn.Input.InputRequirements.InputButtons.Count; i++)
                {
                    character.LockOn.Input.InputRequirements.InputButtons[i].Value = lockon.Value;
                }
            }

            if (character.Strafe != null && character.Strafe.Input != null)
            {
                for (int i = 0; i < character.Strafe.Input.InputRequirements.InputButtons.Count; i++)
                {
                    character.Strafe.Input.InputRequirements.InputButtons[i].Value = strafe.Value;
                }
            }
           
            
        }

       
   
        public virtual void ConsumeInputs()
        {
            for (int i = 0; i < inputs.Count; i++)
            {
                inputs[i].Value = false;
            }

            if (lockon != null)
            {
                lockon.Value = false;
            }
      if (strafe != null)
            {
                strafe.Value = false;
            }


            for (int i = 0; i < inputs.Count; i++)
            {
                InputSlot slot = character.InputRequirements.InputSlots[i];
                for (int j = 0; j < slot.Requirements.InputRequirements.InputButtons.Count; j++)
                {
                    slot.Requirements.InputRequirements.InputButtons[j].Value = inputs[i].Value;
                }

            }

            if (character.LockOn != null && character.LockOn.Input != null)
            {
                for (int i = 0; i < character.LockOn.Input.InputRequirements.InputButtons.Count; i++)
                {
                    character.LockOn.Input.InputRequirements.InputButtons[i].Value = lockon.Value;
                }
            }

            if (character.Strafe != null && character.Strafe.Input != null)
            {
                for (int i = 0; i < character.Strafe.Input.InputRequirements.InputButtons.Count; i++)
                {
                    character.Strafe.Input.InputRequirements.InputButtons[i].Value = strafe.Value;
                }
            }

            InputBufferMap map = ActionManager.GetActionMap(character.ID);
            if (map != null)
            {
                map.InputSlotIndex = -1;
                map.ActionName = string.Empty;
            }
        }


        public virtual void SetActionSlotsTemplate(List<InputActionSlotSO> templates)
        {
            this.templates = templates;

        }

        protected virtual void CreateCopies()
        {
            slots = new List<InputRequirements>();
            inputs = new List<NPCInput>();
            List<InputActionSlotSO> _ = new List<InputActionSlotSO>();
            for (int i = 0; i < templates.Count; i++)
            {
                InputActionSlotSO copy = ScriptableObject.Instantiate(templates[i]);
                copy.Key = i;
                _.Add(copy);
                inputs.Add(new NPCInput(templates[i].name, i, false));
                slots.Add(copy.InputRequirements);
            }

            character.InputRequirements.SetInputs(_);

            InputActionSlotSO strafeai = ScriptableObject.CreateInstance<InputActionSlotSO>();
            strafeai.name = "Strafe_AI";
            strafeai.InputRequirements = new InputRequirements();
            strafeai.InputRequirements.MovementAxisRequirement = new InputAxisFreeForm(AxisMovementType.None);
            strafeai.InputRequirements.InputButtons = new List<InputButton>(1)
                {
                    new InputButton("Strafe_AI", ButtonType.Click)
                };
            character.SetStrafeInput(strafeai);
            lockon = new NPCInput(strafeai.name, 0, false);

            InputActionSlotSO lockonai = ScriptableObject.CreateInstance<InputActionSlotSO>();
            lockonai.name = "LockOn_AI";
            lockonai.InputRequirements = new InputRequirements();
            lockonai.InputRequirements.MovementAxisRequirement = new InputAxisFreeForm(AxisMovementType.None);
            lockonai.InputRequirements.InputButtons = new List<InputButton>(1)
                {
                    new InputButton("LockOn_AI", ButtonType.Click)
                };
            character.SetLockonInput(lockonai);
            strafe = new NPCInput(lockonai.name, 0, false);
        }

        public virtual void SetInput(float x, float z)
        {
            
            XAxis = x;
            ZAxis = z;

        }

        public virtual void SetTarget(Transform target)
        {
            Target = target;

        }

        public virtual void TriggerActionSlot(int slot)
        {

            slot = Mathf.Clamp(slot, 0, inputs.Count - 1);
            if (character.InputRequirements.InputSlots.Count <= 0)
            {
                Debug.Log("No inputs, can't trigger action");
                return;
            }

            if (inputs.Count <= 0)
            {
                Debug.Log("No inputs, can't trigger action");
                return;
            }
            List<string> actions = character.InputRequirements.InputSlots[slot].ActionList;
            for (int i = 0; i < actions.Count; i++)
            {
                ActionSO so = character.ActionsDatabase.Database.GetAction(actions[i]);
                if (ActionManager.CharacterActionRequirementsSuccess(character, so))
                {
                    if (slot <= inputs.Count - 1)
                    {
                        inputs[slot].Value = true;
                    }
                    else
                    {
                        Debug.LogWarning("Trying to perform ation at slot " + slot + " but not corresponding input found.");
                    }
                   
                }
               
            }

          
        }

        
        public virtual Transform GetTarget()
        {
            return Target;
        }

        public bool InControl()
        {
            return npccontroler;
        }

        public float GetXAxisInput()
        {
            return XAxis;
        }

        public float GetZAxisInputs()
        {
            return ZAxis;
        }
    }
}
