using System;
using System.Collections.Generic;

namespace Forward;

public class EngineV1 : IEngine
{
    #region Temperature

    /// <summary>Current engine temperature</summary>
    public double Temperature { get; private set; }

    private readonly double ambientTemperature;

    #endregion

    #region StaticData

    private const int Inertia = 10;

    /// <summary>Piecewise linear function M from V</summary>
    private readonly List<(int torque, int rotateSpeed)> function = new()
    {
        (20, 0),
        (75, 75),
        (100, 150),
        (105, 200),
        (75, 250),
        (0, 300)
    };

    /// <summary>Coefficient of heating from torque</summary>
    /// <summary>Hm</summary>
    private const double CoeffHeatingFromTorque = 0.001;

    /// <summary>Coefficient of heating from rotate</summary>
    /// <summary>Hv</summary>
    private const double CoeffHeatFromRotate = 0.0001;

    /// <summary>Coefficient of cooling from engine and ambient temperature</summary>
    /// <summary>Hv</summary>
    private const double CoeffСoolFromEngineAmbientTemperature = 0.1;

    #endregion

    #region TorqueAndRotationSpeed

    private double torque; // M
    private double rotateSpeed; // V

    private double FunctionRotateSpeed => function[iteration].rotateSpeed;
    private double NextRotateSpeed => function[iteration + 1].rotateSpeed;

    private double FunctionTorque => function[iteration].torque;
    private double NextTorque => function[iteration + 1].torque;

    #endregion

    private double Acceleration => torque / Inertia;

    private int iteration;

    public EngineV1(double ambientTemperature)
    {
        Temperature = ambientTemperature;
        this.ambientTemperature = ambientTemperature;

        torque = function[0].torque;
        rotateSpeed = function[0].rotateSpeed;

        rotateSpeed += Acceleration;
    }

    public void UpdateTemperature()
    {
        rotateSpeed += Acceleration;

        if (iteration < function.Count - 2 && rotateSpeed > NextRotateSpeed) 
                iteration++;

        Interpolation();

        Temperature += Heat();
        Temperature += Cool();
    }
    
    private void Interpolation()
    {
        var dividend = rotateSpeed - FunctionRotateSpeed;
        var divisor = NextRotateSpeed - FunctionRotateSpeed;
        var factor = NextTorque + FunctionTorque;

        torque = dividend / divisor * factor + FunctionTorque;
    }

    private double Heat()
    {
        var rotateSpeedSquare = Math.Sqrt(rotateSpeed);
        return torque * CoeffHeatingFromTorque * rotateSpeedSquare * CoeffHeatFromRotate;
    }

    private double Cool() => CoeffСoolFromEngineAmbientTemperature * (ambientTemperature - Temperature);
}