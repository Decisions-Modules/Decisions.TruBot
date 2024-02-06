using System.Net;
using System.Net.Http.Json;
using System.Web;
using Decisions.TruBot.Data;
using DecisionsFramework.Utilities.Data;
using Newtonsoft.Json;

namespace Decisions.TruBot;

public class TruBotRest
{
    public static string TruBotPost(string url, TruBotAuthentication authentication, JsonContent content)
    {
        HttpClient client = HttpClients.GetHttpClient(HttpClientAuthType.Normal);
        
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Add("UD-MS", authentication.sid);
        request.Headers.Add("Authorization", $"Bearer {authentication.token}");
        
        request.Content = content;
        
        HttpResponseMessage response = client.Send(request);
        response.EnsureSuccessStatusCode();

        Task<string> resultTask = response.Content.ReadAsStringAsync();
        resultTask.Wait();

        return resultTask.Result;
    }
    
    public static void TruBotDownload(string url, string destinationDirectory, string jobExecutionId, TruBotAuthentication authentication, JsonContent content)
    {
        HttpClient client = HttpClients.GetHttpClient(HttpClientAuthType.Normal);
        
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Add("UD-MS", authentication.sid);
        request.Headers.Add("Authorization", $"Bearer {authentication.token}");
        
        request.Content = content;
        
        HttpResponseMessage response = client.Send(request);
        response.EnsureSuccessStatusCode();

        string fileName = $"BotTransactionLog-{jobExecutionId}.csv";

        byte[] result = response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();

        using (FileStream stream = new FileStream($"{destinationDirectory}/{fileName}", FileMode.Create, FileAccess.Write))
        {
            stream.Write(result, 0, result.Length);
        }
    }
}