using Godot;
using System;

public partial class PauseScreen : Node
{
    [Export] private CanvasLayer canvasLayer;
    [Export] private HSlider MasterSlider = null;
    [Export] private HSlider SFXSlider = null;
    [Export] private HSlider MusicSlider = null;

    public override void _Ready()
    {
        if (canvasLayer == null)
        {
            Logger.Fatal("CanvasLayer not assigned on pause screen");
        }
        
        if (MasterSlider == null)
        {
            Logger.Fatal("MasterSlider not assigned on pause screen");
        }
        
        if (SFXSlider == null)
        {
            Logger.Fatal("SFXSlider not assigned on pause screen");
        }
        
        if (MusicSlider == null)
        {
            Logger.Fatal("MusicSlider not assigned on pause screen");
        }

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

    public void OnResumeButton()
    {
        DoHide();
        UiManager.Instance.GetPlayer()?.SetState(PlayerCharacter.State.EIdle, "Resuming game from pause menu");
    }


    public void OnRestartButton()
    {
        DoHide();
        UiManager.Instance.GetPlayer().SaveGame();
        SceneManager.Instance.LoadGame();
    }

    public void OnBackToMenuButton()
    {
      UiManager.Instance.GetPlayer().SaveGame();
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
