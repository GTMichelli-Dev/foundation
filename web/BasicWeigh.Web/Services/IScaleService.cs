namespace BasicWeigh.Web.Services;

public interface IScaleService
{
    int GetCurrentWeight();
    bool IsInMotion();
    bool IsConnected();
}

public class SimulatedScaleService : IScaleService
{
    private int _simulatedWeight = 0;

    public int GetCurrentWeight() => _simulatedWeight;
    public bool IsInMotion() => false;
    public bool IsConnected() => true;

    public void SetWeight(int weight) => _simulatedWeight = weight;
}
