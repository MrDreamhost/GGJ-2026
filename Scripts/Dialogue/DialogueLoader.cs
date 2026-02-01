using System.Collections.Generic;
using System.Text.Json;
using Godot;
using Godot.Collections;


public static class DialogueLoader
{
    public class DialogueConditionDB
    {
        public string ConditionType { get; set; }
        public int ConditionValue { get; set; }
        
        public string ConditionFlagValue { get; set; }
        public string FinalStringValue { get; set; }
        public string Font { get; set; }
    }
    
    public class DialogueConditionGroupDB
    {
        public int NextLineID { get; set; }
        public List<DialogueConditionDB> DialogueConditions { get; set; }
        public string Font { get; set; }
    }

    public class FontConditionGroupDB : DialogueConditionGroupDB
    {
        public List<DialogueConditionDB> FontConditions { get; set; }
    }
    
    

    public class DialogueLineDB
    {
        public int ID { get; set; }
        public bool SkipLine { get; set; }
        public string Line { get; set; }
        public string Audio { get; set; }
        public string Name { get; set; }
        public int ItemId { get; set; }
        public int Amount { get; set; }
        public string Event { get; set; }
        public List<DialogueConditionGroupDB> DialogueConditionGroups { get; set; }
        public List<FontConditionGroupDB> FontConditionGroups { get; set; }
        
        public List<DialogueFlag> ChangeFlags { get; set; }
    }

    public class DialogueFlag
    {
        public string Flag { get; set; }
        public bool Value { get; set; }
    }

    public class DialogueDB
    {
        public List<DialogueLineDB> DialogueLines { get; set; }
    }
    
    public static Godot.Collections.Dictionary<int, DialogueLine> LoadLines(string DialogueDBPath)
    {
        var file = FileAccess.Open(DialogueDBPath, FileAccess.ModeFlags.Read);
        if (file == null)
        {
            Logger.Fatal("Failed to open dialogue database file with path {0}", DialogueDBPath);
        }
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
            var line = new DialogueLine(lineDb.ID, lineDb.Line, lineDb.ItemId, lineDb.Amount, lineDb.Event);
            line.AudioPath = lineDb.Audio;
            line.Name = lineDb.Name;
            if (lineDb.DialogueConditionGroups != null)
            {
                foreach (var conditionGroupDb in lineDb.DialogueConditionGroups)
                {
                    var conditionGroup = new DialogueConditionGroup();
                    conditionGroup.NextLineID = conditionGroupDb.NextLineID;
                    conditionGroup.Font =  conditionGroupDb.Font;
                    conditionGroup.Conditions = GenerateConditions(conditionGroupDb.DialogueConditions);
                    
                    line.NextLines.Add(conditionGroup);
                }
            }

            if (lineDb.FontConditionGroups != null)
            {
                foreach (var fontConditionGroupDb in lineDb.FontConditionGroups)
                {
                    var fontConditionGroup = new DialogueConditionGroup();
                    fontConditionGroup.Font =  fontConditionGroupDb.Font;
                    fontConditionGroup.Conditions = GenerateConditions(fontConditionGroupDb.FontConditions);
                    line.FontConditions.Add(fontConditionGroup);
                }
            }

            if (lineDb.ChangeFlags != null)
            {
                foreach (var flagDb in lineDb.ChangeFlags)
                {
                    line.ChangeFlags.Add(new PlayerFlag(flagDb.Flag, flagDb.Value));
                }
            }

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
                case "Has_Flag":
                    condition = new FlagDialogueCondition();
                    condition.FlagValue = conditionDb.ConditionFlagValue;
                    break;
                case "Not_Has_Flag":
                    condition = new FlagDialogueCondition();
                    condition.Invert = true;
                    condition.FlagValue = conditionDb.ConditionFlagValue;
                    break;
                case "True":
                    condition = new DialogueCondition();
                    break;
                case "FinalString":
                    condition = new FinalStringCondition();
                    condition.FinalStringValue = conditionDb.FinalStringValue;
                    break;
            }

            if (condition == null)
            {
                Logger.Error("Could not create line condition for type {0}, skipping condition",
                    conditionDb.ConditionType);
                continue;
            }
            condition.Value = conditionDb.ConditionValue;
            condition.Font = conditionDb.Font;

            conditions.Add(condition);
        }

        return conditions;
    }
}
