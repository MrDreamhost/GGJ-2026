using Godot;
using System;

public partial class UiManager : Node
{
    public static UiManager Instance { get; private set; }

    private DialogueManager dialogueManager;

    private PlayerCharacter player;

    private HudVignette vignette;

    private RichTextLabel timerLabel;

    public override void _Ready()
    {
        Instance = this;
    }

    public void RegisterDialogueManager(DialogueManager dialogueManager)
    {
        this.dialogueManager = dialogueManager;
    }

    //TODO this is disgusting
    public void RegisterPlayer(PlayerCharacter player)
    {
        this.player = player;
    }

    public void RegisterHudVignette(HudVignette vignette)
    {
        this.vignette = vignette;
    }

    public void RegisterTimerLabel(RichTextLabel timerLabel)
    {
        this.timerLabel = timerLabel;
    }

    public HudVignette GetHudVignette()
    {
        return vignette;
    }

    public PlayerCharacter GetPlayer()
    {
        return player;
    }

    public RichTextLabel GetTimerLabel()
    {
        return timerLabel;
    }

    public void StartDialogueSequence(int dialogueSequence)
    {
        if (dialogueManager == null)
        {
            Logger.Fatal("UIManager has no active reference to the DialogueManager");
            return;
        }
        Logger.Info("Starting Dialogue Sequence {0}", dialogueSequence);
        dialogueManager.StartDialogueSequence(dialogueSequence);
    }

    public void ContinueDialogueSequence()
    {
        if (dialogueManager == null)
        {
            Logger.Fatal("UIManager has no active reference to the DialogueManager");
            return;
        }
        dialogueManager.ProcessNextLine();
    }

    public int GetCurrentDialogueLine()
    {
        if (dialogueManager == null)
        {
            return 0;
        }

        return dialogueManager.GetCurrentLine();
    }

    public void SelectDialogueChoice(int index)
    {
        if (dialogueManager == null)
            return;
        
        dialogueManager.SelectChoice(index);
    }
}
