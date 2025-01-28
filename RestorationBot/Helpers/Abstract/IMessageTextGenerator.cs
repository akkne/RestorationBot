namespace RestorationBot.Helpers.Abstract;

using Contracts;

public interface IMessageTextGenerator
{
    string GenerateExerciseMessageText(ExerciseMessageInformation messageInformation);
    string GenerateMessageTextOnHavingCertainProblem(int problemIndex);
}