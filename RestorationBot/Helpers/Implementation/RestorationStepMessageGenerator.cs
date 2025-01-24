namespace RestorationBot.Helpers.Implementation;

using Abstract;
using Contracts;
using Shared.Enums;
using Telegram.Bot.Types.ReplyMarkups;

public class RestorationStepMessageGenerator : IRestorationStepMessageGenerator
{
    private readonly ICallbackGenerator _callbackGenerator;

    public RestorationStepMessageGenerator(ICallbackGenerator callbackGenerator)
    {
        _callbackGenerator = callbackGenerator;
    }

    public TelegramResponseMessageInformation GetRestorationStepMessage(RestorationSteps restorationStep)
    {
        string text = restorationStep switch
        {
            RestorationSteps.Early => """
                                      Ранний этап реабилитации (0–2 недели)

                                      Основные задачи этапа:
                                       - Снижение боли и отёков.
                                       - Восстановление кровообращения.
                                       - Подготовка мышц к активной работе.

                                      Выберите упражнение, которое хотите выполнить:
                                      1️⃣ Дыхательные упражнения.
                                      2️⃣ Изометрические упражнения для квадрицепсов.
                                      3️⃣ Изометрические упражнения для ягодичных мышц.
                                      4️⃣ Упражнения для стоп и пальцев ног.
                                      """,
            RestorationSteps.Middle => """
                                       Средний этап реабилитации (2–6 недель)

                                       Основные задачи этапа:
                                        - Увеличение подвижности сустава.
                                        - Укрепление мышц.
                                        - Формирование правильной походки.

                                       Выберите упражнение, которое хотите выполнить:
                                       1️⃣ Активное сгибание и разгибание в коленном суставе.
                                       2️⃣ Поднятие прямой ноги.
                                       3️⃣ Упражнения с резиновой лентой.
                                       4️⃣ Формирование правильной походки.    
                                       """,
            RestorationSteps.Late => """
                                     Поздний этап реабилитации (6 недель и более)

                                     Основные задачи этапа:
                                      - Улучшение равновесия.
                                      - Увеличение выносливости.
                                      - Возвращение к привычной активности.

                                     Выберите упражнение, которое хотите выполнить:
                                     1️⃣ Приседания с опорой.
                                     2️⃣ Подъёмы на носки.
                                     3️⃣ Баланс на одной ноге.
                                     4️⃣ Ходьба.
                                     """,
            _ => throw new ArgumentOutOfRangeException(nameof(restorationStep), restorationStep, null)
        };

        List<InlineKeyboardButton> inlineKeyboardButtons =
            new List<int> { 1, 2, 3, 4 }.Select(x =>
                new InlineKeyboardButton(x.ToString())
                {
                    CallbackData =
                        _callbackGenerator.GenerateCallbackOnGetExercise(
                            ExerciseMessageInformation.Create(restorationStep, x))
                }).ToList();

        InlineKeyboardMarkup keyboardMarkup = new(inlineKeyboardButtons);

        return TelegramResponseMessageInformation.Create(text, keyboardMarkup);
    }
}