namespace RestorationBot.Helpers.Abstract.MessageGenerators;

using Models.Request;

public interface IIdeomotorExerciseMessageGenerator
{
    string? GenerateMessage(ExerciseMessageInformation messageInformation);
}