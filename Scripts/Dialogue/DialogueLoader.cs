using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Godot;
using Godot.Collections;


public static class DialogueLoader
{
    public class DialogueConditionDB
    {
        public int NextLineID { get; set; }
        public string ConditionType { get; set; }
        public int ConditionValue { get; set; }
        public string Font { get; set; }
    }

    public class DialogueLineDB
    {
        public int ID { get; set; }
        public bool SkipLine { get; set; }
        public string Line { get; set; }
        public List<DialogueConditionDB> DialogueConditions { get; set; }
        public List<DialogueConditionDB> FontConditions { get; set; }
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
                line.NextLines = GenerateConditions(lineDb.DialogueConditions);
            
            if (lineDb.FontConditions != null)
                line.FontConditions = GenerateConditions(lineDb.FontConditions);

            dialogueList[line.ID] = line;
        }
        return dialogueList;
    }

    public static Array<DialogueCondition> GenerateConditions(List<DialogueConditionDB> conditionDbs)
    {
        var conditions = new Array<DialogueCondition>();
        if (conditionDbs == null)
        {
            return conditions;}
        foreach (var conditionDb in conditionDbs)
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
            condition.Font = conditionDb.Font;

            conditions.Add(condition);
        }

        return conditions;
    }
}
