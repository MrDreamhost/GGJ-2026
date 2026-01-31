using Godot;
using System;

public partial class MainMenu : Node
{
    [Export] private Button ResetSaveButton = null;
    public override void _Ready()
    {
        if (ResetSaveButton == null)
        {
            Logger.Fatal("ResetSaveButton not assigned on MainMenu");
        }

        UpdateResetSaveButtonVisibility();
        base._Ready();
    }

    private void UpdateResetSaveButtonVisibility()
    {
        ResetSaveButton.Show();
        if (SaveManager.Instance.GetSaveData().Count == 0)
        {
            ResetSaveButton.Hide();
        }
    }

    public void OnPlayButtonPressed()
    {
        SceneManager.Instance.LoadGame();
        //TODO handle savefile
    }
    public void OnQuitButtonPressed()
    {
        GetTree().Quit();
    }

    public void OnResetSaveButtonPressed()
    {
        SaveManager.Instance.ResetSaveData();
        UpdateResetSaveButtonVisibility();
    }
    
    public void OnSettingsButtonPressed()
    {
        //TODO
        Logger.Info("TODO");
    }
    
    public void OnHowToPlayButtonPressed()
    {
        //TODO
        Logger.Info("TODO");
    }
}
