namespace RestorationBot.Telegram.FinalStateMachine.StateStorage.Particular.Implementation;

using System.Collections.Concurrent;
using Abstract.Certain;
using States.Implementation;
using StorageCleaner.Abstract;

public class UserTrainingClearableStateStorageService : IUserTrainingStateStorageService,
                                                        IClearableStateStorageService
{
    private readonly ConcurrentDictionary<long, UserTrainingState> _userState = new();

    public void RemoveAllStates()
    {
        _userState.Clear();
    }

    public UserTrainingState GetOrAddState(long userId)
    {
        return _userState.GetOrAdd(userId, id => new UserTrainingState(id));
    }

    public void TryRemove(long userId)
    {
        _userState.TryRemove(userId, out _);
    }
}