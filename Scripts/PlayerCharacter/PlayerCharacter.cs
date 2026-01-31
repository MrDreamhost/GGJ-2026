using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;

public partial class PlayerCharacter : CharacterBody2D
{
    [Export] private int baseSpeed = 300;
    [Export] private int baseAcceleration = 50;
    [Export] private Area2D collider = null;
    [Export] private AnimatedSprite2D playerSprite = null;
    [Export] private AudioStreamPlayer2D footsteps = null;
    [Export] private Array<MaskData> maskData = new Array<MaskData>();
    [Export] private PauseScreen pauseScreen = null;
    [Export] private GameOverScreen gameOverScreen = null;
    [Export] private double gameTimerSeconds = 300;
    private Timer gameTimer = null;
    
    private int curMaskIndex = 0;

    //TODO save both inventory (and maybe flags?)
    [Export] private PlayerInventory inventory = null;
    [Export] private PlayerFlags flags = null;
    private State currentState = State.EIdle;

    private string nextIdleAnim = "";

    public enum State
    {
        EIdle,
        EWalk,
        EInDialogue,
        EInMenu,
        EGameOver,
    }

    public State GetState()
    {
        return currentState;
    }

    public void SetState(State state, string reason)
    {
        Logger.Info("Externally setting state {0} with reason {1}", state, reason);
        this.currentState = state;
    }

    public override void _Ready()
    {
        Logger.Info("Readied player");
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

        if (maskData.Count <= 1)
        {
            Logger.Fatal("MaskData was not filled in on playerCharacter, mask 0 is default player");
        }

        if (playerSprite == null)
        {
            Logger.Fatal("playerSprite was not assigned on playerCharacter");
        }

        if (pauseScreen == null)
        {
            Logger.Fatal("pauseScreen was not assigned on playerCharacter");
        }
        
        if (gameOverScreen == null)
        {
            Logger.Fatal("gameOverScreen was not assigned on playerCharacter");
        }

        nextIdleAnim = "Down_Idle";
        pauseScreen.DoHide();
        gameOverScreen.DoHide();
        StartTimer();
        LoadFromSaveData();
        UiManager.Instance.RegisterPlayer(this);
        SetCurMask(maskData[0]);
        base._Ready();
    }

    public override void _PhysicsProcess(double delta)
    {
        ProcessInputs();
        SetVelocity(ProcessMovementInput());
        UpdateState();
        ProcessAnimations();
        MoveAndSlide();
        base._PhysicsProcess(delta);
    }

