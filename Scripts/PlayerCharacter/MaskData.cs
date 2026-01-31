using Godot;
using Godot.Collections;

[GlobalClass]
public partial class MaskData : Resource
{
    [Export] public int MaskId;
    [Export] public Texture2D PlayerSprite;
    [Export] public Color VignetteColor;
    [Export] public Array<string> UnlockConditionFlags = new Array<string>();

}