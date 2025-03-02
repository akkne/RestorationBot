namespace RestorationBot.Helpers.Abstract.MessageGenerators;

using Models.Request;
using Models.Response;

public interface IMessageTextGenerator
{
    string GenerateExerciseMessageText(ExerciseMessageInformation messageInformation, int exerciseIndex);
    string GenerateMessageTextOnHavingCertainProblem(int problemIndex);
    TelegramMessageWithPhotoFile GenerateMessageOnHavingPainProblem();
}