using Godot;
using System;

public partial class HudVignette : ColorRect
{
    public override void _Ready()
    {
        UiManager.Instance.RegisterHudVignette(this);
        base._Ready();
    }
}
