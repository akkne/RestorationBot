namespace RestorationBot.Handlers.Telegram.Message.Command;

using global::Telegram.Bot.Types;
using global::Telegram.Bot.Types.Enums;
using global::Telegram.Bot.Types.ReplyMarkups;
using Helpers.Abstract;
using Services.Abstact;
using Shared.Enums;
using TelegramUpdater.FilterAttributes.Attributes;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

[Command(command: "change")]
[Private]
public class ChangeRestorationStepCommandMessageHandler : MessageHandler
{
    private readonly ICallbackGenerator _callbackGenerator;
    private readonly IUserRegistrationService _userRegistrationService;

    public ChangeRestorationStepCommandMessageHandler(ICallbackGenerator callbackGenerator, IUserRegistrationService userRegistrationService)
    {
        _callbackGenerator = callbackGenerator;
        _userRegistrationService = userRegistrationService;
    }

    protected override async Task HandleAsync(IContainer<Message> container)
    {
        Message message = container.Update;
        long chatId = message.From!.Id;

        if (!await _userRegistrationService.ContainsUserAsync(chatId))
        {
            const string onAlreadyRegisteredUser =
                "Для начала пройдите процедуру регистрации";
            await ResponseAsync(onAlreadyRegisteredUser, ParseMode.Html);
            return;
        }
        
        List<InlineKeyboardButton> inlineKeyboardButtons =
            new List<int> { 1, 2, 3 }.Select(x =>
                new InlineKeyboardButton(x.ToString())
                {
                    CallbackData = _callbackGenerator.GenerateCallbackOnChangingRestorationStep((RestorationSteps) (x - 1))
                }).ToList();

        InlineKeyboardMarkup keyboardMarkup = new(inlineKeyboardButtons);
        
        const string choosingStepMessage = """
                                           Отлично! Для того, чтобы изменить этап реабилитации, выберите из этапов ниже:
                                           
                                           1️⃣ Ранний этап (0–2 недели после операции).

                                           2️⃣ Средний этап (2–6 недель после операции).

                                           3️⃣ Поздний этап (6 недель и более после операции).
                                           """;
        
        await ResponseAsync(choosingStepMessage, ParseMode.Html, replyMarkup: keyboardMarkup);
    }
}