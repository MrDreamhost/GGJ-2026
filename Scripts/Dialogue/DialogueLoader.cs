using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Godot;
using Godot.Collections;

namespace GGJ2026.Scripts.Dialogue;

public static class DialogueLoader
{
    public class DialogueConditionDB
    {
        public int NextLineID { get; set; }
        public string ConditionType { get; set; }
        public int ConditionValue { get; set; }
    }

    public class DialogueLineDB
    {
        public int ID { get; set; }
        public bool SkipLine { get; set; }
        public string Line { get; set; }
        public List<DialogueConditionDB> DialogueConditions { get; set; }
    }

    public class DialogueDB
    {
        public List<DialogueLineDB> DialogueLines { get; set; }
    }
    
    public static Godot.Collections.Dictionary<int, DialogueLine> LoadLines(string DialogueDBPath)
    {
        var file = FileAccess.Open(DialogueDBPath, FileAccess.ModeFlags.Read);
        var data = file.GetAsText();
        if (data == "") {
            Logger.Fatal("Failed to read Dialogue line data from json with path {0}", DialogueDBPath);
            return null;
        }
        DialogueDB dataDb = JsonSerializer.Deserialize<DialogueDB>(data);
        if (dataDb == null)
        {
            Logger.Fatal("Failed to parse Dialogue line data from json with path {0}", DialogueDBPath);
            return null;
        }
        Logger.Info("Deserialized Dialogue Database");
        return GenerateDialogueLines(dataDb);
    }

    public static Godot.Collections.Dictionary<int, DialogueLine> GenerateDialogueLines(DialogueDB dialogueDb)
    {
        var dialogueList = new Godot.Collections.Dictionary<int, DialogueLine>();

        foreach (var lineDb in dialogueDb.DialogueLines)
        {
            var line = new DialogueLine(lineDb.ID, lineDb.Line);
            if (lineDb.DialogueConditions != null)
            {
                foreach (var conditionDb in lineDb.DialogueConditions)
                {
                    DialogueCondition condition = null;
                    switch (conditionDb.ConditionType)
                    {
                        case "Has_Mask":
                            condition = new MaskDialogueCondition();
                            break;
                        case "Not_Has_Mask":
                            condition = new MaskDialogueCondition();
                            condition.Invert = true;
                            break;
                    }

                    if (condition == null)
                    {
                        Logger.Error("Could not create line condition for type {0}, skipping condition",
                            conditionDb.ConditionType);
                        continue;
                    }

                    condition.NextLineID = conditionDb.NextLineID;
                    condition.Value = conditionDb.ConditionValue;

                    line.NextLines.Add(condition);
                }
            }

            dialogueList[line.ID] = line;
        }
        return dialogueList;
    }
}