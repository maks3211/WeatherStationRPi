using LiveChartsCore.Defaults;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaTest.Interfaces
{
    public interface IDataBaseService
    {
        void InsertDataIntoTable(string tableName, DateTime date, double value);
        T GetValueFrom24HoursAgo<T>(string tableName);
        (T? MinValue, T? MaxValue) GetTodayMinMaxValue<T>(string tableName);
        void ReadDataFromTable(string location, string tableName, ObservableCollection<DateTimePoint> obs1, ObservableCollection<DateTimePoint> obs2);

      
    }
}
