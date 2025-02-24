namespace RestorationBot.Telegram.FinalStateMachine.States.Implementation;

using Abstract;
using OperationsConfiguration.OperationStatesProfiles.UserTraining;
using Stateless;

public class UserTrainingState : IState<UserTrainingStateProfile, UserTrainingTriggerProfile>
{
    public long UserId { get; }
    public int ExerciseTypeChosen { get; set; }
    public double HeartRate { get; set; }
    public double BloodPressure { get; set; }
    
    public StateMachine<UserTrainingStateProfile, UserTrainingTriggerProfile> StateMachine { get; }

    public UserTrainingState(long userId)
    {
        UserId = userId;
        StateMachine =
            new StateMachine<UserTrainingStateProfile, UserTrainingTriggerProfile>(UserTrainingStateProfile.Ready);
        
        StateMachine.Configure(UserTrainingStateProfile.Ready)
                    .Permit(UserTrainingTriggerProfile.Begin, UserTrainingStateProfile.ExerciseTypeChoosing);
        
        StateMachine.Configure(UserTrainingStateProfile.ExerciseTypeChoosing)
                    .OnEntry(() => Console.WriteLine($"[{UserId}] Waiting for the exercise type choosing..."))
                    .Permit(UserTrainingTriggerProfile.ExerciseTypeChosen,
                         UserTrainingStateProfile.HeartRateEntering);
        
        StateMachine.Configure(UserTrainingStateProfile.HeartRateEntering)
                    .OnEntry(() => Console.WriteLine($"[{UserId}] Waiting for the heart rate entering..."))
                    .Permit(UserTrainingTriggerProfile.HeartRateEntered,
                         UserTrainingStateProfile.BloodPressureEntering);
        
        StateMachine.Configure(UserTrainingStateProfile.BloodPressureEntering)
                    .OnEntry(() => Console.WriteLine($"[{UserId}] Waiting for the blood pressure entering..."))
                    .Permit(UserTrainingTriggerProfile.BloodPressureEntered,
                         UserTrainingStateProfile.Completed);
        
        StateMachine.Configure(UserTrainingStateProfile.Completed)
                    .OnEntry(() => Console.WriteLine($"[{UserId}] Training completed!"));
    }
}