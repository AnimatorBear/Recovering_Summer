using Godot;
using System;

public partial class OutOfGamePlayerStats : Node
{
	public int odens = 500;
    public int bonds = 0;
    public int xp = 0;
	[Export]
	public int xpPerLevel { get; set; } = 500;
	public int level;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void AddXPOdens(int newXP, int newOdens)
	{
		odens += newOdens;
		xp += newXP;
		while(xp >= xpPerLevel)
		{
			level++;
			xp -= xpPerLevel;
		}
	}
}
