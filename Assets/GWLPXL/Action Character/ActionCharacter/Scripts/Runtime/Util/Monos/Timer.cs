namespace GWLPXL.ActionCharacter
{
    public class Timer : ITick
    {
        event System.Action onComplete;
        public float Duration = 0;

        public Timer(float duration, System.Action OnComplete)
        {
            onComplete = OnComplete;
            this.Duration = duration;
        }
        public void AddTicker()
        {
            TickManager.AddTicker(this);
        }

        public float GetTickDuration()
        {
            return Duration;
        }

        public void RemoveTicker()
        {
            TickManager.RemoveTicker(this);
        }

        public void Tick()
        {
            onComplete?.Invoke();
            RemoveTicker();
        }
    }
}
