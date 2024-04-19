using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaTest.ViewModels;

    public class Units
    {

    private string  currentTempUnit = "°C";
    private string currentWindUnit = "m/s";

    private Units() { }
    private static Units _instance;
    public static Units GetInstance()
    {
        if (_instance == null)
        {
            _instance = new Units();
        }
        return _instance;
    }

    public string GetTempUnit()
    {
        return currentTempUnit;
    }
    public string GetWindUnit()
    {
        return currentWindUnit;
    }

    public double CalculateTemp(double temp)
    {    
        if (currentTempUnit == "°F")
        {
            return Math.Round(temp * 1.8 + 32, 2);
        }
        return temp;
    }
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
    public void ChangeTempUnit(string newUnit)
    {
        if (newUnit == "F")
        {
            currentTempUnit = "°F";
        }
        else currentTempUnit = "°C";
    }
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

