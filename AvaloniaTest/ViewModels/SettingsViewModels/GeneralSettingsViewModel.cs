using Avalonia.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaTest.ViewModels;

public partial class GeneralSettingsViewModel : ViewModelBase, INotifyPropertyChanged
{
    //[ObservableProperty]
    //private bool _celsiusSelected = true;
    //[ObservableProperty]
    //private bool _fahrenheitSelected = false;


    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }


    private bool _mSelected = (Units.GetInstance().GetWindUnit() == "m/s") ? true : false;
    private bool _kmSelected = (Units.GetInstance().GetWindUnit() == "km/h") ? true : false;
    private bool _ktSelected = (Units.GetInstance().GetWindUnit() == "kt") ? true : false;



  
    private bool _celsiusSelected = (Units.GetInstance().GetTempUnit() == "°C") ? true : false;
   // private bool _celsiusSelected = true;
    private bool _fahrenheitSelected = (Units.GetInstance().GetTempUnit() == "°F") ? true : false;

    public bool MSelected
    {
        get => _mSelected;
        set
        {
            if (_mSelected != value)
            {
                _mSelected = value;
                OnPropertyChanged(nameof(MSelected));
                // Tutaj możesz wywołać swoją metodę po zmianie wartości
                // np. MojaMetoda();
                if (value == true)
                {
                    Console.WriteLine("m");
                    Units.GetInstance().ChangeWindUnit("m");
                }  
            }
        }
    }

    public bool KmSelected
    {
        get => _kmSelected;
        set
        {
            if (_kmSelected != value)
            {
                _kmSelected = value;
                OnPropertyChanged(nameof(KmSelected));
                // Tutaj możesz wywołać swoją metodę po zmianie wartości
                // np. MojaMetoda();
                if (value == true)
                {
                    Console.WriteLine("km");
                    Units.GetInstance().ChangeWindUnit("km");
                }
               
            }
        }
    }
    public bool KtSelected
    {
        get => _ktSelected;
        set
        {
            if (_ktSelected != value)
            {
                _ktSelected = value;
                OnPropertyChanged(nameof(KtSelected));
                // Tutaj możesz wywołać swoją metodę po zmianie wartości
                // np. MojaMetoda();
                if (value == true)
                {
                    Console.WriteLine("kt");
                    Units.GetInstance().ChangeWindUnit("kt");
                }

            }
        }
    }
    public bool CelsiusSelected
    {
        get => _celsiusSelected;
        set
        {
            if (_celsiusSelected != value)
            {
                _celsiusSelected = value;
                OnPropertyChanged(nameof(CelsiusSelected));
                // Tutaj możesz wywołać swoją metodę po zmianie wartości
                // np. MojaMetoda();
                if (value == true)
                {
                    Units.GetInstance().ChangeTempUnit("C");
                }
  
            }
        }
    }

    
    public bool FahrenheitSelected
    {
        get => _fahrenheitSelected;
        set
        {
            if (_fahrenheitSelected != value)
            {
                _fahrenheitSelected = value;
                OnPropertyChanged(nameof(FahrenheitSelected));
                if (value == true)
                {
                    Units.GetInstance().ChangeTempUnit("F");
                }
            }
        }
    }
}
