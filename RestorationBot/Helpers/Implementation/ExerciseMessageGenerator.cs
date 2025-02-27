namespace RestorationBot.Helpers.Implementation;

using Abstract;
using global::Telegram.Bot.Types;
using Models.Request;
using Models.Response;

public class ExerciseMessageGenerator : IExerciseMessageGenerator
{
    private const int ExerciseCount = 6;
    private readonly IExerciseVideoHelper _exerciseVideoHelper;

    private readonly IMessageTextGenerator _messageTextGenerator;

    public ExerciseMessageGenerator(IMessageTextGenerator messageTextGenerator,
                                    IExerciseVideoHelper exerciseVideoHelper)
    {
        _messageTextGenerator = messageTextGenerator;
        _exerciseVideoHelper = exerciseVideoHelper;
    }

    public IEnumerable<TelegramMessageWithVideoFiles> GenerateExerciseMessages(
        ExerciseMessageInformation messageInformation)
    {
        for (int i = 0; i < ExerciseCount; i++)
        {
            string exerciseMessageText =
                _messageTextGenerator.GenerateExerciseMessageText(messageInformation, i);
            List<InputFile> animations = _exerciseVideoHelper.GetExerciseVideos(messageInformation, i);
            yield return
                TelegramMessageWithVideoFiles.Create(exerciseMessageText, animations);
        }
    }
}