namespace RestorationBot.Telegram.FinalStateMachine.States.Abstract;

using Stateless;

public interface IState<TStateProfile, TTriggerProfile>
{
    StateMachine<TStateProfile, TTriggerProfile> StateMachine { get; }
}