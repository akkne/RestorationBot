namespace RestorationBot.Helpers.Abstract.MessageGenerators;

using Models.Request;

public interface IMessageTextGenerator
{
    string GenerateExerciseMessageText(ExerciseMessageInformation messageInformation, int exerciseIndex);
    string GenerateMessageTextOnHavingCertainProblem(int problemIndex);
}