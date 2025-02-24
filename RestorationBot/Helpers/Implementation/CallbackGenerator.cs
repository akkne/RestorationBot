namespace RestorationBot.Helpers.Implementation;

using System.Text.RegularExpressions;
using Abstract;
using Contracts;
using Shared.Enums;

public class CallbackGenerator : ICallbackGenerator
{
    private readonly Regex _callbackRegexOnChangingRestorationStep;
    private readonly Regex _callbackRegexOnChoosingRestorationStep;
    private readonly Regex _callbackRegexOnGetExercise;
    private readonly Regex _callbackRegexOnHavingProblems;
    private readonly Regex _callbackRegexOnHavingCertainProblem;
    private readonly Regex _callbackRegexOnEnteringSex;
    
    public CallbackGenerator()
    {
        _callbackRegexOnChangingRestorationStep = new Regex(@"restorationStep/update/(?<index>\d+)$");
        _callbackRegexOnHavingCertainProblem = new Regex("exercise/havingCertainProblem/(?<problemIndex>\\d+)$");
        _callbackRegexOnHavingProblems = new Regex("exercise/havingProblem/(?<hasProblem>True|False)$");
        _callbackRegexOnChoosingRestorationStep = new Regex(@"restorationStep/choose/(?<index>\d+)$");
        _callbackRegexOnGetExercise = new Regex(@"getExercise/(?<step>\d+)/(?<index>\d+)$");
        _callbackRegexOnEnteringSex = new Regex("sex/choose/(?<index>\\d+)$");
    }

    public Regex GetCallbackRegexOnChoosingRestorationStep()
    {
        return _callbackRegexOnChoosingRestorationStep;
    }
    
    public string GenerateCallbackOnChoosingRestorationStep(RestorationSteps restorationStep)
    {
        return $"restorationStep/choose/{(int) restorationStep}";
    }
    
    public Regex GetCallbackRegexOnGetExercise()
    {
        return _callbackRegexOnGetExercise;
    }

    public string GenerateCallbackOnHavingProblem(bool hasProblem)
    {
        return $"exercise/havingProblem/{hasProblem}";
    }

    public Regex GetCallbackRegexOnHavingProblem()
    {
        return _callbackRegexOnHavingProblems;
    }

    public string GenerateCallbackOnHavingCertainProblem(int problemIndex)
    {
        return $"exercise/havingCertainProblem/{problemIndex}";
    }

    public Regex GetCallbackRegexOnHavingCertainProblem()
    {
        return _callbackRegexOnHavingCertainProblem;
    }

    public string GenerateCallbackOnChangingRestorationStep(RestorationSteps restorationStep)
    {
        return $"restorationStep/update/{(int) restorationStep}";
    }

    public Regex GetCallbackRegexOnChangingRestorationStep()
    {
        return _callbackRegexOnChangingRestorationStep;
    }

    public string GenerateCallbackOnChoosingSex(Sex sex)
    {
        return $"sex/choose/{(int) sex}";
    }

    public Regex GetCallbackRegexOnChoosingSex()
    {
        return _callbackRegexOnEnteringSex;
    }

    public string GenerateCallbackOnGetExercise(ExerciseMessageInformation messageInformation)
    {
        return $"getExercise/{(int) messageInformation.RestorationStep}/{messageInformation.ExerciseIndex}";
    }
}