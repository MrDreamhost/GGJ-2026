using Godot;
using Godot.Collections;

[GlobalClass]
public partial class MaskData : Resource
{
    [Export] public int MaskId;
    [Export] public SpriteFrames PlayerSprite;
    [Export] public Color VignetteColor;
    [Export] public Array<string> UnlockConditionFlags = new Array<string>();

}