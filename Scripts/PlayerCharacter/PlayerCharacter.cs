using Godot;
using System;
using System.Diagnostics;

public partial class PlayerCharacter : CharacterBody2D
{
    [Export] private int baseSpeed = 300;
    [Export] private int baseAcceleration = 50;
    [Export] private Area2D collider = null;

    [Export] private AudioStreamPlayer2D footsteps = null;

    //TODO save both inventory (and maybe flags?)
    [Export] private PlayerInventory inventory = null;
    [Export] private PlayerFlags flags = null;
    private State currentState = State.EIdle;

    public enum State
    {
        EIdle,
        EWalk,
        EInDialogue,
        EInMenu
    }

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

        if (inventory == null)
        {
            Logger.Fatal("inventory was not assigned on playerCharacter");
        }

        if (flags == null)
        {
            Logger.Fatal("flag system node was not assigned on playerCharacter");
        }

        UiManager.Instance.RegisterPlayer(this);
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
            if (CanInteract())
            {
                var areas = collider.GetOverlappingAreas();
                foreach (var area in areas)
                {
                    if (area is Interactable interactable)
                    {
                        interactable.OnInteract(this);
                    }
                }
            }


            if (currentState == State.EInDialogue)
            {
                UiManager.Instance.ContinueDialogueSequence();
            }
        }

    }

    private void UpdateState()
    {
        switch (currentState)
        {
            case State.EIdle:
                if (Velocity.X != 0 || Velocity.Y != 0)
                {
                    if (!footsteps.Playing) footsteps.Play();
                    currentState = State.EWalk;
                }

                if (UiManager.Instance.GetCurrentDialogueLine() != 0)
                {
                    Logger.Info("Setting state to dialogue");
                    currentState = State.EInDialogue;
                }

                break;
            case State.EWalk:
                if (Velocity.X == 0 && Velocity.Y == 0)
                {
                    footsteps.Stop();
                    currentState = State.EIdle;
                }

                if (UiManager.Instance.GetCurrentDialogueLine() != 0)
                {
                    footsteps.Stop();
                    currentState = State.EInDialogue;
                }

                break;
            case State.EInDialogue:
                if (UiManager.Instance.GetCurrentDialogueLine() == 0)
                {
                    Logger.Info("Setting state to idle");
                    currentState = State.EIdle;
                }

                break;
        }
    }

    private Vector2 ProcessMovementInput()
    {
        if (!CanMove())
        {
            return new Vector2(0, 0);
        }

        Vector2 direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
        Vector2 newVelocity = Velocity;
        newVelocity.X = Mathf.MoveToward(Velocity.X, (direction * baseSpeed).X, baseAcceleration);
        newVelocity.Y = Mathf.MoveToward(Velocity.Y, (direction * baseSpeed).Y, baseAcceleration);
        return newVelocity;
    }

    public PlayerInventory GetInventory()
    {
        return this.inventory;
    }

    public PlayerFlags GetPlayerFlags()
    {
        return this.flags;
    }

    private bool CanMove()
    {
        switch (currentState)
        {
            case State.EIdle:
            case State.EWalk:
                return true;
            case State.EInDialogue:
            case State.EInMenu:
                return false;
        }

        Logger.DebugInfo("Unhandled state {0} in CanMove, allowing movement", currentState);
        return true;
    }

    private bool CanInteract()
    {
        switch (currentState)
        {
            case State.EIdle:
            case State.EWalk:
                return true;
            case State.EInDialogue:
            case State.EInMenu:
                return false;
        }

        Logger.DebugInfo("Unhandled state {0} in CanInteract, allowing interaction", currentState);
        return true;
    }
}