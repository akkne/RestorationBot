namespace RestorationBot.Helpers.Abstract;

using Models.Response;
using Shared.Enums;

public interface IRestorationStepMessageGenerator
{
    public TelegramMessageWithInlineKeyboard GetRestorationStepMessage(RestorationSteps restorationStep);
}