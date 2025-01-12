using Microsoft.Extensions.Options;
using Source.Models;

namespace Source;

public class Worker(IHostApplicationLifetime lifeTime, IOptions<Cheese> cheeseSettings) : IHostedService {
    public async Task StartAsync(CancellationToken token)
    {
        Console.WriteLine("Results:");
        Console.WriteLine($"Name: {cheeseSettings.Value.Name}");
        Console.WriteLine($"Price: {cheeseSettings.Value.Price}");
        Console.WriteLine($"Milk: {cheeseSettings.Value.Milk}");
        Console.WriteLine($"Flavours: {string.Join(", ", cheeseSettings.Value.Flavours)}");
        Console.WriteLine($"Origin location: {cheeseSettings.Value.Origin?.Location}");
        Console.WriteLine($"Origin farm: {cheeseSettings.Value.Origin?.Name}");

        await StopAsync(token);
    }

    public Task StopAsync(CancellationToken token)
    {
        lifeTime.StopApplication();
        return Task.CompletedTask;
    }
}