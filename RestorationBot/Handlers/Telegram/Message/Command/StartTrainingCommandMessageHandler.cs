namespace RestorationBot.Handlers.Telegram.Message.Command;

using global::Telegram.Bot.Types;
using global::Telegram.Bot.Types.Enums;
using Helpers.Abstract;
using Helpers.Contracts;
using Microsoft.Extensions.Logging;
using Services.Abstact;
using TelegramUpdater.FilterAttributes.Attributes;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;
using BusinessUser = Models.User;

[Command(command: "training")]
[Private]
public class StartTrainingCommandMessageHandler : MessageHandler
{
    private readonly ILogger<StartTrainingCommandMessageHandler> _logger;
    private readonly IRestorationStepMessageGenerator _restorationStepMessageGenerator;
    private readonly IUserRegistrationService _userRegistrationService;

    public StartTrainingCommandMessageHandler(IUserRegistrationService userRegistrationService,
                                              ILogger<StartTrainingCommandMessageHandler> logger,
                                              IRestorationStepMessageGenerator restorationStepMessageGenerator,
                                              int group = 0) : base(group)
    {
        _userRegistrationService = userRegistrationService;
        _logger = logger;
        _restorationStepMessageGenerator = restorationStepMessageGenerator;
    }

    protected override async Task HandleAsync(IContainer<Message> container)
    {
        Message message = container.Update;
        long chatId = message.From!.Id;

        BusinessUser? user = await _userRegistrationService.GetByTelegramIdAsync(chatId);
        if (user == null)
        {
            _logger.LogWarning("User {@user} was not found", chatId);
            await ResponseAsync("Вы не зарегистрированны, пройдите процедуру регистрации через команду /start");
            return;
        }

        TelegramResponseMessageInformation responseInformation =
            _restorationStepMessageGenerator.GetRestorationStepMessage(user.RestorationStep);

        await ResponseAsync(responseInformation.Text, ParseMode.Html, replyMarkup: responseInformation.KeyboardMarkup);
    }
}