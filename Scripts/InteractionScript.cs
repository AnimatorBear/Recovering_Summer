using Godot;
using System;

public partial class InteractionScript : Node
{
	[Export]
	public bool used = true;
	[Export]
	public float timeToUse {  get; set; }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Interact()
	{
		if (!used)
		{
			used = true;
            GD.Print("A SEE YUU");
        }
	}
}
