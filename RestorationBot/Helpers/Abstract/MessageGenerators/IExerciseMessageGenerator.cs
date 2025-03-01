namespace RestorationBot.Helpers.Abstract.MessageGenerators;

using Models.Request;
using Models.Response;

public interface IExerciseMessageGenerator
{
    IEnumerable<TelegramMessageWithVideoFiles> GenerateExerciseMessages(ExerciseMessageInformation messageInformation);
}