using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static AvaloniaTest.Models.WeatherForecast.WeatherForecast;
using static System.Net.WebRequestMethods;

namespace AvaloniaTest.Models.AddressSearch
{
    public class AddressSearchController
    {
        private string APIkey = "pk.4344bbc28696098166b439d8c6efea0b";
        private HttpClient web;
        private string url;
        private string searchString;
        public List<Root> SearchResults;
        public AddressSearchController()
        {
            web = new HttpClient();
            url = string.Format("https://api.locationiq.com/v1/autocomplete?key={0}", APIkey);
        }

        public async Task<List<Root>> Search(string name)
        {

            try
            {
             
                string fullUrl = $"{url}&q={name}&accept-language=pl";
                var json = await web.GetStringAsync(fullUrl); 
                if (string.IsNullOrEmpty(json))
                {
                    return null;
                }
                SearchResults = JsonConvert.DeserializeObject<List<Root>>(json);
                Console.WriteLine($"Znaleziono: {SearchResults.Count()} miast");
                return SearchResults;
         
            }
            catch (Exception ex)
            {
                // Zaloguj wyjątek lub obsłuż go w inny sposób
                Console.WriteLine($"Error occurred: {ex.Message}");
                return null;
            }


        }
    }
}
