using Microsoft.Extensions.Options;
using Source.Models;

namespace Source.Services;

public class Worker(IHostApplicationLifetime lifeTime, IOptions<Cheese> cheeseSettings) : IHostedService {
    public async Task StartAsync(CancellationToken token)
    {
        Console.WriteLine($"Cheese name: {cheeseSettings.Value.Name}");

        await StopAsync(token);
    }

    public Task StopAsync(CancellationToken token)
    {
        lifeTime.StopApplication();
        return Task.CompletedTask;
    }
}