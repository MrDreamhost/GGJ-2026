using Godot;
using System;
using System.Threading.Tasks;

public partial class BlueFlame : AnimatedSprite2D
{
    [Export] private Interactable orb = null;

    public override void _Ready()
    {
        if (orb == null)
        {
            Logger.Fatal("BlueFlame does not have orb interactable assigned");
        }
        
        UiManager.Instance.RegisterBlueFlame(this);
        Hide();
        base._Ready();
        
    }

    public async void OnWin()
    {
        await Task.Delay(1800);
        Show();
        Play();
        await Task.Delay(1800);
        orb.Hide();
        Stop();
    }
}
