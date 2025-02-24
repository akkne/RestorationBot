namespace RestorationBot.Telegram.FinalStateMachine.StateStorage.StorageCleaner.Implementation;

using Particular.Abstract.Base;
using Particular.Abstract.Certain;
using RestorationBot.Telegram.FinalStateMachine.OperationsConfiguration.OperationStatesProfiles.UserRegistration;
using RestorationBot.Telegram.FinalStateMachine.States.Implementation;
using RestorationBot.Telegram.FinalStateMachine.StateStorage.Particular.Abstract;
using RestorationBot.Telegram.FinalStateMachine.StateStorage.StorageCleaner.Abstract;

public class StateStorageCleanerServiceService : IStateStorageCleanerService
{
    private readonly IEnumerable<IClearableStateStorageService> _clearableStateStateStorageServices;

    public StateStorageCleanerServiceService(IEnumerable<IClearableStateStorageService> clearableStateStateStorageServices)
    {
        _clearableStateStateStorageServices = clearableStateStateStorageServices;
    }
    
    public void RemoveAllStates()
    {
        foreach (IClearableStateStorageService service in _clearableStateStateStorageServices)
        {
            service.RemoveAllStates();
        }
    }
}