using Godot;
using Godot.Collections;


public partial class DialogueConditionGroup : GodotObject
{
    public int NextLineID;
    public string Font;
    public Array<DialogueCondition> Conditions;

    public DialogueConditionGroup()
    {
        Conditions = new Array<DialogueCondition>();
    }
    public bool AreConditionsTrue()
    {
        if (Conditions.Count == 0)
        {
            return true;
        }

        foreach (var condition in Conditions)
        {
            if (!condition.IsConditionTrue())
            {
                return false;
            }
        }
        return true;
    }
}