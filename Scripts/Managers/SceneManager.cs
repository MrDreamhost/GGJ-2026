using Godot;
using System;

public partial class SceneManager : Node
{
    public static SceneManager Instance { get; private set; }

    private string mainMenuScenePath = "res://Scenes/MainMenu.tscn";
    private string gameScenePath = "res://Scenes/MainScene.tscn";
    
    private PackedScene mainMenuScene = null;
    private PackedScene gameScene = null;

    public override void _Ready()
    {
        Instance = this;
        
        mainMenuScene = GD.Load<PackedScene>(mainMenuScenePath);
        if (mainMenuScene == null)
        {
            Logger.Fatal("failed to load main menu scene from path {0}", mainMenuScenePath);
        }

        gameScene = GD.Load<PackedScene>(gameScenePath);
        if (gameScene == null)
        {
            Logger.Fatal("failed to load game scene from path {0}", gameScenePath);
        }

        base._Ready();
    }

    public void LoadMainMenu()
    {
        var error=GetTree().ChangeSceneToPacked(mainMenuScene);
        if ( error != Error.Ok)
        {
            Logger.Fatal("failed to load main menu scene with path {0} and error {1}", gameScene ,error);
            return;
        }
    }

    public void LoadGame()
    {
       var error = GetTree().ChangeSceneToPacked(gameScene);
       if ( error != Error.Ok)
       {
           Logger.Fatal("failed to load gameScene with path {0} and error {1}", gameScene ,error);
           return;
       }
    }
}
