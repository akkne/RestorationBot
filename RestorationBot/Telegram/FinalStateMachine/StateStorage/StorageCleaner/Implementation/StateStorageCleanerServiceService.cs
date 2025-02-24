namespace RestorationBot.Telegram.FinalStateMachine.StateStorage.StorageCleaner.Implementation;

using Abstract;

public class StateStorageCleanerServiceService : IStateStorageCleanerService
{
    private readonly IEnumerable<IClearableStateStorageService> _clearableStateStateStorageServices;

    public StateStorageCleanerServiceService(
        IEnumerable<IClearableStateStorageService> clearableStateStateStorageServices)
    {
        _clearableStateStateStorageServices = clearableStateStateStorageServices;
    }

    public void RemoveAllStates()
    {
        foreach (IClearableStateStorageService service in _clearableStateStateStorageServices)
            service.RemoveAllStates();
    }
}