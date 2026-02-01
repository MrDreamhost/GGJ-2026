using Godot;
using Godot.Collections;

public partial class SaveManager : Node
{
    public static SaveManager Instance { get; private set; }
    private Dictionary<string, string> saveData = new Dictionary<string, string>();
    private string saveDataPath = "user://savegame.save";

    public override void _Ready()
    {
        Instance = this;
        Logger.DebugInfo("Initialized SaveManager");
        ReadSaveData();
        base._Ready();
    }

    public Dictionary<string, string> GetSaveData()
    {
        return saveData;
    }

    public void UpdateSaveData(Dictionary<string, string> saveData)
    {
        this.saveData = saveData;

        WriteSaveData();
    }

    private void WriteSaveData()
    {
        using var saveFile = FileAccess.Open(saveDataPath, FileAccess.ModeFlags.Write);
        var jsonString = Json.Stringify(saveData);
        saveFile.StoreString(jsonString);
    }

    private void ReadSaveData()
    {
        if (!FileAccess.FileExists(saveDataPath))
        {
            Logger.Info("No save data exists to read at path {0}", saveData);
            return;
        }

        using var saveFile = FileAccess.Open(saveDataPath, FileAccess.ModeFlags.Read);
        var jsonString = saveFile.GetLine();

        // Creates the helper class to interact with JSON.
        var json = new Json();
        var parseResult = json.Parse(jsonString);
        if (parseResult != Error.Ok)
        {
            Logger.Error("Failed to parse save data, error {0}", parseResult);
            return;
        }

        saveData = new Dictionary<string, string>((Dictionary)json.Data);
        Logger.Info("Read save data {0}", saveData);
    }

    public void ResetSaveData()
    {
        Logger.Info("Start SaveData Reset");
        var keys = saveData.Keys;
        foreach (var key in keys)
        {
            if (!key.Contains("audio"))
            {
                saveData.Remove(key);
            }
        }
        WriteSaveData();
        Logger.Info("Finsh SaveData Reset");
    }
}