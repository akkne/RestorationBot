namespace RestorationBot.Helpers.Abstract;

using Models.Request;

public interface IMessageTextGenerator
{
    string GenerateExerciseMessageText(ExerciseMessageInformation messageInformation, int exerciseIndex);
    string GenerateMessageTextOnHavingCertainProblem(int problemIndex);
}