using Godot;
using System;

public partial class PauseScreen : Node
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

    public void OnResumeButton()
    {
        DoHide();
        UiManager.Instance.GetPlayer()?.SetState(PlayerCharacter.State.EIdle, "Resuming game from pause menu");
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

    public void FlipFlop()
    {
        canvasLayer.SetVisible(!canvasLayer.Visible);
    }

    public bool IsGamePaused()
    {
        return canvasLayer.Visible;
    }
}
