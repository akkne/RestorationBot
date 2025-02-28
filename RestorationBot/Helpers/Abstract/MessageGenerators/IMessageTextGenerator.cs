namespace RestorationBot.Helpers.Abstract.MessageGenerators;

using RestorationBot.Helpers.Models.Request;

public interface IMessageTextGenerator
{
    string GenerateExerciseMessageText(ExerciseMessageInformation messageInformation, int exerciseIndex);
    string GenerateMessageTextOnHavingCertainProblem(int problemIndex);
}