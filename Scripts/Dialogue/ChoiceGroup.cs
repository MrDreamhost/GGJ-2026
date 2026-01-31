using Godot;
using Godot.Collections;


[GlobalClass]
public partial class ChoiceGroup : Resource
{
    [Export] public Array<Choice> Choices = new Array<Choice>();
}