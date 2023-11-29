using Godot;

public partial class HealthScript : Area3D
{
	[Export] public float maxHealth;
	[Export] public float health;
	public float extraDamage;
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
		health -= (damage * (extraDamage + 1));
		GD.Print(damage * (extraDamage + 1));
		if(health < 1)
		{
            GD.Print($"Death. of {GetParent().Name}");
			GetParent().QueueFree();
        }else if (health > maxHealth)
		{
			health = maxHealth;
		}
		GD.Print(health + " / " + maxHealth);
	}

    private void OnArea3DBodyEntered(Node3D body)
    {
        if(body.Name != GetParent().Name)
		{
            if (GetParent().GetMeta("Attacking").AsBool() == true&& body.GetMeta("Blocking").AsBool() == false)
			{
				HealthScript healthScript = body.GetNode<Area3D>("MobDetector") as HealthScript;
				healthScript.TakeDamage(GetParent().GetMeta("Damage").AsInt32());
            }
			if (body.GetMeta("Blocking").AsBool() == true && GetParent().GetMeta("Attacking").AsBool() == true)
			{
                GD.Print("Parried L");
                PlayerMovement move = GetParent() as PlayerMovement;
				move.stunTime = 5f;
			}
        }
    }
}
