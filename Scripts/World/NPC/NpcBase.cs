using Godot;
using System;

public partial class NpcBase : Interactable
{
    [Export] private int dialogueSequence;

    public override void _Ready()
    {
        if (dialogueSequence == 0)
        {
            Logger.DebugError("No dialogue sequence assigned to character");
        }

        base._Ready();
    }

    public override void OnInteract(PlayerCharacter playerCharacter)
    {
        Logger.Info("Interacted with NPC");
        UiManager.Instance.StartDialogueSequence(dialogueSequence);
        base.OnInteract(playerCharacter);
    }
}