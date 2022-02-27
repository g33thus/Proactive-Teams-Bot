using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NotificationTrigger
{
    public class NotifyFunction
    {
        [FunctionName("NotifyFunction")]
        //5-7 * * * * *
        //0 30 9 * * 1-5 - imp
        public static async Task Run([TimerTrigger("5-7 * * * * *")] TimerInfo myTimer, ILogger log)
        {
            try
            {
                // Read from the user preference store to determine where to send notifications
                //GET LIST OF USERS
                string notifyUrl = "http://localhost:3978/" + "api/notify";
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Clear();
                var result = client.GetAsync(notifyUrl).Result;

                await Task.Yield();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not send event to bot due to exception: {ex.Message}");
            }
        }
    }
}
