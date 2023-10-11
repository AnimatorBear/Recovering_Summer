using Godot;

public partial class Print : Node
{
	[Export]
	private string print = "Hello World";

	public override void _Ready()
	{
		// Called every time the node is added to the scene.
		// Initialization here.
		GD.Print(print);
	}

	public override void _Process(double delta)
	{
		// Called every frame. Delta is time since the last frame.
		// Update ugame logic here.
	}
}
