namespace RestorationBot.Telegram.Handlers.Callback.Implementation.HavingProblem;

using System.Text.RegularExpressions;
using Abstract;
using FinalStateMachine.OperationsConfiguration.OperationStatesProfiles.HavingPain;
using FinalStateMachine.States.Implementation;
using FinalStateMachine.StateStorage.Particular.Abstract.Certain;
using global::Telegram.Bot;
using global::Telegram.Bot.Types;
using global::Telegram.Bot.Types.Enums;
using Helpers.Abstract;
using Helpers.Abstract.MessageGenerators;
using Helpers.Models.Response;

public class OnHavingCertainProblemCallbackHandler : ICallbackHandler
{
    private readonly ICallbackGenerator _callbackGenerator;
    private readonly IUserHavingPainStateStorageService _storageService;
    private readonly IMessageTextGenerator _textGenerator;

    public OnHavingCertainProblemCallbackHandler(ICallbackGenerator callbackGenerator,
                                                 IMessageTextGenerator textGenerator,
                                                 IUserHavingPainStateStorageService storageService)
    {
        _callbackGenerator = callbackGenerator;
        _textGenerator = textGenerator;
        _storageService = storageService;
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
        if (problemIndex == 1)
        {
            await OnHavingPainProblemAsync(userFrom, botClient, cancellationToken);
            return;
        }

        string messageText = _textGenerator.GenerateMessageTextOnHavingCertainProblem(problemIndex);
        await botClient.SendMessage(userFrom.Id, messageText, ParseMode.Html, cancellationToken: cancellationToken);
    }

    private async Task OnHavingPainProblemAsync(User userFrom, ITelegramBotClient botClient,
                                                CancellationToken cancellationToken)
    {
        HavingPainState state = _storageService.GetOrAddState(userFrom.Id);
        if (state.StateMachine.State != HavingPainStateProfile.Ready)
        {
            _storageService.TryRemove(userFrom.Id);
            _storageService.GetOrAddState(userFrom.Id);
        }

        await state.StateMachine.FireAsync(HavingPainTriggerProfile.Begin);

        const int problemIndex = 1;

        string messageText = _textGenerator.GenerateMessageTextOnHavingCertainProblem(problemIndex);
        await botClient.SendMessage(userFrom.Id, messageText, ParseMode.Html, cancellationToken: cancellationToken);

        TelegramMessageWithPhotoFile message = _textGenerator.GenerateMessageOnHavingPainProblem();
        await botClient.SendPhoto(userFrom.Id, message.Photo, message.Text, ParseMode.Html,
            cancellationToken: cancellationToken);
    }
}