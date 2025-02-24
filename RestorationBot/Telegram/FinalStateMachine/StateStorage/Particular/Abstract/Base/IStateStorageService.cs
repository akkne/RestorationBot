namespace RestorationBot.Telegram.FinalStateMachine.StateStorage.Particular.Abstract.Base;

using RestorationBot.Telegram.FinalStateMachine.States.Abstract;

public interface IStateStorageService<out TState, TStateProfile, TTriggerProfile>
    where TState : class, IState<TStateProfile, TTriggerProfile>
{
    TState GetOrAddState(long userId);
    void TryRemove(long userId);
}