    public override void _Process(double delta)
    {
        
        double timeLeft = gameTimer.TimeLeft;

        int mins = (int)(timeLeft / 60);
        int secs = (int)(timeLeft % 60);

        var timerLabel = UiManager.Instance.GetTimerLabel();
        if (timerLabel != null)
        {
            timerLabel.Text = string.Format("{0:00}:{1:00}", mins, secs);
        }
        else
        {
            Logger.Error("Failed to get TimerLabel from UIManager");
        }

        base._Process(delta);
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

        if (Input.IsActionJustReleased("switch_mask") && CanMove())
        {
            SwitchMask();
        }

        if (Input.IsActionJustReleased("open_pause_menu") && currentState != State.EGameOver)
        {
            pauseScreen.FlipFlop();
            if (pauseScreen.IsGamePaused())
            {
                currentState = State.EInMenu;
                gameTimer.SetPaused(true);
            }
            else
            {
                gameTimer.SetPaused(false);
                if (UiManager.Instance.GetCurrentDialogueLine() != 0)
                {
                    currentState = State.EInDialogue;
                }
                else
                {
                    currentState = State.EIdle;
                }
                Logger.Info("Setting state to {0} after unpause", currentState);
            }
        }

        if (Input.IsActionJustReleased("choicebox_1") && currentState == State.EInDialogue)
        {
            UiManager.Instance.SelectDialogueChoice(0);
        }
        
        if (Input.IsActionJustReleased("choicebox_2") && currentState == State.EInDialogue)
        {
            UiManager.Instance.SelectDialogueChoice(1);
        }
        if (Input.IsActionJustReleased("choicebox_3") && currentState == State.EInDialogue)
        {
            UiManager.Instance.SelectDialogueChoice(2);
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
                    playerSprite.Play(nextIdleAnim);
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
            case State.EInMenu:
                footsteps.Stop();
                playerSprite.Play(nextIdleAnim);
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

    private void ProcessAnimations()
    {
        if (Velocity.Y < 0)
        {
            playerSprite.Play("Up_Walk");
            nextIdleAnim = "Up_Idle";
            return;
        }

        if (Velocity.X != 0)
        {
            playerSprite.Play("Left_Walk");
            nextIdleAnim = "Left_Idle";
            playerSprite.SetFlipH(Velocity.X > 0);
            return;
        }
        
        if (Velocity.Y > 0)
        {
            playerSprite.Play("Down_Walk");
            nextIdleAnim = "Down_Idle";
            return;
        }
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
            case State.EGameOver:
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
            case State.EGameOver:
                return false;
        }

        Logger.DebugInfo("Unhandled state {0} in CanInteract, allowing interaction", currentState);
        return true;
    }

    //TODO by index or ID?
    private void SwitchMask()
    {
        if (maskData.Count > curMaskIndex + 1)
        {
            curMaskIndex += 1;
            if (!IsMaskUnlocked(maskData[curMaskIndex]))
            {
                SwitchMask();
                return;
            }
        }
        else
        {
            curMaskIndex = 0;
        }

        SetCurMask(maskData[curMaskIndex]);
    }

    private void SetCurMask(MaskData mask)
    {
        Logger.DebugInfo("Setting current mask to {0}", mask.MaskId);
        var currentAnim = playerSprite.Animation;
        playerSprite.SetSpriteFrames(mask.PlayerSprite);
        playerSprite.Play(currentAnim);
        var material = UiManager.Instance.GetHudVignette()?.GetMaterial() as ShaderMaterial;
        if (material == null)
        {
            Logger.Error("Found no shader material on vignette");
            return;
        }

        material.SetShaderParameter("vignette_color", mask.VignetteColor);
    }

    public int GetCurMask()
    {
        return curMaskIndex;
    }

    private bool IsMaskUnlocked(MaskData mask)
    {
        foreach (var flagKey in mask.UnlockConditionFlags)
        {
            if (!flags.GetFlag(flagKey))
            {
                return false;
            }
        }

        return true;
    }

    private void StartTimer()
    {
        gameTimer = new Timer();
        AddChild(gameTimer);
        gameTimer.SetWaitTime(gameTimerSeconds);
        gameTimer.OneShot = true;
        gameTimer.Timeout += OnTimerEnd;
        gameTimer.Start();
    }

    private void OnTimerEnd()
    {
        SetState(State.EGameOver, "Game timer ended, locking player");
        Logger.DebugInfo("Start saving player");
        var saveData =  SaveManager.Instance.GetSaveData();
        foreach (var mask in maskData)
        {
            foreach (var flag in mask.UnlockConditionFlags)
            {
                saveData[flag] = flags.GetFlag(flag).ToString();
            }
        }
        SaveManager.Instance.UpdateSaveData(saveData);
        Logger.DebugInfo("Finished saving player");
        gameOverScreen.DoShow();
        Logger.DebugInfo("Timer ended!");
    }

    private void LoadFromSaveData()
    {
        var saveData = SaveManager.Instance.GetSaveData();
        if (saveData.Count == 0)
        {
            return; //Nothing to load from save file if save file is empty
        }
        
        foreach (KeyValuePair<string, string> entry in saveData)
        {
            flags.SetFlag(entry.Key, Convert.ToBoolean(entry.Value));
        }
        
    }
}