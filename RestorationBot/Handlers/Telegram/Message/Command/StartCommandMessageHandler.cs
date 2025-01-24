namespace RestorationBot.Handlers.Telegram.Message.Command;

using global::Telegram.Bot.Types;
using global::Telegram.Bot.Types.Enums;
using global::Telegram.Bot.Types.ReplyMarkups;
using Helpers.Abstract;
using Microsoft.Extensions.Logging;
using Services.Abstact;
using TelegramUpdater.FilterAttributes.Attributes;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

[Command(command: "start")]
[Private]
public class StartCommandMessageHandler : MessageHandler
{
    private readonly ICallbackGenerator _callbackGenerator;
    private readonly ILogger<StartCommandMessageHandler> _logger;
    private readonly IUserRegistrationService _userRegistrationService;

    public StartCommandMessageHandler(IUserRegistrationService userRegistrationService,
                                      ILogger<StartCommandMessageHandler> logger, ICallbackGenerator callbackGenerator,
                                      int group = 0) : base(group)
    {
        _userRegistrationService = userRegistrationService;
        _logger = logger;
        _callbackGenerator = callbackGenerator;
    }

    protected override async Task HandleAsync(IContainer<Message> container)
    {
        Message message = container.Update;
        long chatId = message.From!.Id;

        if (await _userRegistrationService.ContainsUserAsync(chatId))
        {
            const string onAlreadyRegisteredUser =
                "Здравствуйте! Я ваш виртуальный помощник для реабилитации после эндопротезирования коленного сустава.";
            await ResponseAsync(onAlreadyRegisteredUser, ParseMode.Html);
            return;
        }

        const string welcomeMessage = """
                                      Здравствуйте! Я ваш виртуальный помощник для реабилитации после эндопротезирования коленного сустава.

                                      С моей помощью вы сможете:
                                      - Узнать безопасные упражнения для восстановления.
                                      - Оценить своё состояние.
                                      - Получить рекомендации, если вы испытываете трудности.

                                      Пожалуйста, выберите этап вашей реабилитации:
                                      1️⃣ Ранний этап (0–2 недели после операции).

                                      2️⃣ Средний этап (2–6 недель после операции).

                                      3️⃣ Поздний этап (6 недель и более после операции).
                                      """;

        List<InlineKeyboardButton> inlineKeyboardButtons =
            new List<int> { 1, 2, 3 }.Select(x =>
                new InlineKeyboardButton(x.ToString())
                {
                    CallbackData = _callbackGenerator.GenerateCallbackOnChoosingRestorationStep(x)
                }).ToList();

        InlineKeyboardMarkup keyboardMarkup = new(inlineKeyboardButtons);

        await ResponseAsync(welcomeMessage, ParseMode.Html, replyMarkup: keyboardMarkup);
    }
}