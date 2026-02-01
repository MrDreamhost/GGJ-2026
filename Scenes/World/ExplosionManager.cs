using Godot;
using System;

public partial class ExplosionManager : Node
{
    public override void _Ready()
    {
        UiManager.Instance.RegisterExplosionManager(this);
        base._Ready();
    }

    public void StartExplosions(int maxDelayMS)
    {
    }
}
