namespace RestorationBot.Helpers.Implementation.MessageGenerators;

using Abstract;
using Abstract.MessageGenerators;
using global::Telegram.Bot.Types.ReplyMarkups;
using Models.Request;
using Models.Response;
using Shared.Enums;

public class RestorationStepMessageGenerator : IRestorationStepMessageGenerator
{
    private readonly ICallbackGenerator _callbackGenerator;

    public RestorationStepMessageGenerator(ICallbackGenerator callbackGenerator)
    {
        _callbackGenerator = callbackGenerator;
    }

    public TelegramMessageWithInlineKeyboard GetPhysicalTrainingMessage(RestorationSteps restorationStep)
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
                                      2️⃣ Изометрические упражнения для мышц бедра и ягодиц.
                                      3️⃣ Упражнения для стоп и пальцев ног.
                                      4️⃣ Миофасциальная релаксация (с фоам-роллером).
                                      """,
            RestorationSteps.Middle => """
                                       Средний этап реабилитации (2–6 недель)

                                       Основные задачи этапа:
                                        - Увеличение подвижности сустава.
                                        - Укрепление мышц.
                                        - Формирование правильной походки.

                                       Выберите упражнение, которое хотите выполнить:
                                       1️⃣ Дыхательные упражнения.
                                       2️⃣ Укрепление мышц ног.
                                       3️⃣ Упражнения для суставов.
                                       4️⃣ Миофасциальная релаксация (с фоам-роллером).
                                       """,
            RestorationSteps.Late => """
                                     Поздний этап реабилитации (6 недель и более)

                                     Основные задачи этапа:
                                      - Улучшение равновесия.
                                      - Увеличение выносливости.
                                      - Возвращение к привычной активности.

                                     Выберите упражнение, которое хотите выполнить:
                                     1️⃣ Дыхательные упражнения.
                                     2️⃣ Укрепление мышц ног.
                                     3️⃣ Упражнения для суставов.
                                     4️⃣ Упражнения для баланса и координации.
                                     5️⃣ Миофасциальная релаксация (с фоам-роллером).
                                     """,
            _ => throw new ArgumentOutOfRangeException(nameof(restorationStep), restorationStep, null)
        };

        List<int> variety = restorationStep != RestorationSteps.Late
            ? Enumerable.Range(1, 4).ToList()
            : Enumerable.Range(1, 5).ToList();

        List<InlineKeyboardButton> inlineKeyboardButtons =
            variety.Select(x =>
                new InlineKeyboardButton(x.ToString())
                {
                    CallbackData =
                        _callbackGenerator.GenerateCallbackOnGetExercise(
                            ExerciseMessageInformation.Create(restorationStep, x), 1)
                }).ToList();

        InlineKeyboardMarkup keyboardMarkup = new(inlineKeyboardButtons);

        return TelegramMessageWithInlineKeyboard.Create(text, keyboardMarkup);
    }

    public TelegramMessageWithInlineKeyboard GetIdeomotorTrainingMessage(RestorationSteps restorationStep)
    {
        string text = restorationStep switch
        {
            RestorationSteps.Early => """
                                      Ранний этап реабилитации (0–2 недели)

                                      Основные задачи этапа:
                                       - Снижение боли и отёков.
                                       - Восстановление кровообращения.
                                       - Подготовка мышц к активной работе.

                                      Выберите идеомоторные упражнение, которое хотите выполнить:
                                      1️⃣ Идеомоторные дыхательные упражнения.
                                      2️⃣ Идеомоторные изометрические упражнения для мышц бедра и ягодиц.
                                      3️⃣ Идеомоторные упражнения для стоп и пальцев ног.
                                      """,
            RestorationSteps.Middle => """
                                       Средний этап реабилитации (2–6 недель)

                                       Основные задачи этапа:
                                        - Увеличение подвижности сустава.
                                        - Укрепление мышц.
                                        - Формирование правильной походки.

                                       Выберите идеомоторные упражнение, которое хотите выполнить:
                                       1️⃣ Идеомоторные дыхательные упражнения.
                                       2️⃣ Идеомоторное укрепление мышц ног.
                                       3️⃣ Идеомоторные упражнения для суставов.
                                       """,
            RestorationSteps.Late => """
                                     Поздний этап реабилитации (6 недель и более)

                                     Основные задачи этапа:
                                      - Улучшение равновесия.
                                      - Увеличение выносливости.
                                      - Возвращение к привычной активности.

                                     Выберите идеомоторные упражнение, которое хотите выполнить:
                                     1️⃣ Идеомоторные дыхательные упражнения.
                                     2️⃣ Идеомоторное укрепление мышц ног.
                                     3️⃣ Идеомоторные упражнения для суставов.
                                     4️⃣ Идеомоторные упражнения для баланса и координации.
                                     """,
            _ => throw new ArgumentOutOfRangeException(nameof(restorationStep), restorationStep, null)
        };

        List<int> variety = restorationStep != RestorationSteps.Late
            ? Enumerable.Range(1, 3).ToList()
            : Enumerable.Range(1, 4).ToList();

        List<InlineKeyboardButton> inlineKeyboardButtons =
            variety.Select(x =>
                new InlineKeyboardButton(x.ToString())
                {
                    CallbackData =
                        _callbackGenerator.GenerateCallbackOnGetExercise(
                            ExerciseMessageInformation.Create(restorationStep, x), 0)
                }).ToList();

        InlineKeyboardMarkup keyboardMarkup = new(inlineKeyboardButtons);

        return TelegramMessageWithInlineKeyboard.Create(text, keyboardMarkup);
    }
}