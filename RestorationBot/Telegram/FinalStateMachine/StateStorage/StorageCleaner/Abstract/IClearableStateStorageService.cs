namespace RestorationBot.Telegram.FinalStateMachine.StateStorage.StorageCleaner.Abstract;

public interface IClearableStateStorageService
{
    void RemoveAllStates();
}