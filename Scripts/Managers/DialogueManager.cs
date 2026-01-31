using Godot;
using Godot.Collections;

public partial class DialogueManager : Node
{
    [Export] private RichTextLabel textBox = null;
    [Export] private Panel dialogueBox = null;
    [Export] private Dictionary<string, AudioStreamWav> audioFiles = new Dictionary<string, AudioStreamWav>();
    [Export] private AudioStreamPlayer2D audioPlayer = null;
    [Export] private Panel choiceBox1 = null;
    [Export] private RichTextLabel choiceBox1Text = null;
    [Export] private Panel choiceBox2 = null;
    [Export] private RichTextLabel choiceBox2Text = null;
    [Export] private Panel choiceBox3 = null;
    [Export] private RichTextLabel choiceBox3Text = null;
    

    [Export]
    private Dictionary<string, Font> fonts = new Godot.Collections.Dictionary<string, Font>();

    [Export] private Dictionary<int, ChoiceGroup> choiceDatabase = new Dictionary<int, ChoiceGroup>();
    
    private Dictionary<int, DialogueLine> dialogueDataBase;

    private string dialogueDBPath = "res://resources/DialogueDB.txt";

    private string defaultFontName = "default";
    
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

        if (fonts[defaultFontName] == null)
        {
            Logger.Fatal("No font found in the font map with 'default', cannot continue without a default fallback font");
        }

        if (audioPlayer == null)
        {
            Logger.Fatal("DialogueManager has no audioPlayer assigned");
        }

        if (choiceBox1 == null)
        {
            Logger.Fatal("DialogueManager has no choiceBox1 assigned");
        }

        if (choiceBox1Text == null)
        {
            Logger.Fatal("DialogueManager has no choiceBox1Text assigned");
        }
        if (choiceBox2 == null)
        {
            Logger.Fatal("DialogueManager has no choiceBox2 assigned");
        }

        if (choiceBox2Text == null)
        {
            Logger.Fatal("DialogueManager has no choiceBox2Text assigned");
        }
        if (choiceBox3 == null)
        {
            Logger.Fatal("DialogueManager has no choiceBox3 assigned");
        }

        if (choiceBox3Text == null)
        {
            Logger.Fatal("DialogueManager has no choiceBox3Text assigned");
        }
        dialogueBox?.Hide();
        choiceBox1.Hide();
        choiceBox2.Hide();
        choiceBox3.Hide();
        
        //Fully Initialize before registering to UIManager
        LoadLines();
        Logger.DebugInfo("Finished loading lines into dialogue manager");
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

        if (choiceDatabase.TryGetValue(currentLine.ID, out ChoiceGroup group))
        {
            Logger.Info("not continuing sequence through interact key because current dialogue line has a choice attached");
            return;
        }
        currentLine.PostLine();
        DialogueLine nextLine = null;
        foreach (var condition in currentLine.NextLines)
        {
            if (condition.AreConditionsTrue())
            {
                nextLine = dialogueDataBase[condition.NextLineID];
                if (nextLine == null)
                {
                    Logger.Error("Tried to start dialogue line {0} but line not found in database, moving on to next condition group", condition.NextLineID);
                    continue;
                }
                Logger.DebugInfo("condition group passed, moving to line {0}", nextLine.ID);
                break;
            }
            Logger.DebugInfo("condition group failed, skipping line {0}", condition.NextLineID);
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
        textBox.Theme.DefaultFont = GetFontForCurrentLine();
        textBox.SetText(currentLine.Line);
        dialogueBox.Show();

        if (currentLine.AudioPath != "")
        {
            PlayAudio(currentLine.AudioPath);
        }
        
        choiceBox1.Hide();
        choiceBox2.Hide();
        choiceBox3.Hide();
        if (choiceDatabase.TryGetValue(currentLine.ID, out ChoiceGroup group))
        {
            if (group.Choices.Count > 0)
            {
                var choice = group.Choices[0];
                choiceBox1.Show();
                choiceBox1Text.SetText(choice.Line);
            }
            if (group.Choices.Count > 1)
            {
                var choice = group.Choices[1];
                choiceBox2.Show();
                choiceBox2Text.SetText(choice.Line);
            }
            if (group.Choices.Count > 2)
            {
                var choice = group.Choices[2];
                choiceBox3.Show();
                choiceBox3Text.SetText(choice.Line);
            }
        }
    }

    private Font GetFontForCurrentLine()
    {
        if (currentLine == null)
        {
            return fonts[defaultFontName];
        }
        foreach (var fontCondition in currentLine.FontConditions)
        {
            if (fontCondition.AreConditionsTrue())
            {
                var font = fonts[fontCondition.Font];
                if (font != null)
                {
                    return font;
                }
                
            }
        }

        return fonts[defaultFontName];
    }

    private void LoadLines()
    {
        dialogueDataBase = DialogueLoader.LoadLines(dialogueDBPath);
        if (dialogueDataBase == null)
        {
            Logger.Fatal("Failed to load Dialogue Database");
        }
    }

    public int GetCurrentLine()
    {
        if (currentLine == null)
        {
            return 0;}

        return currentLine.ID;
    }

    public void PlayAudio(string audioName)
    {
        if (!audioFiles.ContainsKey(audioName))
        {
            Logger.Error("could not play audio with name {0}", audioName);
            return;
        }

        audioPlayer.Stream = audioFiles[audioName];
        audioPlayer.Play();
        Logger.DebugInfo("Playing audio {0}", audioName);
    }

    public void SelectChoice(int index)
    {
        if (currentLine == null)
            return;
        if (choiceDatabase.TryGetValue(currentLine.ID, out ChoiceGroup group))
        {
            var choice = group.Choices[index];
            if (choice == null)
                return;
            
            Logger.Info("Choice {0} selected, transitioning to line ID {1}", index, choice.NextLineId);
            StartDialogueSequence(choice.NextLineId);
        }
    }
}