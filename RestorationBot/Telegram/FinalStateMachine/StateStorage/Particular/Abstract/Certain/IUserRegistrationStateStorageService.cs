namespace RestorationBot.Telegram.FinalStateMachine.StateStorage.Particular.Abstract.Certain;

using Base;
using OperationsConfiguration.OperationStatesProfiles.UserRegistration;
using States.Implementation;

public interface IUserRegistrationStateStorageService : IStateStorageService<UserRegistrationState,
    UserRegistrationStateProfile, UserRegistrationTriggerProfile>;