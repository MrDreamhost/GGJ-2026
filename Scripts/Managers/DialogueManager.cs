using Godot;

public partial class DialogueManager : Node
{
    [Export] private RichTextLabel textBox = null;
    [Export] private TextureRect dialogueBox = null;

    [Export]
    private Godot.Collections.Dictionary<string, Font> fonts = new Godot.Collections.Dictionary<string, Font>();

    private Godot.Collections.Dictionary<int, DialogueLine> dialogueDataBase;

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
        dialogueBox?.Hide();
        
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
}