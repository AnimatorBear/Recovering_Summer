using Godot;
using System;

public partial class PlayerControls : Node
{
    [Signal]
    public delegate void HitEventHandler();


    private void OnHitboxEntered(Node3D body)
    {
        if (body.Name != GetParent().Name)
        {
            if (body.GetMeta("Shoving").AsBool() == true)
            {
                GD.Print(GetParent().Name + " Shoved");
                PlayerMovement move = GetParent() as PlayerMovement;
                move.stunTime = 5f;
            }
        }
    }
}
