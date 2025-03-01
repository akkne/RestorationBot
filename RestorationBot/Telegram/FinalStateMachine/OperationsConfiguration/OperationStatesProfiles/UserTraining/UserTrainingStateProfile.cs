namespace RestorationBot.Telegram.FinalStateMachine.OperationsConfiguration.OperationStatesProfiles.UserTraining;

public enum UserTrainingStateProfile
{
    Ready,
    PreHeartRateEntering,
    PreBloodPressureEntering,
    ExercisePointChoosing,
    ExerciseTypeChoosing,
    PostHeartRateEntering,
    PostBloodPressureEntering,
    Completed
}