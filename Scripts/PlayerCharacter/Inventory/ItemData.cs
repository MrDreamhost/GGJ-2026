using Godot;

public partial class ItemData : GodotObject
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string TexturePath { get; set; }
    
    //TODO lazy load textures if needed?
}