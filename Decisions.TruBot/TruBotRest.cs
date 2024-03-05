using System.Net.Http.Json;
using DecisionsFramework.ServiceLayer;
using DecisionsFramework.Utilities.Data;

namespace Decisions.TruBot;

public class TruBotRest
{
    private static readonly TruBotSettings Settings = ModuleSettingsAccessor<TruBotSettings>.GetSettings();
    
    public static string TruBotPost(string url, JsonContent content)
    {
        HttpClient client = HttpClients.GetHttpClient(HttpClientAuthType.Normal);
        
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Add("UD-MS", Settings.Sid);
        request.Headers.Add("Authorization", $"Bearer {Settings.Token}");
        
        request.Content = content;
        
        HttpResponseMessage response = client.Send(request);
        response.EnsureSuccessStatusCode();

        Task<string> resultTask = response.Content.ReadAsStringAsync();
        resultTask.Wait();

        return resultTask.Result;
    }
    
    public static void TruBotDownload(string url, string destinationDirectory, string? jobExecutionId, JsonContent content)
    {
        HttpClient client = HttpClients.GetHttpClient(HttpClientAuthType.Normal);
        
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Add("UD-MS", Settings.Sid);
        request.Headers.Add("Authorization", $"Bearer {Settings.Token}");
        
        request.Content = content;
        
        HttpResponseMessage response = client.Send(request);
        response.EnsureSuccessStatusCode();
        
        string[] contentName = response.Content.Headers.ContentDisposition.FileName.Split(".");
        string fileName = contentName.First() + $"-{jobExecutionId}.{contentName.Last()}";

        byte[] result = response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
        
        using (FileStream stream = new FileStream(Path.Combine(destinationDirectory, fileName), FileMode.Create, FileAccess.Write))
        {
            stream.Write(result, 0, result.Length);
        }
    }
}