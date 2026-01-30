using Godot;
using System;

public partial class UiManager : Node
{
    public static UiManager Instance { get; private set; }

    private DialogueManager dialogueManager;

    public override void _Ready()
    {
        Instance = this;
    }

    public void RegisterDialogueManager(DialogueManager dialogueManager)
    {
        this.dialogueManager = dialogueManager;
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
}
