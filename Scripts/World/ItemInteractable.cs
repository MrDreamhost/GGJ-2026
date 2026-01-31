using Godot;
using System;

public partial class ItemInteractable : Interactable
{
    [Export] private int itemId;
    [Export] private int amount;
    public override void OnInteract(PlayerCharacter playerCharacter)
    {
        if (itemId == 0 || amount == 0)
        {
            Logger.Warning("ItemId or amount not assigned to interactable");
            return;
        }
        playerCharacter.GetInventory().AddItem(itemId, amount, "Picked up from Interactable");
        base.OnInteract(playerCharacter);
        DoHide();
    }
}
