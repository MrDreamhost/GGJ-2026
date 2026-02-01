using Godot;

public partial class Door : Interactable
{
    [Export] private Door linkedDoor = null;
    [Export] private Vector2 teleportOffset = new Vector2();
    [Export] private AudioStream songOfLinkedArea = null;
    public Door GetLinkedDoor()
    {
        return linkedDoor;
    }

    public override void _Ready()
    {
        if (linkedDoor == null) {
            Logger.Fatal("Door does not have a linked door");
        }

        base._Ready();
    }

    public override void OnInteract(PlayerCharacter playerCharacter)
    { 
        if (linkedDoor == null)
        {
            Logger.Error("Interacted door has no linked door");
            return;
        }

        var linkedPosition = linkedDoor.Position + teleportOffset;
        Logger.Info("Teleporting to linked door at position {0} with an additional (already calculated into the position) offset {1}", linkedPosition, teleportOffset);
        playerCharacter.SetPosition(linkedPosition);
        if (songOfLinkedArea != null)
        {
            playerCharacter.PlayMusic(songOfLinkedArea);
        }

        base.OnInteract(playerCharacter);
    }
}
