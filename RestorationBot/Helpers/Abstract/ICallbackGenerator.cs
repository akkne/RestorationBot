namespace RestorationBot.Helpers.Abstract;

using Contracts;

public interface ICallbackGenerator
{
    string GenerateCallbackOnChoosingRestorationStep(int restorationStepIndex);
    string GenerateCallbackOnGetExercise(ExerciseMessageInformation messageInformation);
}