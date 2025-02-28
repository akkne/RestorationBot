namespace RestorationBot.Telegram.Handlers.Callback.Implementation.HavingProblem;

using System.Text.RegularExpressions;
using Abstract;
using global::Telegram.Bot;
using global::Telegram.Bot.Types;
using global::Telegram.Bot.Types.Enums;
using Helpers.Abstract;
using Helpers.Abstract.MessageGenerators;

public class OnHavingCertainProblemCallbackHandler : ICallbackHandler
{
    private readonly ICallbackGenerator _callbackGenerator;
    private readonly IMessageTextGenerator _textGenerator;

    public OnHavingCertainProblemCallbackHandler(ICallbackGenerator callbackGenerator,
                                                 IMessageTextGenerator textGenerator)
    {
        _callbackGenerator = callbackGenerator;
        _textGenerator = textGenerator;
    }

    public bool CanHandle(CallbackQuery callbackQuery)
    {
        Match match = _callbackGenerator.GetCallbackRegexOnHavingCertainProblem().Match(callbackQuery.Data!);

        return match.Success;
    }

    public async Task HandleCommandAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery,
                                         CancellationToken cancellationToken)
    {
        Match match = _callbackGenerator.GetCallbackRegexOnHavingCertainProblem().Match(callbackQuery.Data!);
        if (!match.Success) throw new ArgumentException("Invalid callback query");

        int problemIndex = int.Parse(match.Groups["problemIndex"].Value);

        await OnHavingCertainProblemAsync(problemIndex, callbackQuery.From, botClient, cancellationToken);
    }

    private async Task OnHavingCertainProblemAsync(int problemIndex, User userFrom, ITelegramBotClient botClient,
                                                   CancellationToken cancellationToken)
    {
        string messageText = _textGenerator.GenerateMessageTextOnHavingCertainProblem(problemIndex);
        await botClient.SendMessage(userFrom.Id, messageText, ParseMode.Html, cancellationToken: cancellationToken);
    }
}