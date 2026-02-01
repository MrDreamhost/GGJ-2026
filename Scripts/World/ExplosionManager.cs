using Godot;
using System;
using System.Threading.Tasks;

public partial class ExplosionManager : Node
{
    public override void _Ready()
    {
        UiManager.Instance.RegisterExplosionManager(this);
        foreach (var child in GetChildren())
        {
            if (child is AnimatedSprite2D sprite)
            {
                sprite.Hide();
            }
        }
        base._Ready();
    }

    public void StartExplosions(int MaxDelayMs)
    {
        UiManager.Instance.GetInteractionPanel().DoHide();
        foreach (var child in GetChildren())
        {
            if (child is AnimatedSprite2D sprite)
            {
                DoExplosion(sprite, GD.RandRange(0, MaxDelayMs));
            }
        }
    }

    public async void DoExplosion(AnimatedSprite2D sprite, int delayMS)
    {
        await Task.Delay(3250);
        await Task.Delay(delayMS);
        sprite.Show();
        sprite.Play("default");
    }
}
