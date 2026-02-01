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

        var saveData = SaveManager.Instance.GetSaveData();
        
        var masterIndex = AudioServer.GetBusIndex("Master");
        var currentMasterDb = AudioServer.GetBusVolumeLinear(masterIndex);
        MasterSlider.Value = currentMasterDb;
        
        var masterBusSaveName = "audio_masterSlider";
        MasterSlider.ValueChanged += value => { OnVolumeSliderChanged(value, masterIndex, masterBusSaveName); };
        if (saveData.ContainsKey(masterBusSaveName))
        {
            MasterSlider.SetValue(Convert.ToDouble(saveData[masterBusSaveName]));
        }
        
        var SFXIndex = AudioServer.GetBusIndex("SFX");
        var currentSFXDb = AudioServer.GetBusVolumeLinear(SFXIndex);
        SFXSlider.Value = currentSFXDb;
        
        var SFXBusSaveName = "audio_SFXSlider";
        SFXSlider.ValueChanged += value => { OnVolumeSliderChanged(value, SFXIndex, SFXBusSaveName); };
        if (saveData.ContainsKey(SFXBusSaveName))
        {
            SFXSlider.SetValue(Convert.ToDouble(saveData[SFXBusSaveName]));
        }
        
        var MusicIndex = AudioServer.GetBusIndex("Music");
        var currentMusicDb = AudioServer.GetBusVolumeLinear(MusicIndex);
        MusicSlider.Value = currentMusicDb;
        var MusicBusSaveName = "audio_MusicSlider";
        MusicSlider.ValueChanged += value => { OnVolumeSliderChanged(value, MusicIndex, MusicBusSaveName); };
        if (saveData.ContainsKey(MusicBusSaveName))
        {
            MusicSlider.SetValue(Convert.ToDouble(saveData[MusicBusSaveName]));
        }
        
        
        base._Ready();
    }

    private void UpdateResetSaveButtonVisibility()
    {
        ResetSaveButton.Show();
        var saveData = SaveManager.Instance.GetSaveData();
        foreach (var keyValue in saveData)
        {
            if (!keyValue.Key.Contains("audio"))
            {
                return;
            }
        }
            ResetSaveButton.Hide();
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
    
    private void OnVolumeSliderChanged(double value, int busIndex, string busName)
    {
        float dbValue = Mathf.LinearToDb((float)value);
        
        AudioServer.SetBusVolumeDb(busIndex, dbValue);
        Logger.Info("setting bus {0} volume to {1}", busIndex, dbValue);

        AudioServer.SetBusMute(busIndex, value <= 0.0001f);

        var saveData = SaveManager.Instance.GetSaveData();
        saveData[busName] = value.ToString();
        SaveManager.Instance.UpdateSaveData(saveData);
    }
}
