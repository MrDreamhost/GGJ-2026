using Godot;
using System;

public partial class NpcBase : Interactable
{
	[Export] private int dialogueSequence;
	[Export] private string interactionText = "NoShow";

	public override void _Ready()
	{
		if (dialogueSequence == 0)
		{
			Logger.DebugError("No dialogue sequence assigned to character");
		}
		
		this.AreaEntered += OnAreaEntered;
		this.AreaExited += OnAreaLeft;

		base._Ready();
	}

	public override void OnInteract(PlayerCharacter playerCharacter)
	{
		Logger.Info("Interacted with NPC");
		UiManager.Instance.StartDialogueSequence(dialogueSequence);
		base.OnInteract(playerCharacter);
	}
	
	private void OnAreaEntered(Area2D area)
	{
		Logger.Info("Area Entered");
		if (area.Owner is PlayerCharacter character)
		{
			Logger.Info("Character Area Entered");
			UiManager.Instance.GetInteractionPanel().DoShow(interactionText);
		} 
	}

	private void OnAreaLeft(Area2D area)
	{
		if (area.Owner is PlayerCharacter character)
		{
			Logger.Info("Character Area left");
			UiManager.Instance.GetInteractionPanel().DoHide();
		} 
	}
}
