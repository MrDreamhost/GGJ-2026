namespace GGJ2026.Scripts.Dialogue;

public partial class FlagDialogueCondition : DialogueCondition
{
    public override bool IsConditionTrue()
    {
        if (Invert)
        {
            return !UiManager.Instance.GetPlayer().GetPlayerFlags().GetFlag(FlagValue);
        }

        return UiManager.Instance.GetPlayer().GetPlayerFlags().GetFlag(FlagValue);
    }
}