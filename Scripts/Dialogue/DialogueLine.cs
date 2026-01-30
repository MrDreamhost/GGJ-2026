using Godot;
using Godot.Collections;

public partial class DialogueLine : GodotObject
{
    public int ID;
    public string Line;
    public Array<DialogueCondition> NextLines;

    public DialogueLine(int ID, string Line)
    {
        NextLines = new Array<DialogueCondition>();
        this.ID = ID;
        this.Line = Line;
    }

    //Returns 0 if none found or acceptable, so close the dialogue box
    public int GetNextLine()
    {
        foreach (var line in NextLines)
        {
            if (line.CheckCondition())
            {
                Logger.DebugInfo("Next Line {0} chosen from current line {1}", line.NextLineID, ID);
                return line.NextLineID;
            }
            Logger.DebugInfo("Next Line {0} NOT chosen from current line {1}", line.NextLineID, ID);
        }

        return 0;
    }
}