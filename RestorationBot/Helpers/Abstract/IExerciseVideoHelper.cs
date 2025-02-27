namespace RestorationBot.Helpers.Abstract;

using global::Telegram.Bot.Types;
using Models.Request;

public interface IExerciseVideoHelper
{
    List<InputFile> GetExerciseVideos(ExerciseMessageInformation messageInformation, int exerciseIndex);
}