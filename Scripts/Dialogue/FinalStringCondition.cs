
public partial class FinalStringCondition: DialogueCondition
{
    public override bool IsConditionTrue()
    {
        return FinalStringValue.Contains(UiManager.Instance.GetDialogueManager().FinalChoiceString);
    }
}