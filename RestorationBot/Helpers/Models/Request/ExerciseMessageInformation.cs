namespace RestorationBot.Helpers.Models.Request;

using Shared.Enums;

public class ExerciseMessageInformation
{
    public RestorationSteps RestorationStep { get; set; }
    public int ExerciseType { get; set; }

    public static ExerciseMessageInformation Create(RestorationSteps restorationStep, int exerciseIndex)
    {
        return new ExerciseMessageInformation
        {
            RestorationStep = restorationStep,
            ExerciseType = exerciseIndex
        };
    }
}