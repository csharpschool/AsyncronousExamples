using System.Net.Http.Json;

namespace AsyncronousExamples.Classes;

public class HttpFileReader
{
    HttpClient Http { get; }

    public HttpFileReader(HttpClient http) => Http = http;

    private async Task<List<T>?> ReadJsonFromFile<T>(string path)
    {

        return await Http.GetFromJsonAsync<List<T>>(path);
    }
}

public class HttpServiceFileReader
{
    HttpClient? Http { get; set; }
    IServiceProvider ServiceProvider { get; }

    public HttpServiceFileReader(IServiceProvider serviceProvider)
    {  
        ServiceProvider = serviceProvider; 
    }

    HttpClient? GetHttpClientInstance()
    {
        var scope = ServiceProvider.CreateScope();
        Http = scope.ServiceProvider.GetRequiredService<HttpClient>();
        return Http;
        /*HttpClient? http;
        using (var scope = ServiceProvider.CreateScope())
        {
            http = scope.ServiceProvider.GetRequiredService<HttpClient>();
        }

        return http;*/
    }

    //public HttpServiceFileReader(HttpClient http) => Http = http;

    public async Task<List<T>?> ReadJsonFromFile<T>(string path)
    {
        var http = GetHttpClientInstance();
        return http is null 
            ? throw new HttpRequestException("No HTTP service available") 
            : await http.GetFromJsonAsync<List<T>>(path);
    }

}

