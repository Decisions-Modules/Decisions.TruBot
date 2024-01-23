using System.Net.Http.Json;
using Decisions.TruBot.Data;

namespace Decisions.TruBot;

public class TruBotRest
{
    public static string TruBotPost(string url, TruBotAuthentication authentication, JsonContent content)
    {
        HttpClient client = new HttpClient();
        
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
}