using Avalonia.Markup.Xaml.MarkupExtensions;
using AvaloniaTest.Helpers;
using Google.Protobuf.WellKnownTypes;
using LiveChartsCore.Defaults;
using MySql.Data.MySqlClient;
using ScottPlot.TickGenerators.TimeUnits;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;

namespace AvaloniaTest.Services
{
    public class DataBaseService
    {
        private string connString = "server=sql7.freesqldatabase.com ; uid=sql7733142 ; pwd=BANKMcx6Gt ; database=sql7733142";
        private readonly MySqlConnection connection;
        private MySqlCommand cmd;
        private  MySqlDataReader reader;
        public DataBaseService()
        {
            connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error connecting to the database: " + ex.Message);
            }
        }


        public void InsertDataIntoTable(string tableName, DateTime date, double value)
        {
            try
            {
                string insertQuery = $"INSERT INTO {tableName} (date, {tableName}) VALUES (@date, @value)";
                cmd = new MySqlCommand(insertQuery, connection);
                cmd.Parameters.AddWithValue("@date", date);
                cmd.Parameters.AddWithValue("@value", value);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while inserting data into the database: " + ex.Message);
            }
        }

        public T GetValueFrom24HoursAgo<T>(string tableName)
        {
            try
            {
                DateTime targetDateTime = DateTime.Now.AddHours(-24);
                DateTime startRange = targetDateTime.AddHours(-1); // Zakres -1 godzina
                DateTime endRange = targetDateTime.AddHours(1);    // Zakres +1 godzina

                // Zmodyfikowane zapytanie SQL
                string selectQuery = $"SELECT {tableName} FROM {tableName} WHERE date BETWEEN @startRange AND @endRange ORDER BY ABS(TIMESTAMPDIFF(SECOND, date, @targetDateTime)) LIMIT 1";

                using var cmd = new MySqlCommand(selectQuery, connection);
                cmd.Parameters.AddWithValue("@startRange", startRange);
                cmd.Parameters.AddWithValue("@endRange", endRange);
                cmd.Parameters.AddWithValue("@targetDateTime", targetDateTime);

                object result = cmd.ExecuteScalar(); // Pobranie pojedynczej wartości z bazy danych

                if (result != null && result != DBNull.Value)
                {
                    // Rzutowanie wyniku na typ T
                    return (T)Convert.ChangeType(result, typeof(T));
                }

              //  return GetErrorValues<T>(); // Zwraca wartość domyślną, jeśli wynik jest null
                return ErrorValues.GetErrorValue<T>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while retrieving data from the database: " + ex.Message);
                return ErrorValues.GetErrorValue<T>();
                //return GetErrorValues<T>(); // Zwraca wartość domyślną w przypadku błędu
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public (T? MinValue, T? MaxValue)  GetTodayMinMaxValue<T>(string tableName)
        {
            try
            {
                DateTime today = DateTime.Today; // Początek aktualnego dnia (00:00:00)
                DateTime tomorrow = today.AddDays(1); // Początek kolejnego dnia

                string selectQuery = $"SELECT MIN({tableName}), MAX({tableName}) FROM {tableName} WHERE date BETWEEN @today AND @tomorrow";


                using var cmd = new MySqlCommand(selectQuery, connection);
                cmd.Parameters.AddWithValue("@today", today);
                cmd.Parameters.AddWithValue("@tomorrow", tomorrow);

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    object minResult = reader[0];
                    object maxResult = reader[1];
                    T minValue;
                    T? maxValue;
                    if (minResult is System.DBNull)
                    {
                       // minValue = GetErrorValues<T>();
                        minValue = ErrorValues.GetErrorValue<T>();
                    }
                    else
                    {
                        //odczytano min
                         minValue = minResult != DBNull.Value ? (T)Convert.ChangeType(minResult, typeof(T)) : default;
                    }
                    if (maxResult is System.DBNull)
                    {
                       // maxValue = GetErrorValues<T>();
                        maxValue = ErrorValues.GetErrorValue<T>();
                      
                    }
                    else 
                    {
                        //odczytano max
                         maxValue = maxResult != DBNull.Value ? (T)Convert.ChangeType(maxResult, typeof(T)) : default;
                    }
                    
                    return (minValue, maxValue); // Zwrócenie wartości jako krotki (min, max)
                }

                return (default, default); // Zwraca domyślne wartości, jeśli nie ma wyników
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while retrieving data from the database: " + ex.Message);
                return (default, default); // Zwraca wartości domyślne w przypadku błędu
            }

        }




    }
}
