namespace BasicWeigh.Web.Services;

public interface IScaleService
{
    int GetCurrentWeight();
    bool IsInMotion();
    bool IsConnected();
    bool HasError();
}

public class SimulatedScaleService : IScaleService
{
    private int _simulatedWeight = 0;
    private bool _motion = false;
    private bool _error = false;

    /// <summary>
    /// Timestamp of the last weight update from an external scale (non-demo mode).
    /// Null until the first POST api/scale/weight call.
    /// </summary>
    public DateTime? LastUpdate { get; private set; }

    public int GetCurrentWeight() => _simulatedWeight;
    public bool IsInMotion() => _motion;
    public bool IsConnected() => !_error;
    public bool HasError() => _error;

    public void SetWeight(int weight) => _simulatedWeight = weight;
    public void SetMotion(bool motion) => _motion = motion;
    public void SetError(bool error) => _error = error;

    /// <summary>
    /// Mark that the scale just sent a fresh reading (resets the 5-second timeout).
    /// </summary>
    public void Touch() => LastUpdate = DateTime.UtcNow;
}
