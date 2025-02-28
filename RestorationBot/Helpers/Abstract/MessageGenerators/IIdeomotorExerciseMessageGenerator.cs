namespace RestorationBot.Helpers.Abstract.MessageGenerators;

using RestorationBot.Helpers.Models.Request;

public interface IIdeomotorExerciseMessageGenerator
{ 
    string? GenerateMessage(ExerciseMessageInformation messageInformation);
}