using Godot;

public partial class DialogueCondition : GodotObject
{
    public int NextLineID;
    public int Value;
    public bool Invert;
    public string Font;
    public virtual bool IsConditionTrue()
    {
        return true;
    }
}
