using Microsoft.EntityFrameworkCore;
using RestorationBot.Database.DataContext;
using RestorationBot.Extensions;
using RestorationBot.Helpers.Abstract;
using RestorationBot.Helpers.Implementation;
using RestorationBot.Services.Abstract;
using RestorationBot.Services.Implementation;
using RestorationBot.Telegram.FinalStateMachine.OperationsConfiguration.OperationStatesProfiles.UserRegistration;
using RestorationBot.Telegram.FinalStateMachine.States;
using RestorationBot.Telegram.FinalStateMachine.States.Implementation;
using RestorationBot.Telegram.FinalStateMachine.StateStorage.Particular.Abstract;
using RestorationBot.Telegram.FinalStateMachine.StateStorage.Particular.Abstract.Base;
using RestorationBot.Telegram.FinalStateMachine.StateStorage.Particular.Abstract.Certain;
using RestorationBot.Telegram.FinalStateMachine.StateStorage.Particular.Implementation;
using RestorationBot.Telegram.FinalStateMachine.StateStorage.StorageCleaner.Abstract;
using RestorationBot.Telegram.FinalStateMachine.StateStorage.StorageCleaner.Implementation;
using RestorationBot.Telegram.GatewayServices.Callback.Abstract;
using RestorationBot.Telegram.GatewayServices.Callback.Implementation;
using RestorationBot.Telegram.GatewayServices.Command.Abstract;
using RestorationBot.Telegram.GatewayServices.Command.Implementation;
using RestorationBot.Telegram.GatewayServices.State.Abstract;
using RestorationBot.Telegram.GatewayServices.State.Implementation;
using RestorationBot.Telegram.Handlers.Base;
using RestorationBot.Telegram.Handlers.Callback.Abstract;
using RestorationBot.Telegram.Handlers.Callback.Implementation;
using RestorationBot.Telegram.Handlers.Callback.Implementation.HavingProblem;
using RestorationBot.Telegram.Handlers.Callback.Implementation.UserRegistration;
using RestorationBot.Telegram.Handlers.Callback.Implementation.UserRestorationStepChange;
using RestorationBot.Telegram.Handlers.Callback.Implementation.UserTraining;
using RestorationBot.Telegram.Handlers.Command.Abstract;
using RestorationBot.Telegram.Handlers.Command.Implementation;
using RestorationBot.Telegram.Handlers.State.Abstract;
using RestorationBot.Telegram.Handlers.State.Implementation;
using RestorationBot.Telegram.Handlers.State.Implementation.UserRegistration;
using RestorationBot.Telegram.Handlers.State.Implementation.UserTraining;
using RestorationBot.Telegram.Services.Abstract;
using RestorationBot.Telegram.Services.Implementation;
using Telegram.Bot;
using Telegram.Bot.Polling;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

ConfigureServices(builder.Services, builder.Configuration);

WebApplication app = builder.Build();

app.Run();
return;

void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    services.AddDbContext<ApplicationDbContext>(options =>
    {
        string connectionString = configuration.GetConnectionString(nameof(ApplicationDbContext))
                               ?? throw new NullReferenceException("No connection string found.");

        options.UseNpgsql(connectionString)
               .EnableSensitiveDataLogging();
    });

    services.AddScoped<IUserRegistrationService, UserRegistrationService>();
    services.AddScoped<IUserTrainingService, UserTrainingService>();

    services.AddTransient<ICallbackGenerator, CallbackGenerator>();
    services.AddTransient<IRestorationStepMessageGenerator, RestorationStepMessageGenerator>();
    services.AddTransient<IMessageTextGenerator, MessageTextGenerator>();

    ConfigureTelegramServices(services, configuration);
}

void ConfigureTelegramServices(IServiceCollection services, IConfiguration configuration)
{
    string token = configuration.GetSection("TelegramBotToken").Value ??
                   throw new Exception("Server error: no telegram bot token configured");

    TelegramBotClient client = new(token);

    services.AddHttpClient(nameof(TelegramBotClient))
            .AddTypedClient<ITelegramBotClient>(httpClient => client);

    services.AddScoped<IUpdateHandler, ScopedMessageHandler>();
    services.AddScoped<IReceiverService, ReceiverService>();

    services.AddScoped<IStateStorageCleanerService, StateStorageCleanerServiceService>();
    
    services.AddSingletonWithMultipleAbstractions<UserRegistrationStateStorageCleaner, IClearableStateStorageService, IUserRegistrationStateStorageService>();
    services.AddSingletonWithMultipleAbstractions<UserChangeRestorationStepStateStorageCleaner, IClearableStateStorageService, IUserChangeRestorationStepStateStorageService>();
    services.AddSingletonWithMultipleAbstractions<UserTrainingClearableStateStorageService, IClearableStateStorageService,
            IUserTrainingStateStorageService>();

    services.AddScoped<ICallbackGatewayService, CallbackGatewayService>();
    services.AddScoped<ICommandGatewayService, CommandGatewayService>();
    services.AddScoped<IStateGatewayService, StateGatewayService>();

    services.AddScoped<ICallbackHandler, EnteringRestorationStepCallbackHandler>();
    services.AddScoped<ICallbackHandler, EnteringSexCallbackHandler>();
    services.AddScoped<ICallbackHandler, UpdatingUserRestorationStepCallbackHandler>();
    services.AddScoped<ICallbackHandler, ExerciseTypeChoosingCallbackHandler>();
    services.AddScoped<ICallbackHandler, OnHavingProblemsCallbackHandler>();
    services.AddScoped<ICallbackHandler, OnHavingCertainProblemCallbackHandler>();

    services.AddScoped<ICommandHandler, StartCommandHandler>();
    services.AddScoped<ICommandHandler, ChangeRestorationStepCommandHandler>();
    services.AddScoped<ICommandHandler, StartTrainingCommandHandler>();
    services.AddScoped<ICommandHandler, GetTrainingReportsCommandHandler>();

    services.AddScoped<IStateHandler, AgeEnteringStateHandler>();
    services.AddScoped<IStateHandler, BloodPressureEnteringStateHandler>();
    services.AddScoped<IStateHandler, HeartRateEnteringStateHandler>();

    services.AddHostedService<PoolingService>();
}