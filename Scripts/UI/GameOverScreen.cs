using Godot;
using System;

public partial class GameOverScreen : Node
{
    [Export] private CanvasLayer canvasLayer;

    public override void _Ready()
    {
        if (canvasLayer == null)
        {
            Logger.Fatal("CanvasLayer not assigned on pause screen");
        }

        base._Ready();
        
    }

    public void OnRestartButton()
    {
        DoHide();
        SceneManager.Instance.LoadGame();
    }

    public void OnSettingsButton()
    {
        //TODO
        Logger.Info("TODO");
    }

    public void OnHowToPlayButton()
    {
        //TODO
        Logger.Info("TODO");
    }

    public void OnBackToMenuButton()
    {
        SceneManager.Instance.LoadMainMenu();
    }

    public void OnQuitButton()
    {
        GetTree().Quit();
    }


    public void DoHide()
    {
        canvasLayer.Hide();
    }

    public void DoShow()
    {
        canvasLayer.Show();
    }
}
