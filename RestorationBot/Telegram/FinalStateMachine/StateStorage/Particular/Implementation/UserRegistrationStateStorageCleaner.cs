namespace RestorationBot.Telegram.FinalStateMachine.StateStorage.Particular.Implementation;

using System.Collections.Concurrent;
using Abstract.Certain;
using States.Implementation;
using StorageCleaner.Abstract;

public class UserRegistrationStateStorageCleaner : IUserRegistrationStateStorageService, IClearableStateStorageService
{
    private readonly ConcurrentDictionary<long, UserRegistrationState> _userState = new();

    public void RemoveAllStates()
    {
        _userState.Clear();
    }

    public UserRegistrationState GetOrAddState(long userId)
    {
        return _userState.GetOrAdd(userId, id => new UserRegistrationState(id));
    }

    public void TryRemove(long userId)
    {
        _userState.TryRemove(userId, out _);
    }
}