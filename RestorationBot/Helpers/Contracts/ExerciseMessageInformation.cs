namespace RestorationBot.Helpers.Contracts;

using Shared.Enums;

public class ExerciseMessageInformation
{
    public RestorationSteps RestorationStep { get; set; }
    public int ExerciseIndex { get; set; }

    public static ExerciseMessageInformation Create(RestorationSteps restorationStep, int exerciseIndex)
    {
        return new ExerciseMessageInformation
        {
            RestorationStep = restorationStep,
            ExerciseIndex = exerciseIndex
        };
    }
}