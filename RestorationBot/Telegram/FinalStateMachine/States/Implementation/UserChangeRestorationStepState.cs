namespace RestorationBot.Telegram.FinalStateMachine.States.Implementation;

using Abstract;
using OperationsConfiguration.OperationStatesProfiles.UserChangeRestorationStep;
using Shared.Enums;
using Stateless;

public class UserChangeRestorationStepState : IState<UserChangeRestorationStepStateProfile,
    UserChangeRestorationStepTriggerProfile>
{
    public UserChangeRestorationStepState(long userId)
    {
        UserId = userId;
        StateMachine =
            new StateMachine<UserChangeRestorationStepStateProfile, UserChangeRestorationStepTriggerProfile>(
                UserChangeRestorationStepStateProfile
                   .Ready);

        StateMachine.Configure(UserChangeRestorationStepStateProfile.Ready)
                    .Permit(UserChangeRestorationStepTriggerProfile.Begin,
                         UserChangeRestorationStepStateProfile.RestorationStepUpdating);

        StateMachine.Configure(UserChangeRestorationStepStateProfile.RestorationStepUpdating)
                    .OnEntry(() => Console.WriteLine($"[{UserId}] Waiting for restoration step updating..."))
                    .Permit(UserChangeRestorationStepTriggerProfile.RestorationStepUpdated,
                         UserChangeRestorationStepStateProfile.Completed);

        StateMachine.Configure(UserChangeRestorationStepStateProfile.Completed)
                    .OnEntry(() => Console.WriteLine($"[{UserId}] Changed restoration step updating"));
    }

    public long UserId { get; }
    public RestorationSteps RestorationStep { get; set; }

    public StateMachine<UserChangeRestorationStepStateProfile, UserChangeRestorationStepTriggerProfile> StateMachine
    {
        get;
    }
}