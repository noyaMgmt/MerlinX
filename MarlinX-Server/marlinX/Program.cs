using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace marlinX
{
    class Program
    {
        static async Task Main(string[] args)
        {

            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                return;
            }

            // Create a listener.
            HttpListener listener = new HttpListener();
            // Add the prefixes.
            listener.Prefixes.Add("http://127.0.0.1:8081/");
            JsonSerializer serializer = new JsonSerializer();
            listener.Start();
            Console.WriteLine("Listening...");
            while (true)
            {
                // Note: The GetContext method blocks while waiting for a request.
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;
                // Obtain a response object.
                HttpListenerResponse response = context.Response;
                response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:4200");
                byte[] buffer;

                if (request.QueryString.AllKeys.Contains("dateFrom") &&
                    request.QueryString.AllKeys.Contains("dateTo") && 
                    request.QueryString.AllKeys.Contains("source"))
                {
                    DateTime fromDate = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(request.QueryString["dateFrom"])).LocalDateTime;
                    DateTime toDate = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(request.QueryString["dateTo"])).LocalDateTime;

                    // Construct a response.
                    var result = await Load.LoadArticlesAsync(fromDate, toDate, request.QueryString["source"]);

                    response.ContentType = "application/json";
                    string responseString = JsonConvert.SerializeObject(result);
                    buffer = Encoding.UTF8.GetBytes(responseString);
                    // Get a response stream and write the response to it.
                    response.ContentLength64 = buffer.Length;
                }
                else
                {
                    buffer = Encoding.UTF8.GetBytes("missing parameter");
                    // Get a response stream and write the response to it.
                    response.ContentLength64 = buffer.Length;
                    response.StatusCode = 400;
                }

                System.IO.Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                // You must close the output stream.
                output.Close();
            }

        }
    }
}
