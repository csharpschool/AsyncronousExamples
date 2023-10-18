using System.Collections.Concurrent;
using System.Diagnostics;

namespace AsyncronousExamples.Classes;

public class Asynchronous
{
    HttpServiceFileReader Reader { get; }
    const string CAT_PATH = "sample-data/cats.json";
    const string DOG_PATH = "sample-data/dogs.json";

    CancellationTokenSource tokenSource = new();

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

    public async Task<List<Animal>> GetWithCancellationAync()
    {
        tokenSource = new();
        var result = (await ReadJsonFromFile<Dog>(DOG_PATH)) ?? new List<Dog>();

        for (int i = 0; i < result.Count; i++)
        {
            if (tokenSource.Token.IsCancellationRequested) break;
            
            result[i].Race += " [ALTERED]";
            await Task.Delay(1000); // Fake elapsed time when fetching data
        }

        return result?.Cast<Animal>().ToList() ?? new List<Animal>();
    }

    public void Cancel() => tokenSource.Cancel();

    #region Multi-Threading with Parallel.Foreach
    long Factorial(int n)
    {
        long result = 1;
        for (int i = 1; i <= n; i++)
        {
            result *= i;
        }
        return result;
    }
    public (string ForEachDuration, string ParallelDuration) CalcFactorials(int start, int end)
    {
        Stopwatch stopwatch = new Stopwatch();

        // Using regular foreach
        stopwatch.Start();
        foreach (var number in Enumerable.Range(start, end - start + 1))
        {
            Factorial(number);
        }
        stopwatch.Stop();
        var forEachDuration = $"Time taken with foreach: {stopwatch.ElapsedMilliseconds} ms";
        stopwatch.Reset();

        // Using Parallel.ForEach
        stopwatch.Start();
        Parallel.ForEach(Enumerable.Range(start, end - start + 1), number =>
        {
            Factorial(number);
        });
        stopwatch.Stop();
        var parallelDuration = $"Time taken with Parallel.ForEach: {stopwatch.ElapsedMilliseconds} ms";

        return (forEachDuration, parallelDuration);
    }
    #endregion
}
