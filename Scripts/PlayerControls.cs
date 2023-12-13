using Godot;
using System;

public partial class PlayerControls : Node
{
    [Signal]
    public delegate void HitEventHandler();


    private void OnHitboxEntered(Node3D body)
    {
        if (body.Name != GetParent().Name && body.GetMetaList().Contains("Shoving"))
        {
            if (body.GetMeta("Shoving").AsBool() == true)
            {
                PlayerMovement move = GetParent() as PlayerMovement;
                move.stunTime = 5f;

            }
        }
    }
}
