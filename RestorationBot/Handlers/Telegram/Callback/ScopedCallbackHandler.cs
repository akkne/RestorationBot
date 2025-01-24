namespace RestorationBot.Handlers.Telegram.Callback;

using global::Telegram.Bot;
using global::Telegram.Bot.Types;
using global::Telegram.Bot.Types.Enums;
using Helpers.Abstract;
using Helpers.Contracts;
using Services.Abstact;
using Shared.Enums;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;
using BusinessUser = Models.User;

public class ScopedCallbackHandler : CallbackQueryHandler
{
    private readonly ITelegramBotClient _botClient;
    private readonly ICallbackGenerator _callbackGenerator;
    private readonly IExerciseMessageTextGenerator _exerciseMessageTextGenerator;
    private readonly IUserRegistrationService _userRegistrationService;

    public ScopedCallbackHandler(ICallbackGenerator callbackGenerator, IUserRegistrationService userRegistrationService,
                                 ITelegramBotClient botClient,
                                 IExerciseMessageTextGenerator exerciseMessageTextGenerator)
    {
        _callbackGenerator = callbackGenerator;
        _userRegistrationService = userRegistrationService;
        _botClient = botClient;
        _exerciseMessageTextGenerator = exerciseMessageTextGenerator;
    }

    protected override async Task HandleAsync(IContainer<CallbackQuery> container)
    {
        if (container.Container.CallbackQuery?.Data == null) return;

        CallbackQuery? callbackQuery = container.Container.CallbackQuery;

        List<string> possibleCallbacksOnChoosingRestorationStep = new List<int> { 1, 2, 3 }.Select(x =>
            _callbackGenerator.GenerateCallbackOnChoosingRestorationStep(x)).ToList();

        if (possibleCallbacksOnChoosingRestorationStep.Contains(callbackQuery.Data))
            await OnChoosingRestorationStepAsync(callbackQuery);

        // Generates all possible cases for callbacksOnGettingExercise (code is hard sometimes)
        List<List<ExerciseMessageInformation>> listOfListsOfPossibleCallbacksOnGettingExercise =
            new List<RestorationSteps> { RestorationSteps.Early, RestorationSteps.Middle, RestorationSteps.Late }
               .Select(x => new List<ExerciseMessageInformation>
                {
                    ExerciseMessageInformation.Create(x, 1),
                    ExerciseMessageInformation.Create(x, 2),
                    ExerciseMessageInformation.Create(x, 3),
                    ExerciseMessageInformation.Create(x, 4)
                }).ToList();

        List<ExerciseMessageInformation> possibleExerciseInformationOnGettingExercise = [];
        foreach (List<ExerciseMessageInformation> list in listOfListsOfPossibleCallbacksOnGettingExercise)
            possibleExerciseInformationOnGettingExercise.AddRange(list
               .Select(info => info));

        List<string> possibleCallbacksOnGettingExercise = possibleExerciseInformationOnGettingExercise
                                                         .Select(info =>
                                                              _callbackGenerator.GenerateCallbackOnGetExercise(info))
                                                         .ToList();
        if (possibleCallbacksOnGettingExercise.Contains(callbackQuery.Data))
        {
            ExerciseMessageInformation result = possibleExerciseInformationOnGettingExercise.FirstOrDefault(
                info => callbackQuery.Data == _callbackGenerator.GenerateCallbackOnGetExercise(info))!;

            await OnGettingExerciseAsync(callbackQuery, result);
        }
    }

    private async Task OnGettingExerciseAsync(CallbackQuery callbackQuery, ExerciseMessageInformation result)
    {
        long userId = callbackQuery.From.Id;

        string messageText = _exerciseMessageTextGenerator.GenerateExerciseMessageText(result);
        await _botClient.SendTextMessageAsync(userId, messageText, ParseMode.Html);
    }

    private async Task OnChoosingRestorationStepAsync(CallbackQuery callbackQuery)
    {
        long userId = callbackQuery.From.Id;
        if (await _userRegistrationService.ContainsUserAsync(userId))
        {
            await AnswerAsync("Вы уже зарегистрированны, повторная регистрация не предусмотрена.");
            return;
        }

        List<int> possibleRestorationSteps = [1, 2, 3];

        int? nullableRestorationStep = possibleRestorationSteps
           .FirstOrDefault(x => _callbackGenerator.GenerateCallbackOnChoosingRestorationStep(x) == callbackQuery.Data);
        if (!nullableRestorationStep.HasValue) return;

        RestorationSteps restorationStep = nullableRestorationStep.Value switch
        {
            1 => RestorationSteps.Early,
            2 => RestorationSteps.Middle,
            3 => RestorationSteps.Late,
            _ => throw new ArgumentException("Invalid restoration step number")
        };

        BusinessUser? user = await _userRegistrationService.RegisterUserAsync(userId, restorationStep);
        if (user == null)
            await _botClient.SendTextMessageAsync(userId, "Что-то пошло не так, попробуйте позже.", ParseMode.Html);

        await _botClient.SendTextMessageAsync(userId, "Вы были успешно зарегистрированны.", ParseMode.Html);
    }
}