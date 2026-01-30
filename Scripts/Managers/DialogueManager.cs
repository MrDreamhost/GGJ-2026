using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using GGJ2026.Scripts.Dialogue;
using Godot;
using Godot.Collections;
using FileAccess = Godot.FileAccess;

public partial class DialogueManager : Node
{
    [Export] private RichTextLabel textBox = null;

    private Array<DialogueLine> DialogueDataBase;

    private string DialogueDBPath = "res://resources/DialogueDB.txt";
    
   
    
    public override void _Ready()
    {
        DialogueDataBase = new Array<DialogueLine>();
        if (textBox == null)
        {
            Logger.Fatal("DialogueManager has no textBox assigned");
        }
        //Fully Initialize before registering to UIManager
        LoadLines();
        
        UiManager.Instance.RegisterDialogueManager(this);
        base._Ready();
    }

    public void StartDialogueSequence(int sequenceID)
    {
        textBox.SetText(sequenceID.ToString());
    }

    private void LoadLines()
    {
        DialogueDataBase = DialogueLoader.LoadLines(DialogueDBPath);
        if (DialogueDataBase == null)
        {
            Logger.Fatal("Failed to load Dialogue Database");
        }
    }
}