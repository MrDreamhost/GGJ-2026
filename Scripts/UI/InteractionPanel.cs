using Godot;
using System;

public partial class InteractionPanel : Panel
{
    [Export] private RichTextLabel textLabel;
    public override void _Ready()
    {
        if (textLabel == null)
        { 
            Logger.Fatal("textLabel not assigned to InteractionPanel");
        }
        UiManager.Instance.RegisterInteractionPanel(this);
        DoHide();
        base._Ready();
    }

    public void DoShow(string text)
    {
        if (text.Contains("NoShow"))
        {
            textLabel.Text = text;
            DoHide();
            return;
        }
        if (text != "")
        {
            textLabel.Text = text;
        }
        
        //Check for when re-opening interaction panel from dialogue end
        if (textLabel.Text.Contains("NoShow") || text == "")
        {
            DoHide();
            return;
        }

        this.Show();
    }

    public void DoHide()
    {
        this.Hide();
    }
}
