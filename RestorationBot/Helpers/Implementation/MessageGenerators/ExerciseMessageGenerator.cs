namespace RestorationBot.Helpers.Implementation.MessageGenerators;

using Abstract;
using Abstract.MessageGenerators;
using global::Telegram.Bot.Types;
using Models.Request;
using Models.Response;

public class ExerciseMessageGenerator : IExerciseMessageGenerator
{
    private const int ExerciseCount = 6;

    private readonly IExerciseVideoHelper _exerciseVideoHelper;
    private readonly IIdeomotorExerciseMessageGenerator _ideomotorExerciseMessageGenerator;
    private readonly IMessageTextGenerator _messageTextGenerator;

    public ExerciseMessageGenerator(IMessageTextGenerator messageTextGenerator,
                                    IExerciseVideoHelper exerciseVideoHelper,
                                    IIdeomotorExerciseMessageGenerator ideomotorExerciseMessageGenerator)
    {
        _messageTextGenerator = messageTextGenerator;
        _exerciseVideoHelper = exerciseVideoHelper;
        _ideomotorExerciseMessageGenerator = ideomotorExerciseMessageGenerator;
    }

    public IEnumerable<TelegramMessageWithVideoFiles> GenerateExerciseMessages(
        ExerciseMessageInformation messageInformation)
    {
        for (int i = 0; i < ExerciseCount; i++)
        {
            string exerciseMessageText =
                _messageTextGenerator.GenerateExerciseMessageText(messageInformation, i);
            List<InputFile> videos = _exerciseVideoHelper.GetExerciseVideos(messageInformation, i);
            yield return
                TelegramMessageWithVideoFiles.Create(exerciseMessageText, videos);
        }
    }
}