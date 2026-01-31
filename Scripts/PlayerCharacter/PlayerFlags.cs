using Godot;
using System;
using Godot.Collections;
using CollectionExtensions = System.Collections.Generic.CollectionExtensions;

public partial class PlayerFlags : Node
{
    private Dictionary<string, bool> flags = new Dictionary<string, bool>();

    public bool GetFlag(string key)
    {
        return CollectionExtensions.GetValueOrDefault(flags, key, false);
    }

    public void SetFlag(string key, bool value)
    {
        Logger.DebugInfo("Setting flag {0} with value {1}", key, value);
        flags[key] = value;
    }
}
