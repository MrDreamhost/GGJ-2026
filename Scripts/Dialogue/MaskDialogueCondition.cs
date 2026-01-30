using Godot;
using System;

public partial class MaskDialogueCondition : DialogueCondition
{
    public override bool IsConditionTrue()
    {
        //TODO check for player mask
        var maskID = 2;

        if (Invert)
        {
            return Value != maskID;
        }

        return Value == maskID;
    }
}
