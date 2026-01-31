using Godot;
using System;

public partial class MaskDialogueCondition : DialogueCondition
{
    public override bool IsConditionTrue()
    {
        var maskId = UiManager.Instance.GetPlayer().GetCurMask();
        if (Invert)
        {
            return Value != maskId;
        }

        return Value == maskId;
    }
}
