namespace RestorationBot.Telegram.FinalStateMachine.StateStorage.Particular.Implementation;

using System.Collections.Concurrent;
using Abstract.Certain;
using States.Implementation;
using StorageCleaner.Abstract;

public class UserHavingPainStateStorageService : IUserHavingPainStateStorageService,
                                                 IClearableStateStorageService
{
    private readonly ConcurrentDictionary<long, HavingPainState> _userState = new();

    public void RemoveAllStates()
    {
        _userState.Clear();
    }

    public HavingPainState GetOrAddState(long userId)
    {
        return _userState.GetOrAdd(userId, id => new HavingPainState(id));
    }

    public void TryRemove(long userId)
    {
        _userState.TryRemove(userId, out _);
    }
}