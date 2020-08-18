using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace marlinX
{
    static class Load
    {
        private static readonly HttpClient client = new HttpClient();

        public static async Task<Dictionary<string, int>> LoadArticlesAsync(DateTime StartDate, DateTime EndDate,string source)
        {
            List<Task> tasks = new List<Task>();

            for (DateTime date = StartDate; date.Date <= EndDate.Date; date = date.AddDays(1))
            {
                tasks.Add(CallApi(date,source).ContinueWith(result=> SaveToDb(result.Result)));
            }

            await Task.WhenAll(tasks);

            return Db.getFromDatabase();
        }

        private static async Task<string> CallApi(DateTime date,string source)
        {
            try
            {
                string responseBody = await client.GetStringAsync($"http://newsapi.org/v2/everything?language=en&pageSize=100&sortBy=published At&sources={source}&from={date:YYYY-MM-DD}&to={date:YYYY-MM-DD}&apiKey=4ef9ec7a8516412095cb7fad0d40f699");
                return responseBody;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                return String.Empty;
            }
        }

        private static void SaveToDb(string responseBody)
        {
            ResponseModel response = JsonConvert.DeserializeObject<ResponseModel>(responseBody);

            //Convert the string into an array of words  
            IEnumerable<string> words = response.articles.SelectMany(x =>
                (x.title + " " + x.description).Split(new[] { '.', '?', '!', ' ', ';', ':', ',', '-', '\'' },
                    StringSplitOptions.RemoveEmptyEntries));

            Db.SaveToDatabase(words);
        }
    }

}
