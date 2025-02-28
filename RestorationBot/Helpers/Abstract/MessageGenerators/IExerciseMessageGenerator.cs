namespace RestorationBot.Helpers.Abstract.MessageGenerators;

using RestorationBot.Helpers.Models.Request;
using RestorationBot.Helpers.Models.Response;

public interface IExerciseMessageGenerator
{
    IEnumerable<TelegramMessageWithVideoFiles> GenerateExerciseMessages(ExerciseMessageInformation messageInformation);
}