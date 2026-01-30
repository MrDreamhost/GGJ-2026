using Godot;
using Godot.Collections;

public partial class DialogueLine : GodotObject
{
    public int ID;
    public string Line;
    public int ItemId;
    public int Amount;
    public Array<DialogueCondition> NextLines = new Array<DialogueCondition>();
    public Array<DialogueCondition> FontConditions = new Array<DialogueCondition>();

    public DialogueLine(int ID, string Line, int itemId, int amount)
    {
        NextLines = new Array<DialogueCondition>();
        this.ID = ID;
        this.Line = Line;
        this.ItemId = itemId;
        this.Amount = amount;
    }

    //Returns 0 if none found or acceptable, so close the dialogue box
    public int GetNextLine()
    {
        foreach (var line in NextLines)
        {
            if (line.IsConditionTrue())
            {
                Logger.DebugInfo("Next Line {0} chosen from current line {1}", line.NextLineID, ID);
                return line.NextLineID;
            }
            Logger.DebugInfo("Next Line {0} NOT chosen from current line {1}", line.NextLineID, ID);
        }

        return 0;
    }

    public void PostLine()
    {
        if (ItemId != 0 && Amount != 0) {
            UiManager.Instance.GetPlayer().GetInventory().AddItem(ItemId, Amount, "dialogue line "+ ID);
        }
    }
}