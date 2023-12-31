using Godot;
using System;

public partial class ItemScript : Node
{
	[Export]
	public bool isMelee { get; set; }
    [Export]
    public float useTime { get; set; }
    [Export]
	public float extraDamage { get; set; }
	[Export]
	public int uses { get; set; }

	[Export]
	public int scrapValuePerItem { get; set; }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}
}
