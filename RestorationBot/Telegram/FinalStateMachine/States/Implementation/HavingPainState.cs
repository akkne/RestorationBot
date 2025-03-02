namespace RestorationBot.Telegram.FinalStateMachine.States.Implementation;

using Abstract;
using OperationsConfiguration.OperationStatesProfiles.HavingPain;
using Stateless;

public class HavingPainState : IState<HavingPainStateProfile, HavingPainTriggerProfile>
{
    public HavingPainState(long userId)
    {
        UserId = userId;
        StateMachine =
            new StateMachine<HavingPainStateProfile, HavingPainTriggerProfile>(
                HavingPainStateProfile
                   .Ready);

        StateMachine.Configure(HavingPainStateProfile.Ready)
                    .Permit(HavingPainTriggerProfile.Begin, HavingPainStateProfile.PainValueEntering);

        StateMachine.Configure(HavingPainStateProfile.PainValueEntering)
                    .OnEntry(() => Console.WriteLine($"[{UserId}] Waiting for pain value entering..."))
                    .Permit(HavingPainTriggerProfile.PainValueEntered, HavingPainStateProfile.Completed);

        StateMachine.Configure(HavingPainStateProfile.Completed)
                    .OnEntry(() => Console.WriteLine($"[{UserId}] Pain value entered"));
    }

    public long UserId { get; }
    public int PainValue { get; set; }

    public StateMachine<HavingPainStateProfile, HavingPainTriggerProfile> StateMachine { get; }
}