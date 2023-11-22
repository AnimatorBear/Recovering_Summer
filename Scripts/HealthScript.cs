using Godot;

public partial class HealthScript : Area3D
{
	[Export] public float health;
    [Signal]
    public delegate void HitEventHandler();
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{

    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	public void TakeDamage(float damage)
	{
		health -= damage;
		if(health < 0)
		{
            GD.Print("Death.");
        }
		GD.Print(health);
	}

    private void OnArea3DBodyEntered(Node3D body)
    {
        if(body.Name != GetParent().Name)
		{
			GD.Print("Hit smthin");
		}
    }
}
