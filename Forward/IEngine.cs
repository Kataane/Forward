namespace Forward;

public interface IEngine
{
    public double Temperature { get; }

    public double CurrentTemperature();
}