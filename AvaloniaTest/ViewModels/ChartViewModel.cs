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





namespace AvaloniaTest.ViewModels
{
    /// <summary>
    /// ViewModel for managing and displaying chart data from a MySQL database.
    /// </summary>
    public partial class ChartViewModel : ViewModelBase
    {
        MySqlConnection con;
        MySqlCommand cmd;
        MySqlDataReader reader;
        string sensorName;

        private readonly ObservableCollection<DateTimePoint> observableValues;
        private readonly ObservableCollection<DateTimePoint> observableValues2;
        /// <summary>
        /// Collection of trends to be displayed on the chart.
        /// </summary>
        public ObservableCollection<ISeries> Series { get; set; }
        /// <summary>
        /// Configuration for the X axes of the chart.
        /// </summary>
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
        /// <summary>
        /// Configuration for the title of the chart.
        /// </summary>
        public LabelVisual Title { get; set; } = new LabelVisual
        {
            Text = "Archiwum pomiarów",
            TextSize = 25,
            Padding = new LiveChartsCore.Drawing.Padding(15),
            Paint = new SolidColorPaint(SKColors.Black)
        };
        /// <summary>
        /// Configuration for the Y axes of the chart.
        /// </summary>
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
        /// <summary>
        /// Command handler for Altitude button.
        /// </summary>
        [RelayCommand]
        private void AltitudeHandler()
        {
            ReadDataFromTable("outer","Altitude", "Wysokość");
            ReadDataFromTable("inner","Altitude", "Wysokość");
        }
        /// <summary>
        /// Command handler forLuminance button.
        /// </summary>
        [RelayCommand]
        private void LuminanceHandler()
        {
            ReadDataFromTable("outer", "Luminance", "Luminacja");
            ReadDataFromTable("inner", "Luminance", "Luminacja");
        }
        /// <summary>
        /// Command handler for Temperature button.
        /// </summary>
        [RelayCommand]
        private void outerTempHandler()
        {
            ReadDataFromTable("outer","Temperature", "Temperatura");
            ReadDataFromTable("inner", "Temperature", "Temperatura");
        }
        /// <summary>
        /// Command handler for Preasure button.
        /// </summary>
        [RelayCommand]
        private void preasureHandler()
        {
            ReadDataFromTable("outer","Preasure", "Ciśnienie atmosferyczne");
            ReadDataFromTable("inner", "Preasure", "Ciśnienie atmosferyczne");
        }
        /// <summary>
        /// Command handler for Humidity button.
        /// </summary>
        [RelayCommand]
        private void humidityHandler()
        {
            ReadDataFromTable("outer","Humidity", "Wilgotność");
            ReadDataFromTable("inner", "Humidity", "Wilgotność");
        }
        /// <summary>
        /// Command handler for Co button.
        /// </summary>
        [RelayCommand]
        private void COHandler()
        {
            ReadDataFromTable("outer","Co", "CO");
            ReadDataFromTable("inner", "Co", "CO");
        }
        /// <summary>
        /// Command handler for No2 button.
        /// </summary>
        [RelayCommand]
        private void NO2Handler()
        {
            ReadDataFromTable("outer","No2", "NO2");
        }
        /// <summary>
        /// Command handler for NH3 button.
        /// </summary>
        [RelayCommand]
        private void NH3Handler()
        {
            ReadDataFromTable("outer","Nh3", "NH3");
        }


        /// <summary>
        /// Constructor for ChartViewModel.
        /// Creating object we connect with database.
        /// </summary>
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
               // string connString = "server=sql11.freesqldatabase.com ; uid=sql11704729 ; pwd=89jVjCtqzd ; database=sql11704729";
                string connString = "server=sql7.freesqldatabase.com ; uid=sql7733142 ; pwd=BANKMcx6Gt ; database=sql7733142";
                con = new MySqlConnection();
                con.ConnectionString = connString;
                con.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error connecting to the database: " + ex.Message);
            }

        }

        /// <summary>
        /// Reads data from the specified table and updates the chart.
        /// </summary>
        /// <param name="location">The location identifier for sensor</param>
        /// <param name="tableName">Part of name of the table to read from</param>
        /// <param name="axisName">The name to set for Y-axis</param>
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
                    //  string connString = "server=sql11.freesqldatabase.com ; uid=sql11704729 ; pwd=89jVjCtqzd ; database=sql11704729";
                    string connString = "server=sql7.freesqldatabase.com ; uid=sql7733142 ; pwd=BANKMcx6Gt ; database=sql7733142";
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
