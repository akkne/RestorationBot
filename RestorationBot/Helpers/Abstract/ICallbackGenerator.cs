namespace RestorationBot.Helpers.Abstract;

using System.Text.RegularExpressions;
using Models.Request;
using Shared.Enums;

public interface ICallbackGenerator
{
    string GenerateCallbackOnChoosingRestorationStep(RestorationSteps restorationStep);
    Regex GetCallbackRegexOnChoosingRestorationStep();

    string GenerateCallbackOnGetExercise(ExerciseMessageInformation messageInformation, int exercisePoint);
    Regex GetCallbackRegexOnGetExercise();

    string GenerateCallbackOnChoosingExercisePoint(int point);
    Regex GetCallbackRegexOnChoosingExercisePoint();

    string GenerateCallbackOnHavingProblem(bool hasProblem);
    Regex GetCallbackRegexOnHavingProblem();

    string GenerateCallbackOnHavingCertainProblem(int problemIndex);
    Regex GetCallbackRegexOnHavingCertainProblem();

    string GenerateCallbackOnChangingRestorationStep(RestorationSteps restorationStep);
    Regex GetCallbackRegexOnChangingRestorationStep();

    string GenerateCallbackOnChoosingSex(Sex sex);
    Regex GetCallbackRegexOnChoosingSex();
}