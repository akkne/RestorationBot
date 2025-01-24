using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RestorationBot.Database.DataContext;
using RestorationBot.Handlers.Telegram.Callback;
using RestorationBot.Handlers.Telegram.Message;
using RestorationBot.Handlers.Telegram.Message.Command;
using RestorationBot.Helpers.Abstract;
using RestorationBot.Helpers.Implementation;
using RestorationBot.Services.Abstact;
using RestorationBot.Services.Implementation;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramUpdater;
using TelegramUpdater.Hosting;

IHostBuilder builder = Host.CreateDefaultBuilder(args);

builder.ConfigureHostConfiguration(ConfigureApp);
builder.ConfigureServices(ConfigureServices);

IHost host = builder.Build();
await host.RunAsync();
return;


void ConfigureApp(IConfigurationBuilder configuration)
{
    string environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ??
                         throw new Exception("No environment found");

    configuration
       .AddJsonFile("appsettings.json", false, false)
       .AddJsonFile($"appsettings.{environment}.json", true, false);
}

void ConfigureServices(HostBuilderContext context, IServiceCollection services)
{
    services.AddDbContext<ApplicationDbContext>(options =>
    {
        string connectionString = context.Configuration.GetConnectionString(nameof(ApplicationDbContext))
                               ?? throw new NullReferenceException("No connection string found.");

        options.UseNpgsql(connectionString)
               .EnableSensitiveDataLogging();
    });

    services.AddDbContext<ApplicationDbContext>();

    services.AddScoped<IUserRegistrationService, UserRegistrationService>();

    services.AddTransient<ICallbackGenerator, CallbackGenerator>();
    services.AddTransient<IRestorationStepMessageGenerator, RestorationStepMessageGenerator>();
    services.AddTransient<IExerciseMessageTextGenerator, ExerciseMessageTextGenerator>();

    ConfigureTelegramUpdater(context, services);
}

void ConfigureTelegramUpdater(HostBuilderContext context, IServiceCollection services)
{
    string token = context.Configuration.GetSection("TelegramBotToken").Value ??
                   throw new Exception("Server error: no telegram bot token configured");

    TelegramBotClient client = new(token);

    services.AddHttpClient("TelegramBotClient").AddTypedClient<ITelegramBotClient>(httpClient => client);

    User bot = client.GetMeAsync().Result;
    Console.WriteLine(bot.Username);

    UpdaterOptions updaterOptions = new(10,
        allowedUpdates: [UpdateType.Message, UpdateType.CallbackQuery]);

    services.AddTelegramUpdater(client, updaterOptions, botBuilder =>
    {
        botBuilder.AddDefaultExceptionHandler()
                  .AddScopedUpdateHandler<StartCommandMessageHandler, Message>()
                  .AddScopedUpdateHandler<StartTrainingCommandMessageHandler, Message>()
                  .AddScopedUpdateHandler<ScopedMessageHandler, Message>()
                  .AddScopedUpdateHandler<ScopedCallbackHandler, CallbackQuery>();
    });
}