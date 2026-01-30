using Godot;
using System;

public partial class PlayerCharacter : CharacterBody2D
{
    [Export] private int baseSpeed = 300;
    [Export] private int baseAcceleration = 50;
    [Export] private Area2D collider = null;

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
        base._Ready();
    }

    public override void _PhysicsProcess(double delta)
    {
        ProcessInputs();
        SetVelocity(new Vector2(ProcessHorizontalInput(), 0));
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
                if (Velocity.X != 0)
                    currentState = State.EWalk;
                break;
            case State.EWalk:
                if (Velocity.X == 0)
                    currentState = State.EIdle;
                break;
        }
    }

    private float ProcessHorizontalInput()
    {
        var direction = 0;
        if (Input.IsActionPressed("move_left"))
            direction -= 1;
        if (Input.IsActionPressed("move_right"))
            direction += 1;

        return Mathf.MoveToward(Velocity.X, direction * baseSpeed, baseAcceleration);
    }
    
}
