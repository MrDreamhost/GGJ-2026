using Godot;
using System;

public partial class UiManager : Node
{
    public static UiManager Instance { get; private set; }

    private DialogueManager dialogueManager;

    private PlayerCharacter player;

    public override void _Ready()
    {
        Instance = this;
    }

    public void RegisterDialogueManager(DialogueManager dialogueManager)
    {
        this.dialogueManager = dialogueManager;
    }

    public void RegisterPlayer(PlayerCharacter player)
    {
        this.player = player;
    }

    public PlayerCharacter GetPlayer()
    {
        return player;
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
            Logger.Fatal("UIManager has no active reference to the DialogueManager");
            return 0;
        }

        return dialogueManager.GetCurrentLine();
    }
}
