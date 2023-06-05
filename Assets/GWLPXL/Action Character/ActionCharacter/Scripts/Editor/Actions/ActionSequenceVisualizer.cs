using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GWLPXL.ActionCharacter.Editor
{
    [System.Serializable]
    public class ActionGoals
    {
        public Vector3 Start;
        public Vector3 End;
        public ActionGoals(Vector3 start, Vector3 end)
        {
            Start = start;
            End = end;
        }
    }
    [System.Serializable]
    public class ActionTiming
    {
        public float StartTime;
        public float Duration;
        public ActionTiming(float start, float duration)
        {
            StartTime = start;
            Duration = duration;
        }
    }

    [System.Serializable]
    public class ComboTiming
    {
        public float StartTime = 0;
        public float EndTime;
        public ComboTiming(float end)
        {
            EndTime = end;
        }
    }

    [System.Serializable]
    public class ComboPlayback
    {
        public ComboTiming Timing;
        public List<ActionPlayback> Actions;
        public ComboPlayback(List<ActionPlayback> actions, ComboTiming timing)
        {
            Timing = timing;
            Actions = actions;
        }
    }
    [System.Serializable]
    public class ActionPlayback
    {
        public ActionSO ActionRef;
        public string ActionName;
        public ActionGoals Goals;
        public ActionTiming Timing;
        public List<Sequence> Sequences;

        public ActionPlayback(string name)
        {
            ActionName = name;
        }
    }
    [System.Serializable]
    public class ActionSequences
    {
        public List<Sequence> Sequences = new List<Sequence>();

    }
    [System.Serializable]
    public class Frame
    {
        public bool ScrubMarker = false;
        public List<string> HitGivers = new List<string>();
        public List<string> HitTakers = new List<string>();
        public int Value;
        public float TimeStamp;
        public Vector3 Position;
        public Frame(int frame, float timestamp, Vector3 pos)
        {
            TimeStamp = timestamp;
            Value = frame;
            Position = pos;
        }

    }

    [System.Serializable]
    public class Goals
    {
        public Vector3 Start;
        public Vector3 GoalX;
        public Vector3 GoalY;
        public Vector3 GoalZ;

        public Goals(Vector3 start, Vector3 x, Vector3 y, Vector3 z)
        {
            Start = start;
            GoalX = x;
            GoalY = y;
            GoalZ = z;
        }

    }

    [System.Serializable]
    public class Timings
    {
        public float StartTime;
        public float ClipLength;
        public float DeltaTime;
        public float Duration;
        public float XDur;
        public float YDur;
        public float ZDur;

        public Timings(float start, float d, float dt, float clip, float x, float y, float z)
        {
            StartTime = start;
            ClipLength = clip;
            DeltaTime = dt;
            Duration = d;
            XDur = x;
            YDur = y;
            ZDur = z;
        }
    }
    [System.Serializable]
    public class AnimEvent
    {
        public string Function;
        public string Parameter;
        public float Time;
        public AnimEvent(float t, string function, string para)
        {
            Function = function;
            Time = t;
            Parameter = para;
        }
    }

    [System.Serializable]
    public class Sequence
    {
        public List<AnimEvent> AnimEvents = new List<AnimEvent>();
        public ActionCCVars Vars;
        public Timings Timings;
        public Goals Goals;
        public Frame[] XFrames;
        public Frame[] YFrames;
        public Frame[] ZFrames;
        public Frame[] TotalFrames;
        public Sequence(Frame[] x, Frame[] y, Frame[] z, Frame[] toal, Goals goals, Timings timings, ActionCCVars vars)
        {
            Vars = vars;
            Goals = goals;
            Timings = timings;
            ZFrames = z;
            XFrames = x;
            YFrames = y;
            TotalFrames = toal;
        }
    }

    //to do
    //consider targeting or movement (and root animation)
    //break the sequence into segments, currently drawing just the first but dont consider additonal ones. 
    //need to determine ending position for sequence on next
    //how should sequences be shown?
    public enum FrameData
    {
        x,
        y,
        z,
        All
    }
    public static class ActionSequenceVisualizer
    {
        public static Dictionary<int, ActionPlayback> actorsequencedic = new Dictionary<int, ActionPlayback>();


        public static ComboPlayback CreateComboSequencePlayback(ActionCharacter character, List<ActionSO> actions, int fps, bool terriblehiearchy = false)
        {
            List<ActionPlayback> comboplayback = new List<ActionPlayback>(actions.Count);
            float rawtime = 0;
            Vector3 pos = character.transform.position;
            for (int i = 0; i < actions.Count; i++)
            {
                ActionSO action = actions[i];
       
                ActionPlayback playback = CreateSequencePlayback(pos, character, action, fps, terriblehiearchy);
    
                float start = 0;
                if (i > 0)
                {
                    pos = character.transform.position;
                    start = rawtime;
                }
                float duration = 0;
                for (int j = 0; j < playback.Sequences.Count; j++)
                {
                    duration += playback.Sequences[j].Timings.Duration;
                }
                
                rawtime += duration;

                Frame[] total = playback.Sequences[0].TotalFrames;

                Vector3 startPos = total[0].Position;
                total = playback.Sequences[playback.Sequences.Count - 1].TotalFrames;
                Vector3 endpos = total[total.Length - 1].Position;
                playback.Goals = new ActionGoals(startPos, endpos);
                playback.Timing = new ActionTiming(start, duration);
                pos = new Vector3(endpos.x, endpos.y, endpos.z);
                comboplayback.Add(playback);

            }

            ComboTiming timing = new ComboTiming(rawtime);
            ComboPlayback combo = new ComboPlayback(comboplayback, timing);
            return combo;
        }

        
        /// <summary>
        /// need to make a combo one, list of action playbacks
        /// </summary>
        /// <param name="character"></param>
        /// <param name="action"></param>
        /// <param name="fps"></param>
        /// <returns></returns>
        public static ActionPlayback CreateSequencePlayback(Vector3 start, ActionCharacter character, ActionSO action, int fps, bool terriblehiearchy = false)
        {
            List<ActionVars> sequence = action.GetSequenceVars();

            Animator anim = null;
            if (terriblehiearchy)
            {
                anim = character.GetComponentInChildren<Animator>();
            }
            ActionPlayback playback = new ActionPlayback(action.GetActionName());
            playback.ActionRef = action;
            float startime = 0;

            playback.Sequences = new List<Sequence>(sequence.Count);

            for (int i = 0; i < sequence.Count; i++)
            {

                ActionCCVars casted = sequence[i] as ActionCCVars;


                Distances x = casted.TravelX;
                Distances y = casted.TravelY;
                Distances z = casted.TravelZ;

                
                Goals goals = CollectGoals(character.transform, start, x, y, z);
                Timings timings = CalculateTimings(casted, startime, fps);
                Sequence newsequence = CreateSequence(casted, goals, timings, fps);
                newsequence.AnimEvents.Clear();

                AnimationEvent[] animevents = new AnimationEvent[0];
                if (casted.AnimatorVars.Clip != null)
                {
                    animevents = AnimationUtility.GetAnimationEvents(casted.AnimatorVars.Clip);
                }

                for (int j = 0; j < animevents.Length; j++)
                {
                    AnimEvent animevent = new AnimEvent(animevents[j].time * 1/casted.AnimatorVars.AnimatorSpeed, animevents[j].functionName, animevents[j].stringParameter);
                    newsequence.AnimEvents.Add(animevent);
                }

              

                playback.Sequences.Add(newsequence);

                //collect frames
                CalculateFrames(FrameData.z, newsequence, timings.DeltaTime, z.Curve, terriblehiearchy, anim);
                CalculateFrames(FrameData.y, newsequence, timings.DeltaTime, y.Curve, terriblehiearchy, anim);
                CalculateFrames(FrameData.x, newsequence, timings.DeltaTime, x.Curve, terriblehiearchy, anim);

                Vector3[] points;
                CalculateTotalSequenceFrames(start, timings.DeltaTime, newsequence, out points);


                start = points[points.Length - 1];
                startime += timings.Duration;



            }

            return playback;
        }
        
      
        public static void DrawCombo(List<ActionPlayback> actions, bool displayline = true, bool displayframes = false)
        {
            int framecount = 0;
            for (int i = 0; i < actions.Count; i++)
            {
                ActionPlayback action = actions[i];

                for (int j = 0; j < action.Sequences.Count; j++)
                {
                    Sequence sequence = action.Sequences[j];

                    Vector3[] points = new Vector3[sequence.TotalFrames.Length];
                    for (int k = 0; k < points.Length; k++)
                    {
                        points[k] = sequence.TotalFrames[k].Position;
                        framecount++;
                        if (displayframes)
                        {
                            UnityEditor.Handles.Label(points[k], "Frame " + framecount);// + '\n' +"Timestamp: " + sequence.TotalFrames[k].TimeStamp);
                            
                        }

                    }
                    if (displayline)
                    {
                        UnityEditor.Handles.DrawAAPolyLine(points);
                    }

                    UnityEditor.Handles.Label(action.Goals.End, "Sequence End");
                }

            }
        }
        

       
        

        public static void MoveCombo(ActionCharacter character, List<ActionPlayback> actions, float normalizedplayback)
        {
            float totald = 0;
            for (int i = 0; i < actions.Count; i++)
            {
                ActionPlayback action = actions[i];
                for (int j = 0; j < action.Sequences.Count; j++)
                {
                    totald += action.Sequences[j].Timings.Duration;
                }
            }

            float rawtime = totald * normalizedplayback;
            float convert = rawtime;

            for (int i = 0; i < actions.Count; i++)
            {
                ActionPlayback action = actions[i];

                float starttime = action.Timing.StartTime;
                if (rawtime >= starttime && rawtime < (starttime + action.Timing.Duration))
                {
                    //found action
                    PlayAction(character, action.Sequences, convert);
                    break;

                }
                else
                {
                    convert -= action.Timing.Duration;
                }

              //  Debug.Log(convert);
               
            }
        }

        public static void DrawHitBoxes(ActionCharacter character, ComboPlayback entireCombo)
        {
            DisableAllHitBoxes(character.GetComponent<ActorHitBoxes>());
            for (int i = 0; i < entireCombo.Actions.Count; i++)
            {
                ActionPlayback playback = entireCombo.Actions[i];
                for (int j = 0; j < playback.Sequences.Count; j++)
                {
                    DrawBoxes(character, playback.Sequences[j]);
                }
            }
        }

       

        static List<Frame> GetFrames(float starttime, float endtime, Sequence sequence)
        {
            List<Frame> frames = new List<Frame>();
            for (int i = 0; i < sequence.TotalFrames.Length; i++)
            {
                Frame frame = sequence.TotalFrames[i];
                if (frame.TimeStamp >= starttime && frame.TimeStamp <= endtime)
                {
                    frames.Add(frame);
                }
            }
            return frames;
        }
        static Frame GetFrame(float time, Sequence sequence)
        {
            for (int i = 0; i < sequence.TotalFrames.Length; i++)
            {
                Frame frame = sequence.TotalFrames[i];
                if (i == sequence.TotalFrames.Length - 1)
                {
                    if (time >= frame.TimeStamp)
                    {
                        return frame;
                    }
                    //last one
                }
                else
                {
   
                    if (time >= frame.TimeStamp && time < sequence.TotalFrames[i + 1].TimeStamp)
                    {
                        return frame;
                    }
                }
               
            }
            return null;
        }

        public static void DisableAllHitBoxes(ActorHitBoxes boxes)
        {
            for (int i = 0; i < boxes.HitBoxes.HitGivers.HitBoxesBase.HitBoxes.Count; i++)
            {
                boxes.HitBoxes.HitGivers.HitBoxesBase.HitBoxes[i].Collider.enabled = false;
            }
            for (int i = 0; i < boxes.HitBoxes.HitTakers.HitBoxesBase.HitBoxes.Count; i++)
            {
                boxes.HitBoxes.HitGivers.HitBoxesBase.HitBoxes[i].Collider.enabled = false;
            }
        }


        static void DrawBox(ActionCharacter character, List<ActivateHitBox> acboxes, Sequence sequence)
        {

            for (int i = 0; i < acboxes.Count; i++)
            {
                string boxname = acboxes[i].HitBoxName;
                List<Frame> frames = GetFrames(acboxes[i].EnableNormalizedTime * sequence.Timings.Duration, acboxes[i].DisableNormalizedTime * sequence.Timings.Duration, sequence);

                Vector3[] pts = new Vector3[frames.Count];
                Vector3 offset = new Vector3(0, 1, 0);
                for (int j = 0; j < frames.Count; j++)
                {
                    pts[j] = frames[j].Position + offset;
                }


                Handles.Label(pts[0], HitBoxDefaults.EnableHitGiver + " " + boxname);
                Handles.Label(pts[pts.Length - 1], HitBoxDefaults.DisableHitGiver + " " + boxname);
                Handles.color = Color.red;
                Handles.DrawPolyLine(pts);
                Handles.color = Color.white;


            }

        }

        static void AssignGiverBoxes(ActionCharacter character, List<ActivateHitBox> acboxes, Sequence sequence)
        {
            for (int i = 0; i < acboxes.Count; i++)
            {
                string boxname = acboxes[i].HitBoxName;
                List<Frame> frames = GetFrames(acboxes[i].EnableNormalizedTime * sequence.Timings.Duration, acboxes[i].DisableNormalizedTime * sequence.Timings.Duration, sequence);

                for (int j = 0; j < frames.Count; j++)
                {
                    frames[j].HitGivers.Add(boxname);
                }
               
            }
        }
        static void AssignTakerBoxes(ActionCharacter character, List<ActivateHitBox> acboxes, Sequence sequence)
        {
            for (int i = 0; i < acboxes.Count; i++)
            {
                string boxname = acboxes[i].HitBoxName;
                List<Frame> frames = GetFrames(acboxes[i].EnableNormalizedTime * sequence.Timings.Duration, acboxes[i].DisableNormalizedTime * sequence.Timings.Duration, sequence);

                for (int j = 0; j < frames.Count; j++)
                {
                    frames[j].HitTakers.Add(boxname);
                }

            }
        }

        public static void AssignBoxes(ActionCharacter character, Sequence sequence)
        {
            List<ActivateHitBox> acboxes = sequence.Vars.HitBoxOptions.GiverOptions.Boxes;
            AssignGiverBoxes(character, acboxes, sequence);

            acboxes = sequence.Vars.HitBoxOptions.TakerOptions.Boxes;
            AssignTakerBoxes(character, acboxes, sequence);
        }
        public static void DrawBoxes(ActionCharacter character, Sequence sequence)
        {
            AssignBoxes(character, sequence);

            List<ActivateHitBox> acboxes = sequence.Vars.HitBoxOptions.GiverOptions.Boxes;
            DrawBox(character, acboxes, sequence);


            acboxes = sequence.Vars.HitBoxOptions.TakerOptions.Boxes;
            DrawBox(character, acboxes, sequence);


        }
        public static void MoveAction(ActionCharacter character,  List<Sequence> sequences, float normalizedplayback)
        {
            float totald = 0;
            for (int i = 0; i < sequences.Count; i++)
            {
                totald += sequences[i].Timings.Duration;
            }
            float rawTime = totald * normalizedplayback;
            Debug.Log(rawTime);
            PlayAction(character, sequences, rawTime);

        }

        private static void PlayAction(ActionCharacter character, List<Sequence> sequences, float rawTime)
        {
            float convert = rawTime;
            for (int i = 0; i < sequences.Count; i++)
            {
                if (rawTime >= sequences[i].Timings.StartTime && rawTime < (sequences[i].Timings.StartTime + sequences[i].Timings.Duration))
                {
                    Sequence sequence = sequences[i];
                    if (sequence.Vars.AnimatorVars.Clip == null)
                    {
                        Debug.LogWarning("Clip at sequence is null, will preview from animator.");
                    }
                    else
                    {
                        Debug.Log("Found it " + "Raw " + rawTime + " Sequence " + sequence.Vars.AnimatorVars.Clip.name);

                    }
                    float norma = convert / sequence.Timings.Duration;
                    SetAnimationFrame(character.GetComponentInChildren<Animator>(), sequence.Vars.AnimatorVars.AnimatorStateName, sequence.Vars.AnimatorVars.Layer, norma);
                    for (int j = 0; j < sequence.TotalFrames.Length; j++)
                    {
                        if (sequence.TotalFrames[j].TimeStamp >= norma)
                        {
                            character.GetComponentInChildren<Animator>().transform.position = sequence.TotalFrames[j].Position;
                            break;
                        }
                    }
                    break;

                }
                else
                {
                    convert -= sequences[i].Timings.Duration;
                }


            }
        }

        public static void MoveSequence(ActionCharacter character, Sequence selected, float normalizedplayback)
        {

            float duration = selected.Timings.Duration;
            float rawTime = duration * normalizedplayback;
            PlaySequence(character, selected, rawTime);
        }

        private static void PlaySequence(ActionCharacter character, Sequence selected, float rawTime)
        {
            SetAnimationFrame(character.GetComponentInChildren<Animator>(), selected.Vars.AnimatorVars.AnimatorStateName, selected.Vars.AnimatorVars.Layer, rawTime);
            for (int i = 0; i < selected.TotalFrames.Length; i++)
            {
                if (selected.TotalFrames[i].TimeStamp >= rawTime)
                {
                    character.GetComponentInChildren<Animator>().transform.position = selected.TotalFrames[i].Position;
                    break;
                }
            }
        }

        public static void SetAnimationFrame(Animator anim, string animationName, int layer, float normalizedAnimationTime)
        {
            if (anim != null)
            {
                anim.speed = 0f;
                anim.Play(animationName, layer, normalizedAnimationTime);
                anim.Update(Time.deltaTime);
            }
        }

        public static Sequence CreateSequence(ActionCCVars casted, Goals goals, Timings timings, int framespersecond)
        {
            int thisSequenceFrames = Mathf.CeilToInt(timings.Duration * (float)framespersecond);
            int zframes = Mathf.CeilToInt(casted.TravelZ.Duration * thisSequenceFrames);
            if (casted.TravelZ.Distance == 0)
            {
                zframes = 0;
            }
            int yframes = Mathf.CeilToInt(casted.TravelY.Duration * thisSequenceFrames);
            if (casted.TravelY.Distance == 0)
            {
                yframes = 0;
            }
            int xframes = Mathf.CeilToInt(casted.TravelX.Duration * thisSequenceFrames);
            if (casted.TravelX.Distance == 0)
            {
                xframes = 0;
            }

            Sequence newsequence = new Sequence(new Frame[xframes], new Frame[yframes], new Frame[zframes], new Frame[thisSequenceFrames], goals, timings, casted);
            return newsequence;
        }

        public static Timings CalculateTimings(ActionCCVars casted, float start, int framespersecond)
        {
            float totald = 0;
            float c = 0;
            if (casted.AnimatorVars.Clip != null)
            {
                c = casted.AnimatorVars.Clip.length * (1 / casted.AnimatorVars.AnimatorSpeed);
                totald += c;
            }
            totald += casted.TimingOptions.AdditionalDuration;
            if (totald == 0)
            {
                totald = 1;
            }

            float xdur = totald * casted.TravelX.Duration;
            float ydur = totald * casted.TravelY.Duration;
            float zdur = totald * casted.TravelZ.Duration;
            float deltaTime = 1f / (float)framespersecond;
            Timings timings = new Timings(start, totald, deltaTime, c, xdur, ydur, zdur);
            return timings;
        }


       
        public static Goals CollectGoals(Transform transform, Vector3 start, Distances x, Distances y, Distances z)
        {
            Vector3 goalZ = start + transform.forward * z.Distance;
            Vector3 goalY = start + transform.up * y.Distance;
            Vector3 goalX = start + transform.right * x.Distance;


            Goals goals = new Goals(start, goalX, goalY, goalZ);
            return goals;
        }

        public static void CalculateTotalSequenceFrames(Vector3 start, float deltaTime, Sequence newsequence, out Vector3[] points)
        {
            points = new Vector3[newsequence.TotalFrames.Length];
            float timestamp = newsequence.Timings.DeltaTime;// + newsequence.Timings.StartTime;
            for (int j = 0; j < newsequence.TotalFrames.Length; j++)
            {
                newsequence.TotalFrames[j] = new Frame(j, timestamp, start);

                if (j < newsequence.ZFrames.Length)
                {
                    newsequence.TotalFrames[j].Position.z = newsequence.ZFrames[j].Position.z;
                }
                else
                {
                    if (newsequence.ZFrames.Length > 0)
                    {
                        newsequence.TotalFrames[j].Position.z = newsequence.ZFrames[newsequence.ZFrames.Length - 1].Position.z;
                    }
                }


                if (j < newsequence.YFrames.Length)
                {
                    newsequence.TotalFrames[j].Position.y = newsequence.YFrames[j].Position.y;
                }
                else
                {
                    if (newsequence.YFrames.Length > 0)
                    {
                        newsequence.TotalFrames[j].Position.y = newsequence.YFrames[newsequence.YFrames.Length - 1].Position.y;
                    }
                }


                if (j < newsequence.XFrames.Length)
                {
                    newsequence.TotalFrames[j].Position.x = newsequence.XFrames[j].Position.x;
                }
                else
                {
                    if (newsequence.XFrames.Length > 0)
                    {
                        newsequence.TotalFrames[j].Position.x = newsequence.XFrames[newsequence.XFrames.Length - 1].Position.x;
                    }
                }


                points[j] = newsequence.TotalFrames[j].Position;
                timestamp += deltaTime;

            }
        }

        public static Frame[] CalculateFrames(FrameData axis, Sequence sequence, float dt, AnimationCurve curve, bool terriblehiearchy = false, Animator anim = null)
        {
            float timestamp = dt;
            Frame[] frames = GetFrames(axis, sequence);

            for (int j = 0; j < frames.Length; j++)
            {

                float percent = (float)j / (float)frames.Length;
                if (curve != null)
                {
                    percent = curve.Evaluate(percent);
                }

                Vector3 pos = GetPosition(sequence, percent, axis);
                if (terriblehiearchy)
                {
                    pos -= anim.transform.position;
                }
                if (frames[j] == null)
                {
                    frames[j] = new Frame(j, timestamp, pos);
                }
                else
                {
                    frames[j].Value = j;
                    frames[j].Position = pos;
                    frames[j].TimeStamp = timestamp;
                }

                timestamp += dt;

            }
            return frames;
        }

        public static Frame[] GetFrames(FrameData axis, Sequence sequence)
        {
            Frame[] frames = new Frame[0];
            switch (axis)
            {
                case FrameData.z:
                    frames = sequence.ZFrames;
                    break;
                case FrameData.y:
                    frames = sequence.YFrames;
                    break;
                case FrameData.x:
                    frames = sequence.XFrames;
                    break;
                case FrameData.All:
                    frames = sequence.TotalFrames;
                    break;

            }

            return frames;
        }

        
        static Vector3 GetPosition(Sequence sequence, float percent, FrameData axis)
        {
            Vector3 goal = new Vector3(0, 0, 0);
            switch (axis)
            {
                case FrameData.z:
                    goal = sequence.Goals.GoalZ;
                    break;
                case FrameData.y:
                    goal = sequence.Goals.GoalY;
                    break;
                case FrameData.x:
                    goal = sequence.Goals.GoalX;
                    break;
            }

            return Vector3.Lerp(sequence.Goals.Start, goal, percent);
        }
    }

    public enum PlaybackFocus
    {
        None = 0,
        Combo = 100,
        Action = 200,
        Sequence = 300
    }

    /// <summary>
    /// it's okay, but doesnt need to be nested. just 1 for each thing that is selected.
    /// </summary>
    [System.Serializable]
    public class EntireActionPlayback
    {
        public PlaybackFocus Focus = PlaybackFocus.None;
        [HideInInspector]
        public string[] ActionChoices = new string[0];
        [HideInInspector]
        public string[] SequenceChoices = new string[0];
        [HideInInspector]
        public int ActionSelection = 0;
        [HideInInspector]
        public int SequenceSelection = 0;
        [Range(0f, 1f)]
        public float NormalizedPlayback = 0;
        public bool FrameData = false;
        public bool HitBoxData = false;
        [Tooltip("Is the Animator component on the root of the character? If so, enable this.")]
        public bool AnimatorOnRoot = false;
        [HideInInspector]
        public ComboPlayback Combo;
    }

    [System.Serializable]
    public class EntireComboPlaybackVars
    {
        public List<ActionPlayback> ActionSequences = new List<ActionPlayback>();
    }

    [System.Serializable]
    public class ActionSequencePlaybackVars
    {
        public ActionPlayback ActionPlayback;
    }


  
    

    

    
}
