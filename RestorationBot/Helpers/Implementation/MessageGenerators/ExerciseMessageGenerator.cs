namespace RestorationBot.Helpers.Implementation.MessageGenerators;

using global::Telegram.Bot.Types;
using RestorationBot.Helpers.Abstract;
using RestorationBot.Helpers.Abstract.MessageGenerators;
using RestorationBot.Helpers.Models.Request;
using RestorationBot.Helpers.Models.Response;

public class ExerciseMessageGenerator : IExerciseMessageGenerator
{
    private const int ExerciseCount = 6;
    
    private readonly IExerciseVideoHelper _exerciseVideoHelper;
    private readonly IMessageTextGenerator _messageTextGenerator;
    private readonly IIdeomotorExerciseMessageGenerator _ideomotorExerciseMessageGenerator;

    public ExerciseMessageGenerator(IMessageTextGenerator messageTextGenerator,
                                    IExerciseVideoHelper exerciseVideoHelper, IIdeomotorExerciseMessageGenerator ideomotorExerciseMessageGenerator)
    {
        _messageTextGenerator = messageTextGenerator;
        _exerciseVideoHelper = exerciseVideoHelper;
        _ideomotorExerciseMessageGenerator = ideomotorExerciseMessageGenerator;
    }

    public IEnumerable<TelegramMessageWithVideoFiles> GenerateExerciseMessages(
        ExerciseMessageInformation messageInformation)
    {
        string? ideomotorMessage = _ideomotorExerciseMessageGenerator.GenerateMessage(messageInformation);
        if (ideomotorMessage != null)
        {
            yield return TelegramMessageWithVideoFiles.Create(ideomotorMessage, []);
        }
        
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