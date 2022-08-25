using System;
using System.Globalization;
using Forward;

Console.WriteLine("Введите температуру окружающей среды:");
var input = Console.ReadLine()?.Replace(",", ".").Replace(" ", "");
var canConvert = double.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out var temperature);

if (!canConvert)
{
    Console.WriteLine("Введена неправильная температура");
    return;
}

var engine = new EngineV1(temperature);

var stand = new Stand(engine);

try
{
    stand.Start();
}
catch (Exception e)
{
    Console.WriteLine(e);
}

Console.ReadLine();
