using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaTest.ViewModels;

/// <summary>
/// Class for managing units used in weather data.
/// </summary>
public class Units
    {

    private string  currentTempUnit = "°C";
    private string currentWindUnit = "m/s";
    private static Units _instance;

    /// <summary>
    /// Method to get the singleton instance of the Units class.
    /// </summary>
    /// <returns>Metod returns an instance of singleton object</returns>
    public static Units GetInstance()
    {
        if (_instance == null)
        {
            _instance = new Units();
        }
        return _instance;
    }

    /// <summary>
    /// Method to get the current temperature unit.
    /// </summary>
    /// <returns>Method returns current temperature unit</returns>
    public string GetTempUnit()
    {
        return currentTempUnit;
    }

    /// <summary>
    /// Method to get the current wind unit.
    /// </summary>
    /// <returns>Method returns current wind unit</returns>
    public string GetWindUnit()
    {
        return currentWindUnit;
    }

    /// <summary>
    /// Method to calculate temperature based on the current unit.
    /// </summary>
    /// <returns>Method returns calculated temperature acording to current unit.</returns>
    public double CalculateTemp(double temp)
    {    
        if (currentTempUnit == "°F")
        {
            return Math.Round(temp * 1.8 + 32, 2);
        }
        return temp;
    }

    /// <summary>
    /// Method to calculate wind based on the current unit.
    /// </summary>
    /// <returns>Method returns calculated wind acording to current unit.</returns>
    public double CalculatWind(double speed)
    {
        if (currentWindUnit == "m/s")
        { 
            return speed; 
        }
        if (currentWindUnit == "km/h")
        {
            return Math.Round(speed *3.6, 2);
        }
        return Math.Round(speed * 0.514, 2);

    }
    /// <summary>
    /// Method to change the temperature unit.
    /// </summary>
    /// <param name="newUnit">New unit to be set</param>
    public void ChangeTempUnit(string newUnit)
    {
        if (newUnit == "F")
        {
            currentTempUnit = "°F";
        }
        else currentTempUnit = "°C";
    }

    /// <summary>
    /// Method to change the wind unit.
    /// </summary>
    /// <param name="newUnit">New unit to be set</param>
    public void ChangeWindUnit(string newUnit)
    {
        if (newUnit == "km")
        {
            currentWindUnit = "km/h";
        }
        else if (newUnit == "m")
        {
            currentWindUnit = "m/s";
        }
        else
        {
            currentWindUnit = "kt";
        }
    }


}

