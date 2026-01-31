using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections;

public partial class Interactable : Area2D
{
    //All required flags to show up in-game
    [Export] private Array<PlayerFlag> requiredFlags = new Array<PlayerFlag>();
    [Export] private AudioStreamPlayer2D audioStreamPlayer;

    public override void _Ready()
    {
        if (audioStreamPlayer == null)
        {
            Logger.Fatal("AudioStreamPlayer is not assigned on Interactable");
        }

        base._Ready();
    }

    //TODO RIP performance, can probably be on a 1sec timer
    public override void _Process(double delta)
    {
        var flags = UiManager.Instance.GetPlayer().GetPlayerFlags();
        foreach (var flag in requiredFlags)
        {
            //if even one doesn't match, we hide the item
            if (UiManager.Instance.GetPlayer().GetPlayerFlags().GetFlag(flag.key) != flag.value)
            {
                Hide();
                Monitorable = false;
                Monitoring = false;
                return;
            }

            //otherwise we make it show
            Show();
            Monitorable = true;
            Monitoring = true;
        }

        base._Process(delta);
    }

    public virtual void OnInteract(PlayerCharacter playerCharacter)
    {
        audioStreamPlayer.Play();
    }

    protected virtual void DoHide()
    {
        this.Monitorable = false;
        this.Monitoring = false;
        this.Hide();
        foreach (var child in GetChildren())
        {
            if (child is CanvasItem canvasChild)
            {
                canvasChild.Visible = false;
            }

            if (child is CollisionShape2D collisionShape2D)
            {
                collisionShape2D.Disabled = true;
            }
        }
    }
}