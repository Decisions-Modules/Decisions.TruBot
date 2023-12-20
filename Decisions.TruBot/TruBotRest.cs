using System.Net.Http;
using System.Threading.Tasks;
using Decisions.TruBot.Data;

namespace Decisions.TruBot;

public class TruBotRest
{
    public static string TruBotGet(string url, TruBotAuthentication authentication)
    {
        HttpClient client = new HttpClient();
        
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("sid", authentication.sid);
        request.Headers.Add("Authorization", $"Bearer {authentication.token}");
        
        HttpResponseMessage response = client.Send(request);
        response.EnsureSuccessStatusCode();

        Task<string> resultTask = response.Content.ReadAsStringAsync();
        resultTask.Wait();

        return resultTask.Result;
    }

    public static string TruBotDelete(string url, TruBotAuthentication authentication)
    {
        HttpClient client = new HttpClient();

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, url);
        request.Headers.Add("sid", authentication.sid);
        request.Headers.Add("Authorization", $"Bearer {authentication.token}");
        
        HttpResponseMessage response = client.Send(request);
        response.EnsureSuccessStatusCode();
        
        Task<string> resultTask = response.Content.ReadAsStringAsync();
        resultTask.Wait();
        
        return resultTask.Result;
    }
}