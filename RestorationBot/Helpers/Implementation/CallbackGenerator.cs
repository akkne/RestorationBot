namespace RestorationBot.Helpers.Implementation;

using Abstract;
using Contracts;

public class CallbackGenerator : ICallbackGenerator
{
    public string GenerateCallbackOnChoosingRestorationStep(int restorationStepIndex)
    {
        return $"restorationStep/{restorationStepIndex}";
    }

    public string GenerateCallbackOnGetExercise(ExerciseMessageInformation messageInformation)
    {
        return $"restorationStep/{messageInformation.RestorationStep}/{messageInformation.ExerciseIndex}";
    }
}