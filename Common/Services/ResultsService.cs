using Common.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Common.Services;

public class ResultsService(IHostApplicationLifetime lifeTime, IOptions<Cheese> cheeseSettings) : IHostedService {
    public async Task StartAsync(CancellationToken token)
    {
        Console.WriteLine("Results");
        Console.WriteLine("---------------");
        Console.WriteLine($"Name: {cheeseSettings.Value.Name}");
        Console.WriteLine($"Price: {cheeseSettings.Value.Price}");
        Console.WriteLine($"Milk: {cheeseSettings.Value.Milk}");
        Console.WriteLine($"Flavours: {string.Join(", ", cheeseSettings.Value.Flavours ?? [])}");
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