namespace RestorationBot.Handlers.Telegram.Callback;

using System.Text.RegularExpressions;
using global::Telegram.Bot;
using global::Telegram.Bot.Types;
using global::Telegram.Bot.Types.Enums;
using global::Telegram.Bot.Types.ReplyMarkups;
using Helpers.Abstract;
using Helpers.Contracts;
using Microsoft.Extensions.Logging;
using Services.Abstact;
using Shared.Enums;
using TelegramUpdater.Filters;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;
using BusinessUser = Models.User;

public class ScopedCallbackHandler : CallbackQueryHandler
{
    private readonly ITelegramBotClient _botClient;
    private readonly ICallbackGenerator _callbackGenerator;
    private readonly IMessageTextGenerator _messageTextGenerator;
    private readonly IUserRegistrationService _userRegistrationService;
    private readonly ILogger<ScopedCallbackHandler> _logger;

    public ScopedCallbackHandler(ICallbackGenerator callbackGenerator, IUserRegistrationService userRegistrationService,
                                 ITelegramBotClient botClient,
                                 IMessageTextGenerator messageTextGenerator, ILogger<ScopedCallbackHandler> logger)
    {
        _callbackGenerator = callbackGenerator;
        _userRegistrationService = userRegistrationService;
        _botClient = botClient;
        _messageTextGenerator = messageTextGenerator;
        _logger = logger;
    }

    protected override async Task HandleAsync(IContainer<CallbackQuery> container)
    {
        if (container.Container.CallbackQuery?.Data == null) return;

        CallbackQuery? callbackQuery = container.Container.CallbackQuery;

        Match matchOnChoosingRestorationStep = _callbackGenerator.GetCallbackRegexOnChoosingRestorationStep().Match(callbackQuery.Data);
        if (matchOnChoosingRestorationStep.Success)
        {
            RestorationSteps restorationStep =
                (RestorationSteps) int.Parse(matchOnChoosingRestorationStep.Groups["index"].Value);
            
            await OnChoosingRestorationStepAsync(callbackQuery, restorationStep);
            return;
        }
        
        Match matchOnGetExercise = _callbackGenerator.GetCallbackRegexOnGetExercise().Match(callbackQuery.Data);
        if (matchOnGetExercise.Success)
        {
            RestorationSteps restorationStep =
                (RestorationSteps) int.Parse(matchOnGetExercise.Groups["step"].Value);
            int exerciseStep = int.Parse(matchOnGetExercise.Groups["index"].Value);
            
            await OnGettingExerciseAsync(callbackQuery, ExerciseMessageInformation.Create(restorationStep, exerciseStep));
            return;
        }
        
        Match matchOnHavingProblems = _callbackGenerator.GetCallbackRegexOnHavingProblem().Match(callbackQuery.Data!);
        if (matchOnHavingProblems.Success)
        {
            bool havingProblem = bool.Parse(matchOnHavingProblems.Groups["hasProblem"].Value);
            
            await OnHavingProblemAsync(callbackQuery, havingProblem);
            return;
        }
        
        Match matchOnHavingCertainProblem = _callbackGenerator.GetCallbackRegexOnHavingCertainProblem().Match(callbackQuery.Data);
        if (matchOnHavingCertainProblem.Success)
        {
            int problemIndex = int.Parse(matchOnHavingCertainProblem.Groups["problemIndex"].Value);
            
            await OnHavingCertainProblemAsync(callbackQuery, problemIndex);
        }
        
        Match matchOnChangingRestorationStep = _callbackGenerator.GetCallbackRegexOnChangingRestorationStep().Match(callbackQuery.Data);
        if (matchOnChangingRestorationStep.Success)
        {
            RestorationSteps restorationStep =
                (RestorationSteps) int.Parse(matchOnChangingRestorationStep.Groups["index"].Value);
            
            await OnChangingRestorationStepAsync(callbackQuery, restorationStep);
            return;
        }
    }

