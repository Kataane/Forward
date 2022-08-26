using System;
using System.Diagnostics;
using System.Threading;

namespace Forward;

public class Stand
{
    private readonly IEngine engine;

    /// <summary>Engine overheating temperature</summary>
    private const int Overheat = 110;

    public Stand(IEngine engine)
    {
        this.engine = engine;
    }

    public void Start(CancellationToken token = default)
    {
        if (!token.CanBeCanceled) token = new CancellationTokenSource(60000).Token;

        if (engine.Temperature >= Overheat)
            Throw($"Стартовая температура {engine.Temperature} двигателя превышает температуру перегрева {Overheat}");

        var watch = Stopwatch.StartNew();
        engine.UpdateTemperature();
        var delta = Overheat - engine.Temperature;

        while (delta > 0)
        {
            if (token.IsCancellationRequested) 
                break;
            engine.UpdateTemperature();
            delta = Overheat - engine.Temperature;
        }

        watch.Stop();

        Report(token.IsCancellationRequested, watch.ElapsedTicks, engine.Temperature);
    }
    
    private void Report(bool isCancelled, long ticks, double temperature)
    {
        if (isCancelled)
            CancelledReport(ticks, temperature);
        else
            NormalReport(ticks, temperature);
    }

    private void NormalReport(long ticks, double temperature)
    {
        Console.WriteLine("Двигатель дошел до температуры перегрева");
        Console.WriteLine($"Двигатель нагрелся до температуры : {temperature}");
        Console.WriteLine($"Прошло {ticks} секунд");
    }

    private void CancelledReport(long ticks, double temperature)
    {
        var message = "Испытание двигателя было отключено по времени. " + 
                      $"Стенд проработал {ticks} секунд. " + 
                      $"Двигатель нагрелся до температуры : {temperature}";
        Throw(message);
    }

    private static void Throw(string message)
    {
        throw new ArgumentException(message);
    }
}