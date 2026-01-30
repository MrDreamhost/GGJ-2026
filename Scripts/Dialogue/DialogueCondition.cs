using Godot;
using System;

public partial class DialogueCondition : GodotObject
{
    public int NextLine;
    public int Value;
    public bool Invert;
    public virtual bool CheckCondition()
    {
        return true;
    }
}
