using Godot;
using System;

public partial class CharacterSelect : Control
{
    [Export] private CanvasLayer canvasLayer = null;
    [Export] private ColorRect artisanDisableRect = null;
    [Export] private ColorRect mageDisableRect = null;
    
    public override void _Ready()
    {
        if (canvasLayer == null)
        {
            Logger.Fatal("canvasLayer not assigned on CharacterSelect screen");
        }
        if (artisanDisableRect == null)
        {
            Logger.Fatal("artisanDisableRect not assigned on CharacterSelect screen");
        }
        
        if (mageDisableRect == null)
        {
            Logger.Fatal("mageDisableRect not assigned on CharacterSelect screen");
        }
        
        artisanDisableRect.Show();
        mageDisableRect.Show();
        if (UiManager.Instance.GetPlayer().IsMaskUnlocked(1))
        {
            artisanDisableRect.Hide();
        }
        
        if (UiManager.Instance.GetPlayer().IsMaskUnlocked(2))
        {
            mageDisableRect.Hide();
        }
        base._Ready();
    }

    public void OnStrangerPicked()
    {
        UiManager.Instance.GetPlayer().SetCurMask(0);
        canvasLayer.Hide();
    }

    public void OnArtisanPicked()
    {
        UiManager.Instance.GetPlayer().SetCurMask(1);
        canvasLayer.Hide();
        
    }

    public void OnMagePicked()
    {
        UiManager.Instance.GetPlayer().SetCurMask(2);
        canvasLayer.Hide();
    }
}
