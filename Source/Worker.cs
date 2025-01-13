using Microsoft.Extensions.Options;
using Source.Models.CheeseModels;

namespace Source;

public class Worker(IHostApplicationLifetime lifeTime, IOptions<Cheese> cheeseSettings) : IHostedService {
    public async Task StartAsync(CancellationToken token)
    {
        Console.WriteLine("Results");
        Console.WriteLine("---------------");
        Console.WriteLine($"Name: {cheeseSettings.Value.Name}");
        Console.WriteLine($"Price: {cheeseSettings.Value.Price}");
        Console.WriteLine($"Milk: {cheeseSettings.Value.Milk}");
        var flavours = cheeseSettings.Value.Flavours?.Select(x => x.Description) ?? new List<string>();
        Console.WriteLine($"Flavours: {string.Join(", ", flavours)}");
        Console.WriteLine($"Origin location: {cheeseSettings.Value.Origin?.Location}");
        Console.WriteLine($"Origin farm: {cheeseSettings.Value.Origin?.Name}");
        Console.WriteLine("---------------");

        await StopAsync(token);
    }

    public Task StopAsync(CancellationToken token)
    {
        lifeTime.StopApplication();
        return Task.CompletedTask;
    }
}