namespace Source.Services;

public class Worker(IHostApplicationLifetime lifeTime) : IHostedService {
    public async Task StartAsync(CancellationToken token)
    {
        await StopAsync(token);
    }

    public Task StopAsync(CancellationToken token)
    {
        lifeTime.StopApplication();
        return Task.CompletedTask;
    }
}