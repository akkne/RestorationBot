namespace RestorationBot.Telegram.FinalStateMachine.OperationsConfiguration.OperationStatesProfiles.UserTraining;

public enum UserTrainingStateProfile
{
    Ready,
    PreHeartRateEntering,
    PreBloodPressureEntering,
    ExerciseTypeChoosing,
    PostHeartRateEntering,
    PostBloodPressureEntering,
    Completed
}