using Godot;
[GlobalClass]
public partial class PlayerFlag :Resource
{
    [Export] public string key;
    [Export] public bool value;

    public PlayerFlag(string key, bool value)
    {
        this.key = key;
        this.value = value;
    }

    public PlayerFlag()
    {
    }
}