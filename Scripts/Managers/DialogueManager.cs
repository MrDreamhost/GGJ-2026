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
    [Export] private TextureRect dialogueBox = null;

    private Godot.Collections.Dictionary<int, DialogueLine> dialogueDataBase;

    private string dialogueDBPath = "res://resources/DialogueDB.txt";

    private DialogueLine currentLine;
    
   
    
    public override void _Ready()
    {
        if (textBox == null)
        {
            Logger.Fatal("DialogueManager has no textBox assigned");
        }

        if (dialogueBox == null)
        {
            Logger.Fatal("DialogueManager has no dialogueBox assigned");
        }
        dialogueBox.Hide();
        
        //Fully Initialize before registering to UIManager
        LoadLines();
        
        UiManager.Instance.RegisterDialogueManager(this);
        base._Ready();
    }

    public void StartDialogueSequence(int lineId)
    {
        DialogueLine curLine = dialogueDataBase[lineId];
        if (curLine == null)
        {
            Logger.Error("Tried to start dialogue line {0} but line not found in database", lineId);
            return;
        }

        currentLine = curLine;
        ShowCurLine();
    }

    public void ProcessNextLine()
    {
        if (currentLine == null)
        {
            dialogueBox.Hide();
            return;
            
        }
        DialogueLine nextLine = null;
        foreach (var condition in currentLine.NextLines)
        {
            if (condition.CheckCondition())
            {
                nextLine = dialogueDataBase[condition.NextLineID];
                if (nextLine == null)
                {
                    Logger.Error("Tried to start dialogue line {0} but line not found in database, moving on to next condition", condition.NextLineID);
                    continue;
                }
                Logger.DebugInfo("condition passed, moving to line {0}", nextLine.ID);
                break;
            }
            Logger.DebugInfo("condition failed, skipping line {0}", condition.NextLineID);
        }
        //it's ok if nextLine is null. That will mean that the dialogue will end
        currentLine = nextLine;
        ShowCurLine();
    }

    private void ShowCurLine()
    {
        if (currentLine == null)
        {
            dialogueBox.Hide();
            return;
        }
        textBox.SetText(currentLine.Line);
        dialogueBox.Show();
    }

    private void LoadLines()
    {
        dialogueDataBase = DialogueLoader.LoadLines(dialogueDBPath);
        if (dialogueDataBase == null)
        {
            Logger.Fatal("Failed to load Dialogue Database");
        }
    }
}