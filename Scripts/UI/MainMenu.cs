using Godot;
using System;

public partial class MainMenu : Node
{
    [Export] private Button ResetSaveButton = null;
    [Export] private HSlider MasterSlider = null;
    [Export] private HSlider SFXSlider = null;
    [Export] private HSlider MusicSlider = null;
    [Export] private AudioStreamPlayer2D audioPlayer = null;
    public override void _Ready()
    {
        if (ResetSaveButton == null)
        {
            Logger.Fatal("ResetSaveButton not assigned on MainMenu");
        }

        if (MasterSlider == null)
        {
            Logger.Fatal("MasterSlider not assigned on MainMenu");
        }
        
        if (SFXSlider == null)
        {
            Logger.Fatal("SFXSlider not assigned on MainMenu");
        }
        
        if (MusicSlider == null)
        {
            Logger.Fatal("MusicSlider not assigned on MainMenu");
        }

        if (audioPlayer == null)
        {
            Logger.Fatal("AudioPlayer is not assigned on MainMenu");
        }
        
        UpdateResetSaveButtonVisibility();
        
        var masterIndex = AudioServer.GetBusIndex("Master");
        var currentMasterDb = AudioServer.GetBusVolumeLinear(masterIndex);
        MasterSlider.Value = currentMasterDb;
        Logger.Info("Master DB = {0}", currentMasterDb);
        MasterSlider.ValueChanged += value => { OnVolumeSliderChanged(value, masterIndex); };
        
        var SFXIndex = AudioServer.GetBusIndex("SFX");
        var currentSFXDb = AudioServer.GetBusVolumeLinear(SFXIndex);
        SFXSlider.Value = currentSFXDb;
        SFXSlider.ValueChanged += value => { OnVolumeSliderChanged(value, SFXIndex); };
        
        var MusicIndex = AudioServer.GetBusIndex("Music");
        var currentMusicDb = AudioServer.GetBusVolumeLinear(MusicIndex);
        MusicSlider.Value = currentMusicDb;
        MusicSlider.ValueChanged += value => { OnVolumeSliderChanged(value, MusicIndex); };
        
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
    
    public void OnHowToPlayButtonPressed()
    {
        //TODO
        Logger.Info("TODO");
    }
    
    private void OnVolumeSliderChanged(double value, int busIndex)
    {
        // Convert 0.0-1.0 slider value to Decibels
        // We use Max(-80, ...) because LinearToDb(0) is -Infinity
        float dbValue = Mathf.LinearToDb((float)value);
        
        AudioServer.SetBusVolumeDb(busIndex, dbValue);
        Logger.Info("setting bus {0} volume to {1}", busIndex, dbValue);

        // Mute the bus entirely if the volume is at 0
        AudioServer.SetBusMute(busIndex, value <= 0.0001f);
    }
}
