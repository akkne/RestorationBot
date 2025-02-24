namespace RestorationBot.Telegram.FinalStateMachine.StateStorage.Particular.Abstract.Certain;

using Base;
using OperationsConfiguration.OperationStatesProfiles.UserTraining;
using States.Implementation;

public interface IUserTrainingStateStorageService : IStateStorageService<UserTrainingState, UserTrainingStateProfile,
    UserTrainingTriggerProfile>;