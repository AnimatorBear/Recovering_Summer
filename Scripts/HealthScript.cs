using Godot;

public partial class HealthScript : Area3D
{
	[Export] public float health;
    [Signal]
    public delegate void HitEventHandler();
    public override void _Ready()
	{

    }
	public override void _Process(double delta)
	{

	}

	public void TakeDamage(float damage)
	{
		health -= damage;
		if(health < 1)
		{
            GD.Print($"Death. of {GetParent().Name}");
        }
		GD.Print(health);
	}

    private void OnArea3DBodyEntered(Node3D body)
    {
        if(body.Name != GetParent().Name)
		{
			if (body.GetMeta("Attacking").AsBool() == true&& body.GetMeta("Blocking").AsBool() == false)
			{
				HealthScript healthScript = body.GetNode<Area3D>("MobDetector") as HealthScript;
				healthScript.TakeDamage(GetParent().GetMeta("Damage").AsInt32());
            }
        }
    }
}
