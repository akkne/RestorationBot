namespace RestorationBot.Telegram.Handlers.Callback.Implementation.HavingProblem;

using System.Text.RegularExpressions;
using Abstract;
using global::Telegram.Bot;
using global::Telegram.Bot.Types;
using global::Telegram.Bot.Types.Enums;
using global::Telegram.Bot.Types.ReplyMarkups;
using Helpers.Abstract;

public class OnHavingProblemsCallbackHandler : ICallbackHandler
{
    private readonly ICallbackGenerator _callbackGenerator;

    public OnHavingProblemsCallbackHandler(ICallbackGenerator callbackGenerator)
    {
        _callbackGenerator = callbackGenerator;
    }

    public bool CanHandle(CallbackQuery callbackQuery)
    {
        Match match = _callbackGenerator.GetCallbackRegexOnHavingProblem().Match(callbackQuery.Data!);

        return match.Success;
    }

    public async Task HandleCommandAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery,
                                         CancellationToken cancellationToken)
    {
        Match match = _callbackGenerator.GetCallbackRegexOnHavingProblem().Match(callbackQuery.Data!);
        if (!match.Success) throw new ArgumentException("Invalid callback query");

        bool havingProblem = bool.Parse(match.Groups["hasProblem"].Value);
        await OnHavingProblemAsync(havingProblem, callbackQuery.From, botClient, cancellationToken);
    }

    private async Task OnHavingProblemAsync(bool havingProblem, User userFrom, ITelegramBotClient botClient,
                                            CancellationToken cancellationToken)
    {
        if (!havingProblem)
        {
            const string messageOnHavingNoProblems = """
                                                     Отлично!
                                                     Продолжайте в том же духе
                                                     """;
            await botClient.SendMessage(userFrom.Id, messageOnHavingNoProblems, cancellationToken: cancellationToken);
            return;
        }

        const string messageOnHavingProblems = """
                                               Что помешало вам выполнить упражнение?

                                               1️⃣ Боль
                                               2️⃣ Усталость
                                               3️⃣ Сложность выполнения
                                               """;
        InlineKeyboardMarkup keyboardMarkup = new(
            new List<int> { 1, 2, 3 }.Select(x => new InlineKeyboardButton(x.ToString())
            {
                CallbackData = _callbackGenerator.GenerateCallbackOnHavingCertainProblem(x)
            }));

        await botClient.SendMessage(userFrom.Id, messageOnHavingProblems, ParseMode.Html, replyMarkup: keyboardMarkup,
            cancellationToken: cancellationToken);
    }
}