    private async Task OnChangingRestorationStepAsync(CallbackQuery callbackQuery, RestorationSteps restorationStep)
    {
        long userId = callbackQuery.From.Id;
        BusinessUser? user = await _userRegistrationService.GetByTelegramIdAsync(userId);

        if (user == null)
        {
            await _botClient.SendTextMessageAsync(userId, "Что-то пошло не так, попробуйте позже", ParseMode.Html);
            return;
        }
        
        if (user.RestorationStep == restorationStep)
        {
            await _botClient.SendTextMessageAsync(userId, "Вы уже находитесь на этом этапе реабилитации", ParseMode.Html);
            return;
        }

        try
        {
            await _userRegistrationService.UpdateUserRestorationStepAsync(userId, restorationStep);
            
            await _botClient.SendTextMessageAsync(userId, "Вы успешно изменили свой этап реабилитации", ParseMode.Html);
        }
        catch (Exception exception)
        {
            _logger.LogWarning("Received exception while changing user restoration step: {0}", exception.Message);
            await _botClient.SendTextMessageAsync(userId, "Что-то пошло не так, попробуйте позже", ParseMode.Html);
        }
        
    }

    private async Task OnHavingCertainProblemAsync(CallbackQuery callbackQuery, int problemIndex)
    {
        string messageText = _messageTextGenerator.GenerateMessageTextOnHavingCertainProblem(problemIndex);
        await _botClient.SendTextMessageAsync(callbackQuery.From.Id, messageText, ParseMode.Html);
    }

    private async Task OnHavingProblemAsync(CallbackQuery callbackQuery, bool havingProblem)
    {
        if (!havingProblem)
        {
            await AnswerAsync("Отлично! Продолжайте в том же духе");
            return;
        }

        const string messageOnHavingProblems = """
                                               Что помешало вам выполнить упражнение?
                                               
                                               1️⃣ Боль
                                               2️⃣ Усталость
                                               3️⃣ Сложность выполнения
                                               """;
        InlineKeyboardMarkup keyboardMarkup = new(
            new List<int> {1, 2, 3}.Select(x => new InlineKeyboardButton(x.ToString())
            {
                CallbackData = _callbackGenerator.GenerateCallbackOnHavingCertainProblem(x)
            }));
        
        await _botClient.SendTextMessageAsync(callbackQuery.From.Id, messageOnHavingProblems, parseMode: ParseMode.Html, replyMarkup: keyboardMarkup);
    }

    private async Task OnGettingExerciseAsync(CallbackQuery callbackQuery, ExerciseMessageInformation result)
    {
        long userId = callbackQuery.From.Id;

        string messageText = _messageTextGenerator.GenerateExerciseMessageText(result);
        await _botClient.SendTextMessageAsync(userId, messageText, ParseMode.Html);
        
        InlineKeyboardMarkup keyboardMarkup = new(
            [
                new InlineKeyboardButton("Да")
                {
                    CallbackData = _callbackGenerator.GenerateCallbackOnHavingProblem(true)
                },
                new InlineKeyboardButton("Нет")
                {
                    CallbackData = _callbackGenerator.GenerateCallbackOnHavingProblem(false)
                }
            ]
        );
        await _botClient.SendTextMessageAsync(userId, "Возникли какие-то проблемы при выполнении упражнений?", ParseMode.Html, replyMarkup: keyboardMarkup);
    }

    private async Task OnChoosingRestorationStepAsync(CallbackQuery callbackQuery, RestorationSteps restorationStep)
    {
        long userId = callbackQuery.From.Id;
        if (await _userRegistrationService.ContainsUserAsync(userId))
        {
            await AnswerAsync("Вы уже зарегистрированны, повторная регистрация не предусмотрена.");
            return;
        }

        BusinessUser? user = await _userRegistrationService.RegisterUserAsync(userId, restorationStep);
        if (user == null)
        {
            await _botClient.SendTextMessageAsync(userId, "Что-то пошло не так, попробуйте позже.", ParseMode.Html);
            return;
        }
        
        await _botClient.SendTextMessageAsync(userId, "Вы были успешно зарегистрированны.", ParseMode.Html);
    }
}