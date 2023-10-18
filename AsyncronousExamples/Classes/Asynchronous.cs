using System.Diagnostics;
using System.Net.Http.Json;
using System.Reflection;
using System.Threading.Tasks.Sources;
using static AsyncronousExamples.Pages.FetchData;

namespace AsyncronousExamples.Classes;

public class Asynchronous
{
    const string CAT_PATH = "sample-data/cats.json";
    const string DOG_PATH = "sample-data/dogs.json";

    HttpServiceFileReader Reader { get; }

    public Asynchronous(HttpServiceFileReader reader) => Reader = reader;
    
    private async Task<List<T>?> ReadJsonFromFile<T>(string path)
    {
        return await Reader.ReadJsonFromFile<T>(path);
    }

    public async Task<List<Animal>> GetDogsAync()
    {
        await Task.Delay(3000); // Fake elapsed time when fetching data
        var result = await ReadJsonFromFile<Dog>(DOG_PATH);
        return result?.Cast<Animal>().ToList() ?? new List<Animal>();
    }

    public async Task<List<Animal>> GetCatsAync()
    {
        var result = await ReadJsonFromFile<Animal>(CAT_PATH);
        return result ?? new List<Animal>();
    }

    public async Task<List<Animal>> GetAllAync()
    {
        var dogTask = GetDogsAync();
        var catTask = GetCatsAync();
        await Task.WhenAll(catTask, dogTask);

        var result = new List<Animal>();
        if (catTask.Result is not null) result.AddRange(catTask.Result);
        if (dogTask.Result is not null) result.AddRange(dogTask.Result);

        return result;
    }

    public async Task<List<Animal>> GetAnyAync()
    {
        var dogTask = GetDogsAync();
        var catTask = GetCatsAync();
        var completedTask = await Task.WhenAny(dogTask, catTask);

        var result = new List<Animal>();
        if (completedTask.Result is not null) result.AddRange(completedTask.Result);

        return result;
    }
}
