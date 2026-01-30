using Godot;
using System;

public partial class DialogueCondition : GodotObject
{
    public int NextLineID;
    public int Value;
    public bool Invert;
    public virtual bool CheckCondition()
    {
        return true;
    }
}
