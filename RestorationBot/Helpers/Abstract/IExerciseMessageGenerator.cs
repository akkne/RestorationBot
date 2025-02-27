namespace RestorationBot.Helpers.Abstract;

using Models.Request;
using Models.Response;

public interface IExerciseMessageGenerator
{
    IEnumerable<TelegramMessageWithVideoFiles> GenerateExerciseMessages(ExerciseMessageInformation messageInformation);
}