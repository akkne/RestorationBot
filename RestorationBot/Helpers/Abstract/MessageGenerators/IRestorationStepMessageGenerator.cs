namespace RestorationBot.Helpers.Abstract.MessageGenerators;

using Models.Response;
using Shared.Enums;

public interface IRestorationStepMessageGenerator
{
    public TelegramMessageWithInlineKeyboard GetPhysicalTrainingMessage(RestorationSteps restorationStep);
    public TelegramMessageWithInlineKeyboard GetIdeomotorTrainingMessage(RestorationSteps restorationStep);
}