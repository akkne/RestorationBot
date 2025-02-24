namespace RestorationBot.Telegram.Services.Implementation;

using Abstract;

public class PoolingService : BackgroundService
{
    private readonly ILogger<PoolingService> _logger;
    private readonly IReceiverService _receiverService;

    public PoolingService(ILogger<PoolingService> logger, IReceiverService receiverService)
    {
        _logger = logger;
        _receiverService = receiverService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("PoolingService is starting.");
        await StartReceivingAsync(stoppingToken);
    }

    private async Task StartReceivingAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
            try
            {
                await _receiverService.ReceiveAsync(stoppingToken);
            }
            catch (Exception exception)
            {
                _logger.LogError("Pooling failed with exception: {0}", exception.Message);
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
    }
}