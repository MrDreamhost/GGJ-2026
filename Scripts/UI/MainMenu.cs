using Godot;
using System;

public partial class MainMenu : Node
{
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
        //TODO
        Logger.Info("TODO");
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
