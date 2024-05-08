using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using SkiaSharp;
using System.Threading;
using Avalonia.Interactivity;
using Tmds.DBus.Protocol;
using CommunityToolkit.Mvvm.Input;
using MySql.Data.MySqlClient;
using Tmds.DBus;
using System.Data;
using LiveChartsCore.Defaults;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using ScottPlot.Statistics;
using Microsoft.CodeAnalysis.CSharp.Syntax;




namespace AvaloniaTest.ViewModels
{
    public partial class ChartViewModel : ViewModelBase
    {
        MySqlConnection con;
        MySqlCommand cmd;
        MySqlDataReader reader;
        string sensorName;

        private readonly ObservableCollection<DateTimePoint> observableValues;
        private readonly ObservableCollection<DateTimePoint> observableValues2;

        public ObservableCollection<ISeries> Series { get; set; }

        public Axis[] XAxes { get; set; }
            = new Axis[]
            {
                new DateTimeAxis(TimeSpan.FromHours(1),  date => date.ToString("yyyy-MM-dd HH:mm:ss"))
                {
                    Name = "Data",
                    NameTextSize = 23,
                    NamePaint = new SolidColorPaint(SKColors.Black),
                    LabelsPaint = new SolidColorPaint(SKColors.Black),
                    //LabelsRotation = 15,
                    TextSize = 15
                }
            };

        public LabelVisual Title { get; set; } = new LabelVisual
        {
            Text = "Archiwum pomiarów",
            TextSize = 25,
            Padding = new LiveChartsCore.Drawing.Padding(15),
            Paint = new SolidColorPaint(SKColors.Black)
        };

        public Axis[] YAxes { get; set; } = new Axis[]
        {
            new Axis
            {
                Name = "Temperatura",
                NameTextSize = 23,
                NamePaint = new SolidColorPaint(SKColors.Black),
                LabelsPaint = new SolidColorPaint(SKColors.Black),
                TextSize = 15
            }
        };

        [RelayCommand]
        private void AltitudeHandler()
        {
            ReadDataFromTable("outer","Altitude", "Wysokość");
            ReadDataFromTable("inner","Altitude", "Wysokość");
        }

        [RelayCommand]
        private void LuminanceHandler()
        {
            ReadDataFromTable("outer", "Luminance", "Luminacja");
            ReadDataFromTable("inner", "Luminance", "Luminacja");
        }

        [RelayCommand]
        private void outerTempHandler()
        {
            ReadDataFromTable("outer","Temperature", "Temperatura");
            ReadDataFromTable("inner", "Temperature", "Temperatura");
        }

        [RelayCommand]
        private void preasureHandler()
        {
            ReadDataFromTable("outer","Preasure", "Ciśnienie atmosferyczne");
            ReadDataFromTable("inner", "Preasure", "Ciśnienie atmosferyczne");
        }
        [RelayCommand]
        private void humidityHandler()
        {
            ReadDataFromTable("outer","Humidity", "Wilgotność");
            ReadDataFromTable("inner", "Humidity", "Wilgotność");
        }
        [RelayCommand]
        private void COHandler()
        {
            ReadDataFromTable("outer","Co", "CO");
            ReadDataFromTable("inner", "Co", "CO");
        }
        [RelayCommand]
        private void NO2Handler()
        {
            ReadDataFromTable("outer","No2", "NO2");
        }
        [RelayCommand]
        private void NH3Handler()
        {
            ReadDataFromTable("outer","Nh3", "NH3");
        }



        public ChartViewModel()
        {
            observableValues = new ObservableCollection<DateTimePoint> {};
            observableValues2 = new ObservableCollection<DateTimePoint> { };

            Series = new ObservableCollection<ISeries>
            {
                new LineSeries<DateTimePoint>
                {
                    Name = "czujnik wewnętrzny",
                    Values = observableValues,
                    Fill = null,
                    LineSmoothness = 1,
                    Stroke = new SolidColorPaint(SKColors.Blue, 6),
                    GeometrySize = 0,
                    GeometryStroke = null
                },
                new LineSeries<DateTimePoint>
                {
                    Name = "czujnik zewnętrzny",
                    Values = observableValues2,
                    Fill = null,
                    LineSmoothness = 1,
                    Stroke = new SolidColorPaint(SKColors.CornflowerBlue, 6),
                    GeometrySize = 0,
                    GeometryStroke = null
               
                }
            };

            try
            {
                string connString = "server=sql11.freesqldatabase.com ; uid=sql11704729 ; pwd=89jVjCtqzd ; database=sql11704729";
                con = new MySqlConnection();
                con.ConnectionString = connString;
                con.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error connecting to the database: " + ex.Message);
            }

        }


        private void ReadDataFromTable(string location, string tableName, string axisName)
        {
            if (location == "outer")
            {
                observableValues.Clear();
                observableValues2.Clear();
                var axis = YAxes[0];
                axis.Name = axisName;
                axis.NamePaint = new SolidColorPaint(SKColors.Black);
            }
            sensorName = location + tableName;
            if (con.State != System.Data.ConnectionState.Open)
            {
                try
                {
                    string connString = "server=sql11.freesqldatabase.com ; uid=sql11704729 ; pwd=89jVjCtqzd ; database=sql11704729";
                    con = new MySqlConnection();
                    con.ConnectionString = connString;
                    con.Open();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error connecting to the database: " + ex.Message);
                }
            }
            else
            {
                try
                {
                    string sql = $"SELECT * FROM {sensorName} ORDER BY date ASC";
                    cmd = new MySqlCommand(sql, con);
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        if (location == "inner")
                            observableValues.Add(new DateTimePoint((DateTime)reader["date"], (double)reader[sensorName]));
                        else
                            observableValues2.Add(new DateTimePoint((DateTime)reader["date"], (double)reader[sensorName]));
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error while handling with database: " + ex.Message);
                }

            }
        }



        }
    }
