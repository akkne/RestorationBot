namespace RestorationBot.Helpers.Abstract;

using Contracts;

public interface IExerciseMessageTextGenerator
{
    string GenerateExerciseMessageText(ExerciseMessageInformation messageInformation);
}