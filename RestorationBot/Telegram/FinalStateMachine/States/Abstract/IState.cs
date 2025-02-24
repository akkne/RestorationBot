namespace RestorationBot.Telegram.FinalStateMachine.States.Abstract;

using OperationsConfiguration.OperationStatesProfiles.UserRegistration;
using Stateless;

public interface IState<TStateProfile, TTriggerProfile>
{
    StateMachine<TStateProfile, TTriggerProfile> StateMachine { get; }
}