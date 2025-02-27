namespace RestorationBot.Telegram.FinalStateMachine.States.Implementation;

using Abstract;
using OperationsConfiguration.OperationStatesProfiles.UserTraining;
using Stateless;

public class UserTrainingState : IState<UserTrainingStateProfile, UserTrainingTriggerProfile>
{
    public UserTrainingState(long userId)
    {
        UserId = userId;
        StateMachine =
            new StateMachine<UserTrainingStateProfile, UserTrainingTriggerProfile>(UserTrainingStateProfile.Ready);

        StateMachine.Configure(UserTrainingStateProfile.Ready)
                    .Permit(UserTrainingTriggerProfile.Begin, UserTrainingStateProfile.PreHeartRateEntering);

        StateMachine.Configure(UserTrainingStateProfile.PreHeartRateEntering)
                    .OnEntry(() => Console.WriteLine($"[{UserId}] Waiting for the pre heart rate entering..."))
                    .Permit(UserTrainingTriggerProfile.PreHeartRateEntered,
                         UserTrainingStateProfile.PreBloodPressureEntering);

        StateMachine.Configure(UserTrainingStateProfile.PreBloodPressureEntering)
                    .OnEntry(() => Console.WriteLine($"[{UserId}] Waiting for the pre blood pressure entering..."))
                    .Permit(UserTrainingTriggerProfile.PreBloodPressureEntered,
                         UserTrainingStateProfile.ExerciseTypeChoosing);

        StateMachine.Configure(UserTrainingStateProfile.ExerciseTypeChoosing)
                    .OnEntry(() => Console.WriteLine($"[{UserId}] Waiting for the exercise type choosing..."))
                    .Permit(UserTrainingTriggerProfile.ExerciseTypeChosen,
                         UserTrainingStateProfile.PostHeartRateEntering);

        StateMachine.Configure(UserTrainingStateProfile.PostHeartRateEntering)
                    .OnEntry(() => Console.WriteLine($"[{UserId}] Waiting for the post heart rate entering..."))
                    .Permit(UserTrainingTriggerProfile.PostHeartRateEntered,
                         UserTrainingStateProfile.PostBloodPressureEntering);

        StateMachine.Configure(UserTrainingStateProfile.PostBloodPressureEntering)
                    .OnEntry(() => Console.WriteLine($"[{UserId}] Waiting for the post blood pressure entering..."))
                    .Permit(UserTrainingTriggerProfile.PostBloodPressureEntered,
                         UserTrainingStateProfile.Completed);

        StateMachine.Configure(UserTrainingStateProfile.Completed)
                    .OnEntry(() => Console.WriteLine($"[{UserId}] Training completed!"));
    }

    public long UserId { get; }
    public int ExerciseTypeChosen { get; set; }
    public double PreHeartRate { get; set; }
    public double PreBloodPressure { get; set; }
    public double PostHeartRate { get; set; }
    public double PostBloodPressure { get; set; }

    public StateMachine<UserTrainingStateProfile, UserTrainingTriggerProfile> StateMachine { get; }
}