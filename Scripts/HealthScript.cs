using Godot;

public partial class HealthScript : Area3D
{
	[Export] public float maxHealth;
	[Export] public float health;
	[Export] public int deathOdens { get; set; } = 25;
	[Export] public int deathXp { get; set; } = 50;
	public float extraDamage;
	[Export]
	public RichTextLabel hpText {  get; set; }
	[Export]
	public RichTextLabel maxHPText { get; set; }
    [Signal]
    public delegate void HitEventHandler();
    public override void _Ready()
	{
		if(GetParent().Name == "Player")
		{
			hpText = GetParent().GetParent().GetNode("UI").GetNode("HP_Text") as RichTextLabel;
            maxHPText = GetParent().GetParent().GetNode("UI").GetNode("MXHP_Text") as RichTextLabel;
            hpText.Text = health.ToString();
            ChangeMaxHP(maxHealth);
        }
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
			OutOfGamePlayerStats st = GetTree().CurrentScene.GetNode("SecretPlayer") as OutOfGamePlayerStats;
			st.AddXPOdens(deathXp, deathOdens);
            GD.Print($"Death. of {GetParent().Name}");
			GetParent().QueueFree();
        }else if (health > maxHealth)
		{
			health = maxHealth;
		}
		if(hpText != null) 
		{
			int hpint = (int)health;
            hpText.Text = hpint.ToString();
		}
		GD.Print(health + " / " + maxHealth);
	}

	public void ChangeMaxHP(float newMax)
	{
		maxHealth = newMax;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        if (maxHPText != null)
		{
            maxHPText.Text = newMax.ToString();
        }
        if (hpText != null)
        {
            int hpint = (int)health;
            hpText.Text = hpint.ToString();
        }

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
                PlayerMovement move = GetParent() as PlayerMovement;
				move.stunTime = 5f;
			}
        }
    }
}
