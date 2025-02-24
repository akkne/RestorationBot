namespace RestorationBot.Telegram.FinalStateMachine.States.Implementation;

using Abstract;
using OperationsConfiguration.OperationStatesProfiles.UserRegistration;
using Shared.Enums;
using Stateless;

public class UserRegistrationState : IState<UserRegistrationStateProfile, UserRegistrationTriggerProfile>
{
    public UserRegistrationState(long userId)
    {
        UserId = userId;
        StateMachine =
            new StateMachine<UserRegistrationStateProfile, UserRegistrationTriggerProfile>(UserRegistrationStateProfile
               .Ready);

        StateMachine.Configure(UserRegistrationStateProfile.Ready)
                    .Permit(UserRegistrationTriggerProfile.Begin, UserRegistrationStateProfile.RestorationStepChoosing);

        StateMachine.Configure(UserRegistrationStateProfile.RestorationStepChoosing)
                    .OnEntry(() => Console.WriteLine($"[{UserId}] Waiting for restoration step..."))
                    .Permit(UserRegistrationTriggerProfile.RestorationStepEntered,
                         UserRegistrationStateProfile.AgeEntering);

        StateMachine.Configure(UserRegistrationStateProfile.AgeEntering)
                    .OnEntry(() => Console.WriteLine($"[{UserId}] Waiting for age..."))
                    .Permit(UserRegistrationTriggerProfile.AgeEntered, UserRegistrationStateProfile.SexChoosing);

        StateMachine.Configure(UserRegistrationStateProfile.SexChoosing)
                    .OnEntry(() => Console.WriteLine($"[{UserId}] Waiting for sex..."))
                    .Permit(UserRegistrationTriggerProfile.SexEntered, UserRegistrationStateProfile.Completed);

        StateMachine.Configure(UserRegistrationStateProfile.Completed)
                    .OnEntry(() => Console.WriteLine($"[{UserId}] Registration completed!"));
    }

    public long UserId { get; }
    public RestorationSteps RestorationStep { get; set; }
    public int Age { get; set; }
    public Sex Gender { get; set; }

    public StateMachine<UserRegistrationStateProfile, UserRegistrationTriggerProfile> StateMachine { get; }
}