using Godot;
using System;

public partial class TimerLabel : RichTextLabel
{
    public override void _Ready()
    {
        UiManager.Instance.RegisterTimerLabel(this);
        base._Ready();
        
    }
}
