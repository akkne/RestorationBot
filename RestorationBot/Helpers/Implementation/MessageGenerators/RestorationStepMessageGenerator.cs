namespace RestorationBot.Helpers.Implementation.MessageGenerators;

using Abstract.MessageGenerators;
using global::Telegram.Bot.Types.ReplyMarkups;
using RestorationBot.Helpers.Abstract;
using RestorationBot.Helpers.Models.Request;
using RestorationBot.Helpers.Models.Response;
using RestorationBot.Shared.Enums;

public class RestorationStepMessageGenerator : IRestorationStepMessageGenerator
{
    private readonly ICallbackGenerator _callbackGenerator;

    public RestorationStepMessageGenerator(ICallbackGenerator callbackGenerator)
    {
        _callbackGenerator = callbackGenerator;
    }

    public TelegramMessageWithInlineKeyboard GetRestorationStepMessage(RestorationSteps restorationStep)
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
                            ExerciseMessageInformation.Create(restorationStep, x))
                }).ToList();

        InlineKeyboardMarkup keyboardMarkup = new(inlineKeyboardButtons);

        return TelegramMessageWithInlineKeyboard.Create(text, keyboardMarkup);
    }
}