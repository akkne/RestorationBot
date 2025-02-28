namespace RestorationBot.Helpers.Abstract.MessageGenerators;

using RestorationBot.Helpers.Models.Response;
using RestorationBot.Shared.Enums;

public interface IRestorationStepMessageGenerator
{
    public TelegramMessageWithInlineKeyboard GetRestorationStepMessage(RestorationSteps restorationStep);
}