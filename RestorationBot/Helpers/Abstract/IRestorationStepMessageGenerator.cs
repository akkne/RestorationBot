namespace RestorationBot.Helpers.Abstract;

using Contracts;
using Shared.Enums;

public interface IRestorationStepMessageGenerator
{
    public TelegramResponseMessageInformation GetRestorationStepMessage(RestorationSteps restorationStep);
}