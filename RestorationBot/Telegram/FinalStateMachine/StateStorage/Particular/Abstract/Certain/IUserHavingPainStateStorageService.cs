namespace RestorationBot.Telegram.FinalStateMachine.StateStorage.Particular.Abstract.Certain;

using Base;
using OperationsConfiguration.OperationStatesProfiles.HavingPain;
using States.Implementation;

public interface
    IUserHavingPainStateStorageService : IStateStorageService<HavingPainState, HavingPainStateProfile,
    HavingPainTriggerProfile>;