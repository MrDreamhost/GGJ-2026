using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections;

public partial class Interactable : Area2D
{
    //All required flags to show up in-game
    [Export]private Array<PlayerFlag> requiredFlags = new Array<PlayerFlag>();

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
        
    }
}
