using Godot;
using Godot.Collections;

public partial class DialogueLine : GodotObject
{
    public int ID;
    public string Line;
    public int ItemId;
    public int Amount;
    public string AudioPath;
    public Array<DialogueConditionGroup> NextLines = new Array<DialogueConditionGroup>();
    public Array<DialogueConditionGroup> FontConditions = new Array<DialogueConditionGroup>();
    public Array<PlayerFlag> ChangeFlags = new Array<PlayerFlag>();

    public DialogueLine(int ID, string Line, int itemId, int amount)
    {
        NextLines = new Array<DialogueConditionGroup>();
        FontConditions = new Array<DialogueConditionGroup>();
        this.ID = ID;
        this.Line = Line;
        this.ItemId = itemId;
        this.Amount = amount;
    }

    //Returns 0 if none found or acceptable, so close the dialogue box
    public int GetNextLine()
    {
        foreach (var conditionGroup in NextLines)
        {
            if (conditionGroup.AreConditionsTrue())
            {
                Logger.DebugInfo("Next Line {0} chosen from current line {1}", conditionGroup.NextLineID, ID);
                return conditionGroup.NextLineID;
            }
            Logger.DebugInfo("Next Line {0} NOT chosen from current line {1}", conditionGroup.NextLineID, ID);
        }

        return 0;
    }

    public void PostLine()
    {
        if (ItemId != 0 && Amount != 0) {
            UiManager.Instance.GetPlayer().GetInventory().AddItem(ItemId, Amount, "dialogue line "+ ID);
        }

        foreach (var flag in ChangeFlags)
        {
            UiManager.Instance.GetPlayer().GetPlayerFlags().SetFlag(flag.key, flag.value);
        }
    }
}