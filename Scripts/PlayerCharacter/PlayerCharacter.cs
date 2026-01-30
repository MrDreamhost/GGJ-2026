using Godot;
using System;

public partial class PlayerCharacter : CharacterBody2D
{
	[Export] private int baseSpeed = 300;
	[Export] private int baseAcceleration = 50;
	[Export] private Area2D collider = null;
	[Export] private AudioStreamPlayer2D footsteps = null;

	private State currentState = State.EIdle;
	
	public enum State{EIdle, EWalk, ETalking}

	public State GetState()
	{
		return currentState;
	}

	public override void _Ready()
	{
		if (collider == null)
		{
			Logger.Fatal("Area2D collider not assigned on playerCharacter");
		}

		if (footsteps == null)
		{
			Logger.Fatal("Footsteps AudioStreamPlayer2D not assigned on playerCharacter");
		}
		base._Ready();
	}

	public override void _PhysicsProcess(double delta)
	{
		ProcessInputs();
		SetVelocity(ProcessMovementInput());
		UpdateState();
		MoveAndSlide();
		base._PhysicsProcess(delta);
	}

	private void ProcessInputs()
	{
		if (Input.IsActionJustReleased("on_interact"))
		{
			var areas = collider.GetOverlappingAreas();
			Logger.Info("Overlapping areas {0}", areas.Count);
			foreach (var area in areas)
			{
				if (area is Interactable interactable)
				{
				  interactable.OnInteract(this);
				}
			}
		}

		if (Input.IsActionJustReleased("progress_dialogue"))
		{
			UiManager.Instance.ContinueDialogueSequence();
		}
	}
	
	private void UpdateState()
	{
		switch (currentState)
		{
			case State.EIdle:
				if (Velocity.X != 0 || Velocity.Y != 0)
				{
					if (!footsteps.Playing)
						footsteps.Play();
					
					currentState = State.EWalk;
				}

				break;
			case State.EWalk:
				if (Velocity.X == 0 && Velocity.Y == 0)
				{
					footsteps.Stop();
					currentState = State.EIdle;
				}

				break;
		}
	}

	private Vector2 ProcessMovementInput()
	{
		Vector2 direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		
		Vector2 newVelocity = Velocity;
		newVelocity.X = Mathf.MoveToward(Velocity.X, (direction * baseSpeed).X, baseAcceleration);
		newVelocity.Y = Mathf.MoveToward(Velocity.Y, (direction * baseSpeed).Y, baseAcceleration);
		return newVelocity;
	}

	
}
